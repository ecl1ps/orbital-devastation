using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Server.Level
{
    public class LevelInfo
    {
        public bool IsDebug { get; set; }
        public string Text { get; set; }

        public LevelInfo(bool isUsedForDebug, string text)
        {
            IsDebug = isUsedForDebug;
            Text = text;
        }
    }

    public class LevelInfoAccessor
    {
        public static LevelInfo GetInfo(GameLevel level)
        {
            switch (level)
            {
                case GameLevel.BASIC_MAP:
                    return LevelBasic.Info;
                case GameLevel.SURVIVAL_MAP:
                    return LevelSurvival.Info;
                case GameLevel.TEST_BASE_COLLISIONS:
                    return LevelTestBaseCollisions.Info;
                case GameLevel.TEST_EMPTY:
                    return LevelTestEmpty.Info;
                case GameLevel.TEST_POWERUPS:
                    return LevelTestPoweUp.Info;
                case GameLevel.TEST_STATIC_OBJ:
                    return LevelTestStaticObjects.Info;
                default:
                    throw new Exception("Level " + level.ToString() + " has not supported Info property");
            }
        }
    }
}
