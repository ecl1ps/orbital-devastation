using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Server.Match;
using Orbit.Core.Server;

namespace Orbit.Core
{
    public class TournamentSettings
    {

        public GameMatchMakerType MMType { get; set; }
        public GameLevel Level { get; set; }
        public int RoundCount { get; set; }

        public TournamentSettings(bool withDefaults = false)
        {
            if (!withDefaults)
                return;

            MMType = GameMatchMakerType.WINS_THEN_SCORE;
            Level = GameLevel.NORMAL1;
            RoundCount = 1;
        }
    }
}
