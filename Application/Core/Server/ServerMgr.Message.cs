using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Orbit.Core.Players;
using Orbit.Core.Helpers;
using Orbit.Core.Server.Match;
using System.Windows.Media;

namespace Orbit.Core.Server
{
    public partial class ServerMgr
    {
        private void PlayerConnectionApproval(NetIncomingMessage msg)
        {
            Logger.Debug("Incoming LOGIN");

            // nepridavat hrace, pokud uz existuje
            if (players.Exists(plr => plr.Connection == null || plr.Connection.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier))
                return;

            // don't allow spectators to join quick game
            if (GameType == Gametype.MULTIPLAYER_GAME && players.Count >= 2)
                return;

            string plrName = msg.ReadString();
            string plrHash = msg.ReadString();
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
            players.ForEach(p => { if (p.IsOnlineOrBot()) onlinePlayers++; });
            outmsg.Write(onlinePlayers);

            foreach (Player plr in players)
            {
                if (!plr.IsOnlineOrBot())
                    continue;

                outmsg.Write(plr.GetId());
                plr.Data.WriteObject(outmsg);
            }

            return outmsg;
        }


        private NetOutgoingMessage CreateTournamentSettingsMessage()
        {
            NetOutgoingMessage outmsg = CreateNetMessage();
            outmsg.Write((int)PacketType.TOURNAMENT_SETTINGS);
            outmsg.Write(TournamentSettings);
            return outmsg;
        }

        private void ReceivedScoreQueryResponseMsg(NetIncomingMessage msg)
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

        private void SkipWaitingForScoreQueryResponse()
        {
            if (savedEndGameAction != null)
            {
                // EndGame() s hracem, ktery vyhral
                savedEndGameAction.Invoke();
                savedEndGameAction = null;
            }
        }

        private void ReceivedPlayerReadyMsg(NetIncomingMessage msg)
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

        private void ReceivedStartGameRequestMsg(NetIncomingMessage msg)
        {
            if (gameSession == null)
            {
                gameSession = new GameManager(this, players);
                StateMgr.AddGameState(gameSession);
            }

            if (gameSession.CheckTournamentFinished())
                return;

            gameSession.CreateNewMatch();
            isInitialized = true;

            gameSession.RequestStartMatch(GetPlayer(msg.SenderConnection));
        }

        private void ReceivedPlayerScoreAndGoldMsg(NetIncomingMessage msg)
        {
            // neprijimat score a goldy po ukonceni hry (premazalo by to uz zresetovana PlayerData)
            if (!isInitialized)
                return;

            Player p = GetPlayer(msg.ReadInt32());
            p.Data.Score = msg.ReadInt32();
            p.Data.Gold = msg.ReadInt32();
            ForwardMessage(msg);
        }

        private void ReceivedPlayerDisconnectedMsg(NetIncomingMessage msg)
        {
            Player disconnected = GetPlayer(msg.ReadInt32());
            if (disconnected == null)
                return;
            disconnected.Data.StartReady = false;
            if (gameSession != null)
                gameSession.PlayerLeft(disconnected);
            ForwardMessage(msg);
        }

        private void ReceivedTournamentSettingsMsg(NetIncomingMessage msg)
        {
            players.ForEach(p => { if (!p.Data.LobbyLeader) p.Data.LobbyReady = false; });
            TournamentSettings = msg.ReadTournamentSettings();
        }
    }
}
