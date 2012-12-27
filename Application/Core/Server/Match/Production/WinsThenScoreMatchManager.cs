using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// kazdy hraje s kazdym a kdyz potom neni jednoznacny vyherce (treba dva maji dve vyhry), tak rozhodne score
    /// </summary>
    public class WinsThenScoreMatchManager : AbstractTournamentMultiplayerMatchManager
    {
        public static readonly MatchManagerInfo Info = new MatchManagerInfo(false, "Each vs. Each (most wins then score)");

        public WinsThenScoreMatchManager(List<Player> players, Random randGen, int rounds)
            : base(players, randGen, rounds)
        {
        }

        /// <summary>
        /// vraci bud viteze nebo null, pokud vitez jeste neni
        /// </summary>
        public override Player GetTournamentWinner()
        {
            // jeste nejsme v poslednim kole
            if (roundNumber < roundCount)
                return null;

            // pokud nehral kazdy s kazdym
            if (!EveryOnePlayedWithNumberOfPlayers(players.Count - 1))
                return null;

            int maxWins = 0;
            data.ForEach(d => { if (d.WonGames > maxWins) maxWins = d.WonGames; });

            if (maxWins == 0)
                return null;

            // hraci, kteri se deli o prvni misto ve vyhranych zapasech
            List<Player> bestPlayers = GetPlayersWhoWonNumberOfMatches(maxWins);
            if (bestPlayers.Count == 1)
                return bestPlayers[0];
            else
            {
                // kdyz jich je vic, tak vyhrava hrac s nejvetsim score
                IOrderedEnumerable<Player> bp = bestPlayers.OrderByDescending(p => p.Data.Score);
                return bp.First();
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
