using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    public interface ITournamentMatchMaker
    {
        void SelectPlayersForNewMatch(out Player plr1, out Player plr2);

        void MatchEnded(Player plr, GameEnd endType);

        Player GetWinner();
    }
}
