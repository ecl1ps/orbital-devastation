using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Input;
using System.Windows.Media;

namespace Orbit.Core
{
    public static class GameProperties
    {
        private static Properties props = new Properties(SharedDef.CONFIG_FILE, GetDefaultConfigValues());
        public static Properties Props
        {
            get
            {
                return props;
            }
        }

        public static string Get(PropertyKey key)
        {
            return Props.Get(key);
        }

        internal static Dictionary<PropertyKey, string> GetDefaultConfigValues()
        {
            Dictionary<PropertyKey, string> defaults = new Dictionary<PropertyKey, string>();
            defaults.Add(PropertyKey.PLAYER_NAME, "Player");
            defaults.Add(PropertyKey.PLAYER_HASH_ID, Player.GenerateNewHashId("Player"));
            defaults.Add(PropertyKey.STATIC_MOUSE_ENABLED, true.ToString());
            defaults.Add(PropertyKey.STATIC_MOUSE_CURSOR, "pack://application:,,,/resources/images/mouse/targeting_icon2.png");
            defaults.Add(PropertyKey.STATIC_MOUSE_SENSITIVITY, "1");
            defaults.Add(PropertyKey.MUSIC_ENABLED, true.ToString());
            defaults.Add(PropertyKey.MUSIC_VOLUME, "0,2");
            defaults.Add(PropertyKey.SOUNDS_VOLUME, "0,1");
            defaults.Add(PropertyKey.USED_SERVERS, "");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO1, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO2, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO3, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO4, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO5, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_QUICK_GAME, "0");
            defaults.Add(PropertyKey.PLAYER_ACTION_1, ((int)Key.D1).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_2, ((int)Key.D2).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_3, ((int)Key.D3).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_4, ((int)Key.D4).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_5, ((int)Key.D5).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_TOP, ((int)Key.W).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_BOT, ((int)Key.S).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_RIGHT, ((int)Key.D).ToString());
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_LEFT, ((int)Key.A).ToString());
            defaults.Add(PropertyKey.PLAYER_SHOW_ACTIONS, ((int)Key.Q).ToString());
            defaults.Add(PropertyKey.PLAYER_SHOW_PROTECTING, ((int)Key.E).ToString());
            defaults.Add(PropertyKey.AVAILABLE_COLORS, ((int)PlayerColorSet.BASIC).ToString());
            defaults.Add(PropertyKey.CHOSEN_COLOR, Colors.CornflowerBlue.ToString());
            return defaults;
        }
    }

    public enum PropertyKey
    {
        PLAYER_NAME,
        PLAYER_HASH_ID,

        STATIC_MOUSE_ENABLED,
        STATIC_MOUSE_CURSOR,
        STATIC_MOUSE_SENSITIVITY,

        MUSIC_ENABLED,
        MUSIC_VOLUME,
        SOUNDS_VOLUME,

        USED_SERVERS,

        PLAYER_HIGHSCORE_SOLO1,
        PLAYER_HIGHSCORE_SOLO2,
        PLAYER_HIGHSCORE_SOLO3,
        PLAYER_HIGHSCORE_SOLO4,
        PLAYER_HIGHSCORE_SOLO5,
        PLAYER_HIGHSCORE_QUICK_GAME,

        PLAYER_ACTION_1,
        PLAYER_ACTION_2,
        PLAYER_ACTION_3,
        PLAYER_ACTION_4,
        PLAYER_ACTION_5,
        PLAYER_ACTION_MOVE_TOP,
        PLAYER_ACTION_MOVE_BOT,
        PLAYER_ACTION_MOVE_RIGHT,
        PLAYER_ACTION_MOVE_LEFT,
        PLAYER_SHOW_ACTIONS,
        PLAYER_SHOW_PROTECTING,

        AVAILABLE_COLORS,
        CHOSEN_COLOR,

        PROPERTY_CHECK,
    }
}
