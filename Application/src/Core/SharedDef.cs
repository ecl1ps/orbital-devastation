
namespace Orbit.Core
{
    class SharedDef
    {
        // Application
        public const long MINIMUM_UPDATE_TIME               = 17;

        public const int PORT_NUMBER                        = 40;

        // Scene
        public const int MIN_ASTEROID_RADIUS                = 10;
        public const int MAX_ASTEROID_RADIUS                = 30;

        public const int MIN_ASTEROID_SPEED                 = 70;
        public const int MAX_ASTEROID_SPEED                 = 150;

        public const int ASTEROID_THRESHOLD_RADIUS          = 10;

        public const int FIRST_COSMICAL_SPEED               = 120;

        public const int MIN_ASTEROID_ROTATION_SPEED        = -10;
        public const int MAX_ASTEROID_ROTATION_SPEED        =  10;

        public const int ASTEROID_COUNT                     = 20;
        public const int GOLD_ASTEROID_BONUS_MULTIPLY       = 10;

        public const int ASTEROID_GOLD_CHANCE               = 10; // (0 - 100)
        public const int ASTEROID_UNSTABLE_CHANCE           = 50; // 10 procent, musi se brat ohled na chance ostatnich typu

        public const int BASE_MAX_INGERITY                  = 100;

        // pouzito pouze v SingularityControl
        public const float MINE_INVISIBILITY_TIME           = 0.5f;

        public const float MINE_LIFE_TIME                   = 0.3f;

        public const float MINE_GROWTH_SPEED                = 1f;
        public const float MINE_STRENGTH                    = 100f;
        public const float MINE_COOLDOWN                    = 1f;
        public const int MINE_FALLING_SPEED                 = 50;

        // polovina gravitacniho zrychleni
        public const float GRAVITY                          = 5f;

        public const int MIN_GRAVITY_HEIGHT                 = 0;
        public const int MAX_GRAVITY_HEIGHT                 = 100;
        public const float STABLE_ORBIT_RELATIVE            = 0.9f;

        public const int BULLET_SPEED                       = 300;
        public const float BULLET_COOLDOWN                  = MINE_COOLDOWN / 2;
        public const int BULLET_DMG                         = 5;

        public const int HOOK_LENGHT                        = 400;
        public const int HOOK_SPEED                         = 150;

        public const int HEAL_MULTIPLY_COEF                 = 2;
        public const int HEAL_AMOUNT                        = 25;
        public const int HEAL_START_COST                    = 10;

        public const float ACTION_BAR_TOP_MARGIN_PCT        = 0.3f;
    }

    public enum Gametype
    {
        NONE,
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
        PLAYER_ID_HAIL,

        PLAYER_WON,

        ALL_PLAYER_DATA,
        ALL_ASTEROIDS,

        NEW_ASTEROID,
        NEW_SINGULARITY_MINE,
        NEW_SINGULARITY_BULLET,
        NEW_HOOK,

        SINGULARITY_MINE_HIT,
        ASTEROID_DESTROYED,
        BASE_INTEGRITY_CHANGE,
        PLAYER_HEAL,

        START_GAME_RESPONSE,
        START_GAME_REQUEST,
    }    
}
