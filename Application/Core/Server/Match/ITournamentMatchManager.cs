using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    public interface ITournamentMatchManager
    {
        Tuple<Player, Player> SelectPlayersForNewMatch();

        void OnMatchEnd(Player plr, GameEnd endType);

        Player GetWinner();

        bool HasRightNumberOfPlayersForStart();
    }

    public enum MatchManagerType
    {
        WINS_THEN_SCORE,
        ONLY_SCORE,
        SKIRMISH,
        QUICK_GAME,

        TEST_LEADER_SPECTATOR,
    }
}
