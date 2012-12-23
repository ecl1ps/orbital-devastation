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

    public enum GameMatchMakerType
    {
        ONE_TO_ALL_THEN_SCORE,
        ONE_TO_ALL_TILL_WINNER,
        ONE_TO_ONE_INFINITE,
#if DEBUG
        FIRST_SPECTATOR,
#endif
    }
}
