using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows;
using Lidgren.Network;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Helpers;

namespace Orbit.Core.Server.Match
{
    public abstract class AbstractTournamentMatchManager : ITournamentMatchManager
    {
        protected List<Player> players;
        protected List<PlayerMatchData> data;
        protected Random rand;
        protected int roundCount;
        protected int roundNumber;

        public AbstractTournamentMatchManager(List<Player> players, Random randGen, int rounds)
        {
            this.players = players;
            data = new List<PlayerMatchData>();
            players.ForEach(p => data.Add(new PlayerMatchData(p.Data.HashId)));
            rand = randGen;
            roundCount = rounds;
            roundNumber = 1;
        }


        public abstract Tuple<Player, Player> SelectPlayersForNewMatch();

        public abstract Player GetTournamentWinner();

        public abstract bool HasRightNumberOfPlayersForStart();


        public virtual void OnMatchEnd(Player plr, GameEnd endType, float time, ServerMgr server)
        {
            if (endType != GameEnd.WIN_GAME)
                return;

            Player looser = players.Find(p => p.IsActivePlayer() && p.Data.HashId != plr.Data.HashId);
            SetPlayedTogether(plr, looser);
            PlayerMatchData d = GetPlayerMatchData(plr);
            d.WonGames += 1;
            plr.Data.WonMatches = d.WonGames;

            int scoreWin = SharedDef.VICTORY_SCORE_BONUS;
            int scoreLoose = SharedDef.LOOSE_SCORE_BONUS;

            plr.Data.Score += scoreWin;
            looser.Data.Score += scoreLoose;

            SendPlayerScore(plr, server);
            SendPlayerScore(looser, server);

            SendTextMessage(plr, "You are victorious. For your brave victory you earned " + scoreWin + " score", server);
            SendTextMessage(looser, "You lost. For your fierce defense you get " + scoreLoose + " score", server);
        }

        protected void SetPlayedTogether(Player plr1, Player plr2)
        {
            PlayerMatchData d = GetPlayerMatchData(plr1);
            d.playedWith.Add(plr2.Data.HashId);
            plr1.Data.PlayedMatches += 1;
            d = GetPlayerMatchData(plr2);
            d.playedWith.Add(plr1.Data.HashId);
            plr2.Data.PlayedMatches += 1;
        }

        protected PlayerMatchData GetPlayerMatchData(Player p)
        {
            return data.Find(d => d.Owner == p.Data.HashId);
        }

        protected Player SelectRandomPlayerForMatchWithPlayer(List<Player> foundPlayers, Player plr1)
        {
            // pokud je hracu lichy pocet a nezbyva nikdo, s kyma hrat, tak se vybira protihrac ze vsech hracu (jakoby zacatek dalsiho kola)
            if (foundPlayers.Count == 0)
            {
                foundPlayers = new List<Player>();
                data.ForEach(d => { if (d.Owner != plr1.Data.HashId && d.IsOnline) foundPlayers.Add(players.Find(p => p.Data.HashId == d.Owner)); });
            }

            return SelectRandomPlayerForMatch(foundPlayers);
        }

        protected Player SelectRandomPlayerForMatch(List<Player> foundPlayers)
        {
            if (foundPlayers.Count == 0)
                return null;

            return foundPlayers[rand.Next(foundPlayers.Count)];
        }

        protected List<Player> GetPlayersWhoPlayedWithLessThan(int playerCount)
        {
            List<Player> plrs = new List<Player>();

            data.ForEach(d => { if (d.playedWith.Count < playerCount && d.IsOnline) plrs.Add(players.Find(p => p.Data.HashId == d.Owner)); });

            return plrs;
        }

        protected bool EveryOnePlayedWithNumberOfPlayers(int i)
        {
            return data.TrueForAll(p => p.playedWith.Count >= i || !p.IsOnline);
        }

        protected Tuple<Player, Player> SelectPlayersBasic()
        {
            // zkousime od jedne do poctu hracu, jestli uz vsichni hrali s timto poctem hracu
            for (int i = 1; i < players.Count; ++i)
            {
                if (!EveryOnePlayedWithNumberOfPlayers(i))
                {
                    // pokud existuje nekdo, kdo nehral s tolika hraci
                    List<Player> foundPlayers = GetPlayersWhoPlayedWithLessThan(i);

                    // tak vybereme nahodne nekoho z nich
                    Player plr1 = SelectRandomPlayerForMatch(foundPlayers);
                    foundPlayers.Remove(plr1);
                    // a najdeme mu spoluhrace (pokud existuje nekdo, kdo take nehral tolikrat, tak ma prednost, jinak nahodne kdokoli jiny)
                    Player plr2 = SelectRandomPlayerForMatchWithPlayer(foundPlayers, plr1);
                    plr1.Data.Active = true;
                    if (plr2 != null)
                        plr2.Data.Active = true;
                    return new Tuple<Player, Player>(plr1, plr2);
                }
            }
            return null;
        }

        protected void SetAllPlayersInactive()
        {
            foreach (Player p in players)
                p.Data.Active = false;
        }

        protected void SendTextMessage(Player p, String text, ServerMgr server)
        {
            NetOutgoingMessage msg = server.CreateNetMessage();
            msg.Write((int)PacketType.SHOW_ALLERT_MESSAGE);
            msg.Write(text);
            msg.Write(AlertMessageManager.TIME_NORMAL);

            server.SendMessage(msg, p);
        }

        protected void SendPlayerScore(Player p, ServerMgr server)
        {
            NetOutgoingMessage msg = server.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_SCORE_UPDATE);
            msg.Write(p.Data.Score);

            server.SendMessage(msg, p);
        }

        public virtual void OnPlayerLeave(Player plr, bool gameRunning)
        {
        }

        public virtual void OnPlayerConnect(Player plr, bool gameRunning)
        {
        }

        public virtual void AssignPlayersToSpectators(Tuple<Player, Player> active, List<Player> spectators)
        {
        }
    }
}
