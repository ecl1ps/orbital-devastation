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
                plr.Statistics.WriteObject(outmsg);
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
            if (savedEndGameAction == null)
                return;

            Player p = GetPlayer(msg.ReadInt32());
            p.Data.MatchPoints = msg.ReadInt32();
            p.Statistics.ReadObject(msg);

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
            p.Data.LobbyReady = msg.ReadBoolean();
            p.Data.LobbyLeader = msg.ReadBoolean();

            // vytvorit novou zpravu, protoze id jeste nemuselo byt ziniciovano
            NetOutgoingMessage rdyMsg = CreateNetMessage();
            rdyMsg.Write((int)PacketType.PLAYER_READY);
            rdyMsg.Write(p.GetId());
            rdyMsg.Write(p.Data.LobbyReady);
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
            p.Data.MatchPoints = msg.ReadInt32();
            p.Data.Gold = msg.ReadInt32();
            ForwardMessage(msg);
        }

        private void ReceivedTournamentSettingsMsg(NetIncomingMessage msg)
        {
            players.ForEach(p => { if (!p.Data.LobbyLeader) p.Data.LobbyReady = false; });
            TournamentSettings = msg.ReadTournamentSettings();
        }
    }
}
