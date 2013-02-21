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
        public int ServerId { get; set; }
        //public byte PlayerCount { get; set; }
        public string Name { get; set; }
        public string Leader { get; set; }
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

            ServerId = 0;
            //PlayerCount = 1;
            MMType = MatchManagerType.SKIRMISH;
            Level = GameLevel.BASIC_MAP;
            RoundCount = 1;
            BotType = SharedDef.DEFAULT_BOT;
            BotCount = 0;
            PlayedMatches = 0;
        }
    }

    public class VisualizableTorunamentSettings
    {
        public string Name { get { return Settings.Name; } }
        public string Leader { get { return Settings.Leader; } }
        public string MMType { get { return MatchManagerInfoAccessor.GetInfo(Settings.MMType).Text; } }
        public string Level { get { return LevelInfoAccessor.GetInfo(Settings.Level).Text; } }
        public string RoundCount { get { return Settings.RoundCount.ToString(Strings.Culture); } }

        public TournamentSettings Settings { get; set; }

        public VisualizableTorunamentSettings(TournamentSettings s)
        {
            Settings = s;
        }
    }
}
