using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// kazdy hraje s kazdym tolikrat, dokud neni jednoznacny vyherce
    /// </summary>
    public class OneToAllTillWinnerMatchMaker : ITournamentMatchMaker
    {
        private List<Player> players;
        private List<PlayerMatchData> data;
        private Random rand;

        public OneToAllTillWinnerMatchMaker(List<Player> players, Random randGen)
        {
            this.players = players;
            data = new List<PlayerMatchData>();
            players.ForEach(p => data.Add(new PlayerMatchData(p.Data.HashId)));
            rand = randGen;
        }

        public void SelectPlayersForNewMatch(out Player plr1, out Player plr2)
        {
            for (int i = 1; i < players.Count + 10; ++i)
            {
                if (!EveryOnePlayedWithNumberOfPlayers(i))
                {
                    List<Player> foundPlayers = GetPlayersWhoPlayedWithLessThan(i);

                    plr1 = SelectRandomPlayerForMatch(foundPlayers);
                    foundPlayers.Remove(plr1);
                    plr2 = SelectRandomPlayerForMatchWithPlayer(foundPlayers, plr1);
                    return;
                }
            }

            plr1 = null;
            plr2 = null;
            throw new Exception("Shouldn't ever happen");
        }

        private void SetPlayingTogether(Player plr1, Player plr2)
        {
            PlayerMatchData d = GetPlayerMatchData(plr1);
            d.playedWith.Add(plr2.Data.HashId);
            d = GetPlayerMatchData(plr2);
            d.playedWith.Add(plr1.Data.HashId);
        }

        private Player SelectRandomPlayerForMatchWithPlayer(List<Player> foundPlayers, Player plr1)
        {
            // pokud je hracu lichy pocet a nezbyva nikdo, s kyma hrat, tak se vybira protihrac ze vsech hracu (jakoby zacatek dalsiho kola)
            if (foundPlayers.Count == 0)
            {
                foundPlayers = new List<Player>(players);
                foundPlayers.Remove(plr1);
            }

            return SelectRandomPlayerForMatch(foundPlayers);
        }

        private Player SelectRandomPlayerForMatch(List<Player> foundPlayers)
        {
            return foundPlayers[rand.Next(foundPlayers.Count)];
        }

        private List<Player> GetPlayersWhoPlayedWithLessThan(int playerCount)
        {
            List<Player> plrs = new List<Player>();

            data.ForEach(d => { if (d.playedWith.Count < playerCount) plrs.Add(players.Find(p => p.Data.HashId == d.Owner)); });

            return plrs;
        }

        private bool EveryOnePlayedWithNumberOfPlayers(int i)
        {
            return data.TrueForAll(p => p.playedWith.Count >= i);
        }

        public void MatchEnded(Player plr, GameEnd endType)
        {
            if (endType != GameEnd.WIN_GAME)
                return;

            SetPlayingTogether(plr, players.Find(p => p.IsActivePlayer() && p.Data.HashId != plr.Data.HashId));
            PlayerMatchData d = GetPlayerMatchData(plr);
            d.WonGames += 1;
            plr.Data.WonMatches = d.WonGames;
        }

        private PlayerMatchData GetPlayerMatchData(Player p)
        {
            return data.Find(d => d.Owner == p.Data.HashId);
        }

        /// <summary>
        /// vraci bud viteze nebo null, pokud vitez jeste neni
        /// </summary>
        public Player GetWinner()
        {
            if (!EveryOnePlayedWithNumberOfPlayers(players.Count - 1))
                return null;

            int maxWins = 0;
            data.ForEach(d => { if (d.WonGames > maxWins) maxWins = d.WonGames; });

            if (maxWins == 0)
                return null;

            // pokud neni stale neni jednoznacny vyherce
            int maxPlayed = 0;
            // najdeme kolik nejvickrat nekdo hral
            players.ForEach(p => { if (p.Data.PlayedMatches > maxPlayed) maxPlayed = p.Data.PlayedMatches; });
            // vsichni musi mit odehrano stejne her, aby se rozhodovalo o vitezi
            if (!EveryOnePlayedWithNumberOfPlayers(maxPlayed))
                return null;

            List<Player> bestPlayers = GetPlayersWhoWonNumberOfMatches(maxWins);
            if (bestPlayers.Count == 1)
                return bestPlayers[0];
            else
            {
                // pokud to je uz vic nez kazdy s kazdym a jeste se ctyrma
                if (maxPlayed >= players.Count + 4)
                {
                    // potom rozhoduje score - aby se nehralo do nekonecna
                    IOrderedEnumerable<Player> bp = bestPlayers.OrderByDescending(p => p.Data.Score);
                    return bp.First();
                }
                return null;
            }
        }

        private List<Player> GetPlayersWhoWonNumberOfMatches(int wins)
        {
            List<Player> bestPlayers = new List<Player>();

            // vrati vsechny hrace, kteri maji wins vyher
            data.ForEach(d => { if (d.WonGames == wins) bestPlayers.Add(players.Find(p => p.Data.HashId == d.Owner)); });

            return bestPlayers;
        }
    }
}
