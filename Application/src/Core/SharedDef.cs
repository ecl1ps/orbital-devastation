
namespace Orbit.Core
{
    class SharedDef
    {
        // Application
        public const long MINIMUM_UPDATE_TIME       = 30;

        public const int PORT_NUMBER                = 194;

        // Scene
        public const int MIN_SPHERE_RADIUS          = 10;
        public const int MAX_SPHERE_RADIUS          = 30;

        public const int MIN_SPHERE_SPEED           = 70;
        public const int MAX_SPHERE_SPEED           = 150;

        public const int MIN_SPHERE_ROTATION_SPEED  = -10;
        public const int MAX_SPHERE_ROTATION_SPEED  =  10;

        public const int SPHERE_COUNT               = 20;

        public const int BASE_MAX_INGERITY          = 100;

        public const float MINE_INVISIBILITY_TIME   = 0.5f;
        public const float MINE_LIFE_TIME           = 0.5f;

        public const float MINE_GROWTH_SPEED        = 1f;
        public const float MINE_STRENGTH            = 1f;
        public const int MINE_COOLDOWN              = 200;

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
        SYNC_ALL_PLAYER_DATA,
        SYNC_ALL_ASTEROIDS,
        NEW_ASTEROID,
        NEW_SINGULARITY_MINE,
        PLAYER_WON,
    }    
}
