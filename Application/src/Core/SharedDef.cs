
namespace Orbit.Core
{
    class SharedDef
    {
        // Application
        public const long MINIMUM_UPDATE_TIME       = 30;

        public const int PORT_NUMBER                = 40;

        // Scene
        public const int MIN_SPHERE_RADIUS          = 10;
        public const int MAX_SPHERE_RADIUS          = 30;

        public const int MIN_SPHERE_SPEED           = 70;
        public const int MAX_SPHERE_SPEED           = 150;
        public const int FIRST_COSMICAL_SPEED       = 120;

        public const int MIN_SPHERE_ROTATION_SPEED  = -10;
        public const int MAX_SPHERE_ROTATION_SPEED  =  10;

        public const int SPHERE_COUNT               = 1;

        public const int BASE_MAX_INGERITY          = 100;

        public const float MINE_INVISIBILITY_TIME   = 0.5f;
        public const float MINE_LIFE_TIME           = 0.5f;

        public const float MINE_GROWTH_SPEED        = 1f;
        public const float MINE_STRENGTH            = 100f;
        public const int MINE_COOLDOWN              = 200;

        // polovina gravitacniho zrychleni
        public const float GRAVITY                  = 5f;

        public const int MIN_GRAVITY_HEIGHT         = 0;
        public const int MAX_GRAVITY_HEIGHT         = 100;
        public const float STABLE_ORBIT_RELATIVE    = 0.9f;

    }

    public enum Gametype
    {
        SOLO_GAME,
        SERVER_GAME,
        CLIENT_GAME
    }

    public enum GameEnd
    {
        WIN_GAME,
        LEFT_GAME
    }

    public enum PacketType
    {
        PLAYER_CONNECT,
        PLAYER_WON,
        SYNC_ALL_PLAYER_DATA,
        SYNC_ALL_ASTEROIDS,
        NEW_ASTEROID,
        NEW_SINGULARITY_MINE,
        SINGULARITY_MINE_HIT,
    }    
}
