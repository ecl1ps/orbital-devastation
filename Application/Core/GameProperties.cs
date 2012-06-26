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
            return defaults;
        }
    }

    public enum PropertyKey
    {
        PLAYER_NAME,
        PLAYER_HASH_ID,
        STATIC_MOUSE_ENABLED,
        USED_SERVERS
    }
}
