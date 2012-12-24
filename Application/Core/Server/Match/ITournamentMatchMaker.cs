using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    public interface ITournamentMatchMaker
    {
        Tuple<Player, Player> SelectPlayersForNewMatch();

        void OnMatchEnd(Player plr, GameEnd endType);

        Player GetWinner();
    }

    public enum GameMatchMakerType
    {
        WINS_THEN_SCORE,
        ONLY_SCORE,
        TEST_LEADER_SPECTATOR,
    }
}
