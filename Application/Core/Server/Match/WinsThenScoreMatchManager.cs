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
    public class WinsThenScoreMatchManager : AbstractTournamentMatchManager
    {
        public WinsThenScoreMatchManager(List<Player> players, Random randGen, int rounds)
            : base(players, randGen, rounds)
        {
        }

        public override Tuple<Player, Player> SelectPlayersForNewMatch()
        {
            // zneaktivnime vsechny hrace
            foreach (Player p in players)
                p.Data.Active = false;

            // zkusime najit hrace beznou cestou
            Tuple<Player, Player> newPlayes = SelectPlayersBasic();
            if (newPlayes != null)
                return newPlayes;

            // zde uz kazdy odehral s kazdym

            // skocime na dalsi kolo
            if (roundNumber++ == roundCount)
                throw new Exception("Error: players are trying to start round " + roundNumber + " but tournament is set for " + roundCount + "rounds!");

            // a vynulujeme zaznamy kdo hral s kym
            data.ForEach(d => d.playedWith.Clear());

            // a hledame hrace znovu pro dalsi kolo
            return SelectPlayersForNewMatch();
        }

        /// <summary>
        /// vraci bud viteze nebo null, pokud vitez jeste neni
        /// </summary>
        public override Player GetWinner()
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
