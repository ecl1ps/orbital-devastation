using System;
using Orbit;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;
using Orbit.Core.Scene;
using Orbit.Core;
using Orbit.Core.Helpers;
using Orbit.Core.Server.Match;

namespace Orbit.Core.Server
{
    public partial class ServerMgr
    {
        private NetServer server;

        private void InitNetwork()
        {
            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");
            conf.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            conf.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            conf.Port = SharedDef.PORT_NUMBER;

#if DEBUG
            conf.SimulatedMinimumLatency = 0.2f; // 100ms
            conf.SimulatedRandomLatency = 0.05f; // +- 50ms
#endif

            // debug only
            conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);

            server = new NetServer(conf);
            server.Start();
        }

        private void ProcessMessages()
        {
            if (server == null || server.Status != NetPeerStatus.Running)
                return;

            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write((byte)GameType);

                        // jmeno serveru
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            response.Write((Application.Current as App).PlayerName);
                        }));
                        
                        server.SendDiscoveryResponse(response, msg.SenderEndpoint);
                        break;
                    // If incoming message is Request for connection approval
                    // This is the very first packet/message that is sent from client
                    case NetIncomingMessageType.ConnectionApproval:
                        if (msg.ReadInt32() == (int)PacketType.PLAYER_CONNECT)
                        {
                            Console.WriteLine("Incoming LOGIN");

                            // nepridavat hrace, pokud uz existuje
                            if (players.Exists(plr => plr.Connection.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier))
                                return;

                            string plrName = msg.ReadString();
                            string plrHash = msg.ReadString();

                            // nepridavat ani hrace ze stejne instance hry (nejde je potom spolehlive rozlisit v tournamentu)
                            Player p = players.Find(plr => plr.Data.HashId == plrHash);
                            if (p == null)
                                p = CreateAndAddPlayer(plrName, plrHash);
                            else if (p.IsOnlineAndOrBot())
                                return;

                            p.Connection = msg.SenderConnection;

                            NetOutgoingMessage hailMsg = CreateNetMessage();
                            hailMsg.Write((int)PacketType.PLAYER_ID_HAIL);
                            hailMsg.Write(p.Data.Id);
                            hailMsg.Write(p.Data.Name);
                            hailMsg.Write((byte)GameType);
                            bool tournamentRunning = GameType == Gametype.TOURNAMENT_GAME && gameSession != null && gameSession.IsRunning;
                            hailMsg.Write(tournamentRunning);

                            // Approve clients connection (Its sort of agreenment. "You can be my client and i will host you")
                            msg.SenderConnection.Approve(hailMsg);

                            // jakmile potvrdime spojeni nejakeho hrace, tak hned zesynchronizujeme data hracu mezi vsemi hraci
                            NetOutgoingMessage plrs = CreateAllPlayersDataMessage();
                            BroadcastMessage(plrs);
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        ProcessIncomingDataMessage(msg);
                        break;
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
                                Player disconnected = GetPlayer(msg.SenderConnection);
                                if (disconnected == null)
                                    return;
                                //players.Remove(disconnected);
                                SendPlayerLeftMessage(disconnected);
                                if (disconnected.IsActivePlayer())
                                    EndGame(disconnected, GameEnd.LEFT_GAME);
                                break;
                            case NetConnectionStatus.Connected:
                                break;
                        }

                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        Console.WriteLine(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                        break;
                    default:
                        Console.WriteLine("Unhandled message type: " + msg.MessageType);
                        break;
                }
                server.Recycle(msg);
            }
        }

        private void SendPlayerLeftMessage(Player p)
        {
            NetOutgoingMessage outMsg = CreateNetMessage();
            outMsg.Write((int)PacketType.PLAYER_DISCONNECTED);
            outMsg.Write(p.GetId());
            BroadcastMessage(outMsg);
        }

        public NetOutgoingMessage CreateAllPlayersDataMessage()
        {
            // data vsech hracu
            NetOutgoingMessage outmsg = CreateNetMessage();

            outmsg.Write((int)PacketType.ALL_PLAYER_DATA);
            int onlinePlayers = 0;
            players.ForEach(p => { if (p.IsOnlineAndOrBot()) onlinePlayers++; });
            outmsg.Write(onlinePlayers);

            foreach (Player plr in players)
            {
                if (!plr.IsOnlineAndOrBot())
                    continue;

                outmsg.Write(plr.GetId());
                outmsg.WriteObjectPlayerData(plr.Data);
            }

            return outmsg;
        }

        private void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
            Console.WriteLine("Server " + GetPlayer(msg.SenderConnection).GetId() + ": received msg " + type.ToString());
            switch (type)
            {
                case PacketType.ALL_PLAYER_DATA:
                case PacketType.ALL_ASTEROIDS:
                case PacketType.NEW_ASTEROID:
                case PacketType.NEW_SINGULARITY_MINE:
                case PacketType.NEW_SINGULARITY_BULLET:
                case PacketType.NEW_HOOK:
                case PacketType.SINGULARITY_MINE_HIT:
                case PacketType.CHAT_MESSAGE:
                case PacketType.HOOK_HIT:
                case PacketType.BULLET_HIT:
                    ForwardMessage(msg);
                    break;
                case PacketType.START_GAME_REQUEST:

                    if (GameType != Gametype.SOLO_GAME && players.Count < 2)
                        break;

                    if (gameSession == null)
                        gameSession = new GameManager(this, players);

                    if (gameSession.CheckTournamentFinished())
                        break;

                    gameSession.CreateNewMatch();
                    isInitialized = true;

                    gameSession.RequestStartMatch(GetPlayer(msg.SenderConnection));

                    break;
                case PacketType.PLAYER_HEAL:
                case PacketType.BASE_INTEGRITY_CHANGE:
                    // neprijimat opozdene packety s ubranim zivotu
                    if (!isInitialized)
                        return;
                    GetPlayer(msg.ReadInt32()).Data.BaseIntegrity = msg.ReadInt32();
                    ForwardMessage(msg);
                    break;
                case PacketType.ASTEROID_DESTROYED:
                    gameSession.ObjectDestroyed(msg.ReadInt64());
                    break;
                case PacketType.PLAYER_READY:
                    {
                        Player p = GetPlayer(msg.SenderConnection);
                        msg.ReadInt32(); // Id
                        p.Data.LobbyReady = true;
                        p.Data.LobbyLeader = msg.ReadBoolean();

                        // vytvorit novou zpravu, protoze id jeste nemuselo byt ziniciovano
                        NetOutgoingMessage rdyMsg = CreateNetMessage();
                        rdyMsg.Write((int)PacketType.PLAYER_READY);
                        rdyMsg.Write(p.GetId());
                        rdyMsg.Write(p.Data.LobbyLeader);
                        BroadcastMessage(rdyMsg);
                    }
                    break;
                case PacketType.ALL_PLAYER_DATA_REQUEST:
                    NetOutgoingMessage plrs = CreateAllPlayersDataMessage();
                    SendMessage(plrs, msg.SenderConnection);
                    break;
                case PacketType.PLAYER_SCORE:
                    GetPlayer(msg.ReadInt32()).Data.Score = msg.ReadInt32();
                    ForwardMessage(msg);
                    break;
                case PacketType.SCORE_QUERY_RESPONSE:
                    {
                        Player p = GetPlayer(msg.ReadInt32());
                        p.Data.Score = msg.ReadInt32();

                        if (!playersRespondedScore.Contains(p.GetId()))
                            playersRespondedScore.Add(p.GetId());

                        if (playersRespondedScore.Count >= (players.Count > 1 ? 2 : players.Count))
                        {
                            // EndGame() s hracem, ktery vyhral
                            savedEndGameAction.Invoke();
                            savedEndGameAction = null;
                        }
                    }
                    break;
            }
        }

        private void SyncReceivedObject(ISceneObject o, NetIncomingMessage msg)
        {
            o.Update(msg.SenderConnection.AverageRoundtripTime / 2);
        }

        public bool IsServer()
        {
            return true;
        }

        public NetOutgoingMessage CreateNetMessage()
        {
            return server.CreateMessage();
        }

        public void BroadcastMessage(NetOutgoingMessage msg, NetConnection except)
        {
            server.SendToAll(msg, except, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void BroadcastMessage(NetOutgoingMessage msg, Player except)
        {
            server.SendToAll(msg, except.Connection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void BroadcastMessage(NetOutgoingMessage msg)
        {
            server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        private void BroadcastMessage(NetIncomingMessage msg)
        {
            NetOutgoingMessage outMsg = CreateNetMessage();
            outMsg.Write(msg);
            BroadcastMessage(outMsg);
        }

        private void ForwardMessage(NetIncomingMessage msg)
        {
            NetOutgoingMessage outMsg = CreateNetMessage();
            outMsg.Write(msg);
            BroadcastMessage(outMsg, msg.SenderConnection);
        }

        public void SendMessage(NetOutgoingMessage msg, Player p)
        {
            SendMessage(msg, p.Connection);
        }

        public void SendMessage(NetOutgoingMessage msg, NetConnection con)
        {
            server.SendMessage(msg, con, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
