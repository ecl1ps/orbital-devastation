using System;
using System.Linq;
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
using System.Collections;

namespace Orbit.Core.Server
{
    public partial class ServerMgr
    {
        private NetServer server;
        private Queue dataMessages = Queue.Synchronized(new Queue());
        private List<NetConnection> activeConnections = new List<NetConnection>(2);

        public void PlayerConnectionApproval(NetIncomingMessage msg, string plrName, string plrHash)
        {
            // nepridavat hrace, pokud uz existuje
            if (players.Exists(plr => plr.Connection == null || plr.Connection.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier))
                return;

            // don't allow spectators to join quick game
            if (GameType == Gametype.MULTIPLAYER_GAME && players.Count >= 2)
                return;

            Color plrColor = msg.ReadColor();

            // nepridavat ani hrace ze stejne instance hry (nejde je potom spolehlive rozlisit v tournamentu)
            Player p = players.Find(plr => plr.Data.HashId == plrHash);
            if (p == null)
                p = CreateAndAddPlayer(plrName, plrHash, plrColor);
            //player je connected kdyz se snazi pripojit
            //else if (p.IsOnlineOrBot())
            // return;

            if (gameSession != null)
                gameSession.PlayerConnected(p);

            p.Connection = msg.SenderConnection;
            activeConnections.Add(msg.SenderConnection);

            NetOutgoingMessage hailMsg = CreateNetMessage();
            hailMsg.Write((int)PacketType.PLAYER_ID_HAIL);
            hailMsg.Write(p.Data.Id);
            hailMsg.Write(p.Data.Name);
            bool gameRunning = gameSession != null && gameSession.IsRunning;
            hailMsg.Write(gameRunning);

            // Approve clients connection (Its sort of agreenment. "You can be my client and i will host you")
            msg.SenderConnection.Approve(hailMsg);
            server.Recycle(msg);

            // jakmile potvrdime spojeni nejakeho hrace, tak hned zesynchronizujeme data hracu mezi vsemi hraci
            NetOutgoingMessage plrs = CreateAllPlayersDataMessage();
            BroadcastMessage(plrs);
        }

        public void PlayerDisconnected(long netConnectionId)
        {
            Player disconnected = GetPlayer(netConnectionId);
            Disconnected(disconnected);
            SendPlayerLeftMessage(disconnected);
        }

        private void ReceivedPlayerDisconnectedMsg(NetIncomingMessage msg)
        {
            Player disconnected = GetPlayer(msg.ReadInt32());
            Disconnected(disconnected);
            ForwardMessage(msg);
        }

        private void Disconnected(Player disconnected)
        {
            if (disconnected == null)
                return;

            disconnected.Data.StartReady = false;
            activeConnections.Remove(disconnected.Connection);

            if (disconnected.Data.LobbyLeader)
                AssignNewLobbyLeader(disconnected);

            if (gameSession != null)
                gameSession.PlayerLeft(disconnected);

            if (disconnected.IsActivePlayer())
                EndGame(disconnected, GameEnd.LEFT_GAME);

            if (activeConnections.Count == 0)
                Shutdown();
        }

        private void AssignNewLobbyLeader(Player leaver)
        {
            try
            {
                Player leader = players.First<Player>(newLeader => newLeader.GetId() != leaver.GetId());

                leaver.Data.LobbyLeader = false;

                if (leader == null)
                    return;

                leader.Data.LobbyLeader = true;
                BroadcastMessage(CreateAllPlayersDataMessage());
            }
            catch (InvalidOperationException) { }
        }

        private void ReceivedPlayerKickRequest(int id)
        {
            Player p = GetPlayer(id);
            if (p != null)
            {
                if (p.Connection != null)
                {
                    p.Connection.Disconnect("kicked");
                    activeConnections.Remove(p.Connection);
                }
                else
                {
                    players.Remove(p); // bot
                    SendPlayerLeftMessage(p);
                }
            }
        }

        public void EnqueueReceivedMessage(NetIncomingMessage msg)
        {
            dataMessages.Enqueue(msg);
        }

        private void ProcessMessages()
        {
            while (dataMessages.Count != 0)
                ProcessIncomingDataMessage(dataMessages.Dequeue() as NetIncomingMessage);
        }

        public void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
#if VERBOSE
            Logger.Debug("Server " + GetPlayer(msg.SenderConnection.RemoteUniqueIdentifier).GetId() + ": received msg " + type.ToString());
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

            server.Recycle(msg);
        }

        private void SyncReceivedObject(ISceneObject o, NetIncomingMessage msg)
        {
            o.Update(msg.SenderConnection.AverageRoundtripTime / 2);
        }

        public NetOutgoingMessage CreateNetMessage()
        {
            return server.CreateMessage();
        }

        public void BroadcastMessage(NetOutgoingMessage msg, NetConnection except)
        {
            if (activeConnections.Count > 1)
                server.SendMessage(msg, activeConnections, NetDeliveryMethod.ReliableOrdered, 0, except);
        }

        public void BroadcastMessage(NetOutgoingMessage msg, Player except)
        {
            if (activeConnections.Count > 1)
                server.SendMessage(msg, activeConnections, NetDeliveryMethod.ReliableOrdered, 0, except.Connection);
        }

        public void BroadcastMessage(NetOutgoingMessage msg)
        {
            if (activeConnections.Count > 0)
                server.SendMessage(msg, activeConnections, NetDeliveryMethod.ReliableOrdered, 0);
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
