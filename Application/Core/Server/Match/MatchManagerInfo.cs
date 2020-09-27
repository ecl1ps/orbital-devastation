using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Server.Match
{
    public class MatchManagerInfo
    {
        public bool IsDebug { get; }
        public string Text { get; }
        public int MinPlayerCount { get; }

        public MatchManagerInfo(bool isUsedForDebug, string text, int minPlayers)
        {
            IsDebug = isUsedForDebug;
            Text = text;
            MinPlayerCount = minPlayers;
        }
    }

    public class MatchManagerInfoAccessor
    {
        public static MatchManagerInfo GetInfo(MatchManagerType type)
        {
            switch (type)
            {
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
