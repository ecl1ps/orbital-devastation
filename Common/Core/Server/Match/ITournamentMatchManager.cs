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

        Player GetTournamentWinner();

        bool HasRightNumberOfPlayersForStart();

        void OnMatchEnd(Player plr, GameEnd endType, float time, ServerMgr server);

        void OnPlayerLeave(Player plr, bool gameRunning);

        void OnPlayerConnect(Player plr, bool gameRunning);
    }

    public enum MatchManagerType
    {
        ONLY_SCORE,
        SKIRMISH,
        QUICK_GAME,

        TEST_LEADER_SPECTATOR,
    }
}
