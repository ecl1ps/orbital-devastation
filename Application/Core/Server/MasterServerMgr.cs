using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

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
            conf.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            conf.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            conf.Port = SharedDef.PORT_NUMBER;

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

        public void GotMessage(object peer)
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
                /*case NetIncomingMessageType.DiscoveryRequest:
                    NetOutgoingMessage response = server.CreateMessage();
                    response.Write((byte)GameType);

                    // jmeno serveru
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        response.Write("Server hosted by " + App.Instance.PlayerName);
                    }));
                        
                    server.SendDiscoveryResponse(response, msg.SenderEndpoint);
                    break;*/
                // If incoming message is Request for connection approval
                // This is the very first packet/message that is sent from client
                case NetIncomingMessageType.ConnectionApproval:
                    if (msg.ReadInt32() != (int)PacketType.PLAYER_CONNECT)
                        return;

                    if (connections.TryGetValue(msg.SenderConnection, out mgr))
                    {
                        mgr.Enqueue(new Action(() => mgr.PlayerConnectionApproval(msg)));
                        break;
                    }

                    if (!FindAvailableServerManager(msg.SenderConnection))
                    {
                        CreateNewServerMgr();
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
                            // TODO:
                            if (!connections.TryGetValue(msg.SenderConnection, out mgr))
                            {
                                Logger.Warn("disconnecting unknown connection -> skipped: " + msg.SenderConnection);
                                break;
                            }
                            mgr.Enqueue(new Action(() => mgr.PlayerDisconnected(msg.SenderConnection.RemoteUniqueIdentifier)));
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

        private void CreateNewServerMgr()
        {
            mgr = new ServerMgr();
            // TODO
            mgr.Init(Gametype.SOLO_GAME, server);

            Thread serverThread = new Thread(new ThreadStart(mgr.Run));
            serverThread.IsBackground = false;
            serverThread.Name = "Server Thread";
            serverThread.Start();
        }

        private bool FindAvailableServerManager(NetConnection netConnection)
        {
            // TODO
            if (connections.Count == 0)
                return false;

            foreach (KeyValuePair<NetConnection, ServerMgr> pair in connections)
            {
                mgr = pair.Value;
                return true;
            }

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
        }
    }
}
