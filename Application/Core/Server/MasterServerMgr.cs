using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using Orbit.Core.Server.Level;
using Orbit.Core.Server.Match;
using Orbit.Core.Helpers;

namespace Orbit.Core.Server
{
    public class MasterServerMgr
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private NetServer server;
        private IDictionary<NetConnection, ServerMgr> connections;
        private ServerMgr mgr = null;

        public MasterServerMgr()
        {
            connections = new Dictionary<NetConnection, ServerMgr>();

            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            conf.Port = SharedDef.MASTER_SERVER_PORT;

#if DEBUG
            /*conf.SimulatedMinimumLatency = 0.2f; // 100ms
            conf.SimulatedRandomLatency = 0.05f; // +- 50ms*/

            conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);
#endif
#if VERBOSE
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
#endif

            server = new NetServer(conf);
            server.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 
            server.Start();
        }

        private void GotMessage(object peer)
        {
            server = peer as NetServer;
            NetIncomingMessage msg = server.ReadMessage();

            switch (msg.MessageType)
            {
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Logger.Debug(msg.ReadString());
                    break;
                case NetIncomingMessageType.UnconnectedData:
                    if ((PacketType)msg.ReadByte() == PacketType.AVAILABLE_TOURNAMENTS_REQUEST)
                        SendAvailableTournamentsResponse(msg.SenderEndPoint);
                    break;
                case NetIncomingMessageType.ConnectionApproval:
                    if (msg.ReadInt32() != (int)PacketType.PLAYER_CONNECT)
                        return;

                    Gametype type = (Gametype)msg.ReadByte();

                    if (connections.TryGetValue(msg.SenderConnection, out mgr))
                    {
                        mgr.Enqueue(new Action(() => mgr.PlayerConnectionApproval(msg)));
                        break;
                    }

                    if (!FindAvailableServerManager(msg.SenderConnection, type))
                    {
                        CreateNewServerMgr(type);
                    }

                    connections.Add(msg.SenderConnection, mgr);
                    mgr.Enqueue(new Action(() => mgr.PlayerConnectionApproval(msg)));
                    return; //musi se provest return, jinak je zprava zrecyklovana jeste pred zpracovanim, o recyklaci se stara ServerMgr
                case NetIncomingMessageType.Data:
                    if (!connections.TryGetValue(msg.SenderConnection, out mgr))
                    {
                        Logger.Warn("received data message from unknown connection -> skipped: " + msg.SenderConnection);
                        break;
                    }
                    mgr.EnqueueReceivedMessage(msg);
                    return; //musi se provest return, jinak je zprava zrecyklovana jeste pred zpracovanim, o recyklaci se stara ServerMgr
                case NetIncomingMessageType.StatusChanged:
                    switch (msg.SenderConnection.Status)
                    {
                        case NetConnectionStatus.None:
                        case NetConnectionStatus.InitiatedConnect:
                        case NetConnectionStatus.RespondedAwaitingApproval:
                        case NetConnectionStatus.RespondedConnect:
                            break;
                        case NetConnectionStatus.Disconnected:
                        case NetConnectionStatus.Disconnecting:
                            ServerMgr tempMgr = null;
                            if (!connections.TryGetValue(msg.SenderConnection, out tempMgr))
                            {
                                Logger.Warn("disconnecting unknown connection -> skipped: " + msg.SenderConnection);
                                break;
                            }
                            long uid = msg.SenderConnection.RemoteUniqueIdentifier;
                            tempMgr.Enqueue(new Action(() => tempMgr.PlayerDisconnected(uid)));
                            connections.Remove(msg.SenderConnection);
                            msg.SenderConnection.Disconnect("x");
                            break;
                        case NetConnectionStatus.Connected:
                            break;
                    }

                    // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                    Logger.Debug(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                    break;
                default:
                    Logger.Debug("non-handled message type: " + msg.MessageType);
                    break;
            }
            server.Recycle(msg);
        }

        private void SendAvailableTournamentsResponse(System.Net.IPEndPoint iPEndPoint)
        {
            List<TournamentSettings> available = GetAvailableTournaments();

            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write((byte)PacketType.AVAILABLE_TOURNAMENTS_RESPONSE);
            msg.Write(available.Count);
            foreach (TournamentSettings s in available)
                msg.Write(s);

            server.SendUnconnectedMessage(msg, iPEndPoint);
        }

        private List<TournamentSettings> GetAvailableTournaments()
        {
            List<TournamentSettings> available = new List<TournamentSettings>();
            foreach (KeyValuePair<NetConnection, ServerMgr> pair in connections)
                if (pair.Value.GameType == Gametype.TOURNAMENT_GAME)
                    available.Add(pair.Value.TournamentSettings);

            return available;
        }

        private void CreateNewServerMgr(Gametype type)
        {
            mgr = new ServerMgr();
            mgr.Init(type, server);

            //mgr.CloseCallback = new Action(() => ManagerClosed(mgr));

            Thread serverThread = new Thread(new ThreadStart(mgr.Run));
            serverThread.IsBackground = false;
            serverThread.Name = "Server Thread";
            serverThread.Start();

            switch (type)
            {
                case Gametype.SOLO_GAME:
                    // v solo se nastavi tournament settings primo pres herni aplikaci
                    break;
                case Gametype.MULTIPLAYER_GAME:
                    {
                        TournamentSettings s = new TournamentSettings();
                        s.MMType = MatchManagerType.QUICK_GAME;
                        s.Level = GameLevel.BASIC_MAP;
                        s.RoundCount = 1;
                        s.BotCount = 0;
                        mgr.Enqueue(new Action(() => mgr.TournamentSettings = s));
                    }
                    break;
                case Gametype.TOURNAMENT_GAME:
                    // v tournamentu se poslou setting pozdeji, az je hrac potvrdi
                    break;
            }
        }

        /*private void ManagerClosed(ServerMgr mgr)
        {
            List<NetConnection> connectionsForRemoval = new List<NetConnection>(2);

            foreach (KeyValuePair<NetConnection, ServerMgr> pair in connections)
                if (pair.Value == mgr)
                    connectionsForRemoval.Add(pair.Key);

            foreach (NetConnection con in connectionsForRemoval)
                connections.Remove(con);
        }*/

        private bool FindAvailableServerManager(NetConnection netConnection, Gametype type)
        {
            if (connections.Count == 0)
                return false;

            // vzdycky vytvorit novy manager pro solo hru
            if (type == Gametype.SOLO_GAME)
                return false;

            foreach (KeyValuePair<NetConnection, ServerMgr> pair in connections)
            {
                if (type == Gametype.MULTIPLAYER_GAME)
                {
                    Logger.Info("plr count: " + pair.Value.GetPlayerCount());
                    if (pair.Value.GameType == Gametype.MULTIPLAYER_GAME && pair.Value.GetPlayerCount() == 1)
                    {
                        mgr = pair.Value;
                        return true;
                    }
                }
                else // Tournament
                {
                    if (pair.Value.GameType == Gametype.TOURNAMENT_GAME && pair.Value.GetPlayerCount() < 6)
                    {
                        mgr = pair.Value;
                        return true;
                    }
                }
            }

            // nenalezen zadny vhodny manager
            return false;
        }

        public void Shutdown()
        {
            foreach (KeyValuePair<NetConnection, ServerMgr> pair in connections)
            {
                pair.Value.Enqueue(new Action(() => pair.Value.Shutdown()));
            }
            connections.Clear();
            server.Shutdown("exit");
            // bussy wait for shutdown
            while (server.Status != NetPeerStatus.NotRunning)
                Thread.Sleep(1);
        }
    }
}
