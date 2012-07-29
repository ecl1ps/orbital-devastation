using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

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

        private static Dictionary<PropertyKey, string> GetDefaultConfigValues()
        {
            Dictionary<PropertyKey, string> defaults = new Dictionary<PropertyKey, string>();
            defaults.Add(PropertyKey.PLAYER_NAME, "Player");
            defaults.Add(PropertyKey.PLAYER_HASH_ID, Player.GenerateNewHashId("Player"));
            defaults.Add(PropertyKey.STATIC_MOUSE_ENABLED, true.ToString());
            defaults.Add(PropertyKey.STATIC_MOUSE_CURSOR, "pack://application:,,,/resources/images/mouse/targeting_icon2.png");
            defaults.Add(PropertyKey.STATIC_MOUSE_SENSITIVITY, "1");
            defaults.Add(PropertyKey.MUSIC_ENABLED, true.ToString());
            defaults.Add(PropertyKey.MUSIC_VOLUME, "1");
            defaults.Add(PropertyKey.SOUNDS_VOLUME, "1");
            defaults.Add(PropertyKey.USED_SERVERS, "");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO1, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO2, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO3, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO4, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_SOLO5, "0");
            defaults.Add(PropertyKey.PLAYER_HIGHSCORE_QUICK_GAME, "0");
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
        PLAYER_HIGHSCORE_QUICK_GAME
    }
}
