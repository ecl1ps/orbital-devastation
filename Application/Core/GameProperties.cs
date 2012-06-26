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
        USED_SERVERS,
        PLAYER_HIGHSCORE_SOLO1,
        PLAYER_HIGHSCORE_SOLO2,
        PLAYER_HIGHSCORE_SOLO3,
        PLAYER_HIGHSCORE_SOLO4,
        PLAYER_HIGHSCORE_SOLO5,
        PLAYER_HIGHSCORE_QUICK_GAME
    }
}
