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
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;
using Orbit.Core.Scene;
using Orbit.Core;
using Orbit.Core.Helpers;
using Orbit.Core.Server.Match;
using System.Net;

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
            conf.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
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
            server.Start();
            RegisterOnMasterServer();
        }

        public void RegisterOnMasterServer()
        {
            NetOutgoingMessage regMsg = server.CreateMessage();
            regMsg.Write((byte)MasterServerPacketType.REGISTER_TOURNAMENT);
            IPAddress mask;
            IPAddress adr = NetUtility.GetMyAddress(out mask);
            regMsg.Write(new IPEndPoint(adr, SharedDef.PORT_NUMBER));

            Logger.Info("Sending registration to master server");
            server.SendUnconnectedMessage(regMsg, NetUtility.Resolve(SharedDef.MASTER_SERVER_ADDRESS, SharedDef.MASTER_SERVER_PORT_NUMBER));
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
                    case NetIncomingMessageType.NatIntroductionSuccess:
                        string token = msg.ReadString();
                        Logger.Info("Nat introduction success to " + msg.SenderEndpoint + " token is: " + token);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Logger.Debug(msg.ReadString());
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage response = server.CreateMessage();

                        // jmeno serveru
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            response.Write("Server hosted by " + App.Instance.PlayerName);
                        }));
                        
                        server.SendDiscoveryResponse(response, msg.SenderEndpoint);
                        break;
                    // If incoming message is Request for connection approval
                    // This is the very first packet/message that is sent from client
                    case NetIncomingMessageType.ConnectionApproval:
                        if (msg.ReadInt32() == (int)PacketType.PLAYER_CONNECT)
                            PlayerConnectionApproval(msg);
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
                                if (gameSession != null)
                                    gameSession.PlayerLeft(disconnected);
                                disconnected.Data.StartReady = false;
                                SendPlayerLeftMessage(disconnected);
                                if (disconnected.IsActivePlayer())
                                    EndGame(disconnected, GameEnd.LEFT_GAME);
                                break;
                            case NetConnectionStatus.Connected:
                                break;
                        }

                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        Logger.Debug(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                        break;
                    default:
                        Logger.Debug("Unhandled message type: " + msg.MessageType);
                        break;
                }
                server.Recycle(msg);
            }
        }

        private void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
#if VERBOSE
            Logger.Debug("Server " + GetPlayer(msg.SenderConnection).GetId() + ": received msg " + type.ToString());
#endif
            switch (type)
            {
                case PacketType.START_GAME_REQUEST:
                    ReceivedStartGameRequestMsg(msg);
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
                    ReceivedPlayerReadyMsg(msg);
                    break;
                case PacketType.ALL_PLAYER_DATA_REQUEST:
                    NetOutgoingMessage plrs = CreateAllPlayersDataMessage();
                    SendMessage(plrs, msg.SenderConnection);
                    break;
                case PacketType.TOURNAMENT_SETTINGS:
                    ReceivedTournamentSettingsMsg(msg);
                    ForwardMessage(msg);
                    break;
                case PacketType.TOURNAMENT_SETTINGS_REQUEST:
                    if (TournamentSettings == null)
                        return;
                    NetOutgoingMessage settings = CreateTournamentSettingsMessage();
                    SendMessage(settings, msg.SenderConnection);
                    break;
                case PacketType.PLAYER_SCORE_AND_GOLD:
                    ReceivedPlayerScoreAndGoldMsg(msg);
                    break;
                case PacketType.SCORE_QUERY_RESPONSE:
                    ReceivedScoreQueryResponseMsg(msg);
                    break;
                case PacketType.PLAYER_RECEIVED_POWERUP:
                    statsMgr.AddStatToPlayer(GetPlayer(msg.ReadInt32()).Data, (PlayerStats)msg.ReadByte(), msg.ReadFloat());
                    ForwardMessage(msg);
                    break;
                case PacketType.PLAYER_DISCONNECTED:
                    ReceivedPlayerDisconnectedMsg(msg);
                    break;
                case PacketType.PLAYER_COLOR_CHANGED:
                    GetPlayer(msg.ReadInt32()).Data.PlayerColor = msg.ReadColor();
                    ForwardMessage(msg);
                    break;
                case PacketType.PLAYER_KICK_REQUEST:
                    ReceivedPlayerKickRequest(msg.ReadInt32());
                    break;
                default:
                    ForwardMessage(msg);
                    break;
            }
        }

        private void ReceivedPlayerKickRequest(int id)
        {
            Player p = GetPlayer(id);
            if (p != null)
                p.Connection.Disconnect("kicked");
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
            if (con == null)
                return;

            server.SendMessage(msg, con, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
