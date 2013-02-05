using Orbit.Core.Server.Level.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Server.Level
{
    public class LevelInfo
    {
        private bool isDebug;
        public bool IsDebug { get { return isDebug; } }
        private string text;
        public string Text { get { return text; } }

        public LevelInfo(bool isUsedForDebug, string text)
        {
            isDebug = isUsedForDebug;
            this.text = text;
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
                case GameLevel.TEST_PARTICLES:
                    return LevelTestParticles.Info;
                default:
                    throw new Exception("Level " + level.ToString() + " has not supported Info property");
            }
        }
    }
}
