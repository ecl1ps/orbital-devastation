using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.AI
{
    public class BotNameAccessor
    {
        public static string GetBotName(BotType type)
        {
            if (type == BotType.NONE)
                return null;
            switch (type)
            {
                case BotType.LEVEL1:
                    return SimpleBot.NAME;
                case BotType.LEVEL2:
                    return HookerBot.NAME;
                case BotType.LEVEL3:
                case BotType.LEVEL4:
                case BotType.LEVEL5:
                default:
                    return null;
            }
        }
    }
}
