using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// pro dva hrace - nikdy nezkonci a bude porad davat hry prvnim dvema hracum
    /// </summary>
    class OneToOneInfiniteMachMaker : ITournamentMatchMaker
    {
        private List<Player> players;

        public OneToOneInfiniteMachMaker(List<Player> players, Random randGen)
        {
            this.players = players;
        }

        public void SelectPlayersForNewMatch(out Player plr1, out Player plr2)
        {
            plr1 = players[0];
            plr2 = players[1];
        }

        public void MatchEnded(Player plr, GameEnd endType)
        {
            if (endType != GameEnd.WIN_GAME)
                return;

            plr.Data.WonMatches += 1;
        }

        public Player GetWinner()
        {
            return null;
        }
    }
}
