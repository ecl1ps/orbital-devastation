using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;

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
            defaults.Add(PropertyKey.GAME_LANGUAGE, "en");
            defaults.Add(PropertyKey.PLAYER_NAME, "Player");
            defaults.Add(PropertyKey.PLAYER_HASH_ID, Player.GenerateNewHashId("Player"));
            defaults.Add(PropertyKey.STATIC_MOUSE_ENABLED, true.ToString(Strings.Culture));
            defaults.Add(PropertyKey.STATIC_MOUSE_CURSOR, "pack://application:,,,/resources/images/mouse/targeting_icon2.png");
            defaults.Add(PropertyKey.STATIC_MOUSE_SENSITIVITY, 1.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.MUSIC_ENABLED, true.ToString(Strings.Culture));
            defaults.Add(PropertyKey.MUSIC_VOLUME, 0.1.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.SOUNDS_VOLUME, 0.05.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.SERVER_ADDRESS, SharedDef.DEFAULT_MASTER_SERVER_ADDRESS);
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO1, 0.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO2, 0.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO3, 0.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO4, 0.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO5, 0.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_QUICK_GAME, 0.ToString(CultureInfo.InvariantCulture));
            defaults.Add(PropertyKey.PLAYER_ACTION_1, ((int)Key.D1).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_2, ((int)Key.D2).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_3, ((int)Key.D3).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_4, ((int)Key.D4).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_5, ((int)Key.D5).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_TOP, ((int)Key.W).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_BOT, ((int)Key.S).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_RIGHT, ((int)Key.D).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_ACTION_MOVE_LEFT, ((int)Key.A).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_SHOW_ACTIONS, ((int)Key.Q).ToString(Strings.Culture));
            defaults.Add(PropertyKey.PLAYER_SHOW_PROTECTING, ((int)Key.E).ToString(Strings.Culture));
            defaults.Add(PropertyKey.AVAILABLE_COLORS, ((int)PlayerColorSet.BASIC).ToString(Strings.Culture));
            defaults.Add(PropertyKey.CHOSEN_COLOR, Colors.CornflowerBlue.ToString(Strings.Culture));
            defaults.Add(PropertyKey.EFFECT_QUALITY, "effect1");
            return defaults;
        }
    }

    public enum PropertyKey
    {
        GAME_LANGUAGE,

        PLAYER_NAME,
        PLAYER_HASH_ID,

        STATIC_MOUSE_ENABLED,
        STATIC_MOUSE_CURSOR,
        STATIC_MOUSE_SENSITIVITY,

        MUSIC_ENABLED,
        MUSIC_VOLUME,
        SOUNDS_VOLUME,

        SERVER_ADDRESS,

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

        EFFECT_QUALITY,
    }
}
