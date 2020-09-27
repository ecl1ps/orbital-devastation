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
    public class ScoreMatchManager : AbstractTournamentMultiplayerMatchManager
    {
        public static MatchManagerInfo Info = new MatchManagerInfo(false, Strings.mm_type_competition, 2);

        public ScoreMatchManager(List<Player> players, Random randGen, int rounds)
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
    }
}
