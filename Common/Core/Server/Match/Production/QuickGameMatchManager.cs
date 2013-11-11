using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// quick game - 2 hraci
    /// </summary>
    public class QuickGameMatchManager : SkirmishMatchManager
    {
        public static new MatchManagerInfo Info = new MatchManagerInfo(true, Strings.mm_type_two_players);

        public QuickGameMatchManager(List<Player> players, Random randGen, int rounds)
            : base(players, randGen, rounds)
        {
        }

        public override bool HasRightNumberOfPlayersForStart()
        {
            return players.FindAll(p => p.Data.PlayerType == PlayerType.HUMAN).Count == 2;
        }
    }
}
