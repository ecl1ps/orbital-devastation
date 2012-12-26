using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// kazdy hraje s kazdym a potom rozhoduje pouze score
    /// </summary>
    public class ScoreMatchManager : AbstractTournamentMatchManager
    {
        public static readonly MatchManagerInfo Info = new MatchManagerInfo(false, "Each vs. Each (score)");

        public ScoreMatchManager(List<Player> players, Random randGen, int rounds)
            : base(players, randGen, rounds)
        {
        }

        public override Tuple<Player, Player> SelectPlayersForNewMatch()
        {
            // zneaktivnime vsechny hrace
            SetAllPlayersInactive();

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
        public override Player GetTournamentWinner()
        {
            // jeste nejsme v poslednim kole
            if (roundNumber < roundCount)
                return null;

            // pokud nehral kazdy s kazdym
            if (!EveryOnePlayedWithNumberOfPlayers(players.Count - 1))
                return null;

            int maxScore = 0;
            players.ForEach(p => { if (p.Data.Score > maxScore) maxScore = p.Data.Score; });

            // hraci, kteri se deli o prvni misto ve score
            List<Player> bestPlayers = GetPlayersWhoHaveScore(maxScore);
            if (bestPlayers.Count == 1)
                return bestPlayers[0];
            else
            {
                // kdyz jich je vic, tak vyhrava hrac s nejvice vyhranymi hrami
                IOrderedEnumerable<Player> bp = bestPlayers.OrderByDescending(p => p.Data.WonMatches);
                return bp.First();
            }
        }

        private List<Player> GetPlayersWhoHaveScore(int score)
        {
            List<Player> bestPlayers = new List<Player>();

            // vrati vsechny hrace, kteri maji dane score
            players.ForEach(p => { if (p.Data.Score == score) bestPlayers.Add(p); });

            return bestPlayers;
        }

        public override bool HasRightNumberOfPlayersForStart()
        {
            return players.FindAll(p => p.Data.PlayerType == PlayerType.HUMAN).Count >= 2;
        }
    }
}
