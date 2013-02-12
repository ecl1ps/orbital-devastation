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
                    return Strings.bot_name_1;
                case BotType.LEVEL2:
                    return Strings.bot_name_2;
                case BotType.LEVEL3:
                    return Strings.bot_name_3;
                case BotType.LEVEL4:
                    return Strings.bot_name_4;
                case BotType.LEVEL5:
                    return Strings.bot_name_5;
                default:
                    return null;
            }
        }
    }
}
