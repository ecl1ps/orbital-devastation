using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    class FirstSpectatorMatchMaker : ITournamentMatchMaker
    {

        private List<Player> players;
        private Player spectator;

        public FirstSpectatorMatchMaker(List<Player> players)
        {
            this.players = players;
        }

        public void SelectPlayersForNewMatch(out Player plr1, out Player plr2)
        {
            int diff = players.Count > 2 ? 1 : 0;

            plr1 = players[diff];
            plr2 = players[diff + 1];

            if (diff > 0)
                spectator = players[0];
        }

        public void MatchEnded(Player plr, GameEnd endType)
        {
        }

        public Player GetWinner()
        {
            return spectator;
        }
    }
}
