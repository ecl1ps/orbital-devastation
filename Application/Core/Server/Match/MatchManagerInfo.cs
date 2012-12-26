using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Server.Match
{
    public class MatchManagerInfo
    {
        public bool IsDebug { get; set; }
        public string Text { get; set; }

        public MatchManagerInfo(bool isUsedForDebug, string text)
        {
            IsDebug = isUsedForDebug;
            Text = text;
        }
    }

    public class MatchManagerInfoAccessor
    {
        public static MatchManagerInfo GetInfo(MatchManagerType type)
        {
            switch (type)
            {
                case MatchManagerType.WINS_THEN_SCORE:
                    return WinsThenScoreMatchManager.Info;
                case MatchManagerType.ONLY_SCORE:
                    return ScoreMatchManager.Info;
                case MatchManagerType.SKIRMISH:
                    return SkirmishMatchManager.Info;
                case MatchManagerType.QUICK_GAME:
                    return QuickGameMatchManager.Info;
                case MatchManagerType.TEST_LEADER_SPECTATOR:
                    return LeaderSpectatorMatchManager.Info;
                default:
                    throw new Exception("MatchManager " + type.ToString() + " has not supported Info property");
            }
        }
    }
}
