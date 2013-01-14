using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Server.Match;
using Orbit.Core.Server;
using Orbit.Core.Server.Level;
using Orbit.Core.Players;

namespace Orbit.Core
{
    public class TournamentSettings
    {
        public MatchManagerType MMType { get; set; }
        public GameLevel Level { get; set; }
        public int RoundCount { get; set; }
        public BotType BotType { get; set; }
        public int BotCount { get; set; }
        public int PlayedMatches { get; set; }

        public TournamentSettings(bool withDefaults = false)
        {
            if (!withDefaults)
                return;

            MMType = MatchManagerType.SKIRMISH;
            Level = GameLevel.BASIC_MAP;
            RoundCount = 1;
            BotType = SharedDef.DEFAULT_BOT;
            BotCount = 0;
            PlayedMatches = 0;
        }
    }
}
