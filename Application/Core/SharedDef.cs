
using System.Windows;
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

        public const int ASTEROID_COUNT                     = 30;
        public const int GOLD_ASTEROID_BONUS_MULTIPLY       = 10;

        public const int ASTEROID_GOLD_CHANCE               = 25; // (0 - 100)
        public const int ASTEROID_UNSTABLE_CHANCE           = 80; // 10 procent, musi se brat ohled na chance ostatnich typu

        public const int BASE_MAX_INGERITY                  = 100;

        // pouzito pouze v SingularityControl
        public const float MINE_INVISIBILITY_TIME           = 0.5f;

        public const float MINE_LIFE_TIME                   = 0.3f;

        public const float MINE_GROWTH_SPEED                = 1f;
        public const float MINE_STRENGTH                    = 100f;
        public const float MINE_COOLDOWN                    = 1f;
        public const int MINE_FALLING_SPEED                 = 100;

        // polovina gravitacniho zrychleni
        public const float GRAVITY                          = 5f;

        public const int MIN_GRAVITY_HEIGHT                 = 0;
        public const int MAX_GRAVITY_HEIGHT                 = 100;
        public const float STABLE_ORBIT_RELATIVE            = 0.9f;

        public const int BULLET_SPEED                       = 300;
        public const float BULLET_COOLDOWN                  = MINE_COOLDOWN / 2;
        public const int BULLET_DMG                         = 5;
        public const float BULLET_LIFE_TIME                 = 1f;
        public const float BULLET_EXPLODE_DURATION          = 0.3f;
        public const float BULLET_EXPLOSION_STRENGTH        = 50;
        public const float BULLET_EXPLOSION_SPEED           = 0.6f;

        public const int HOOK_LENGHT                        = 400;
        public const int HOOK_SPEED                         = 150;

        public const int HEAL_MULTIPLY_COEF                 = 2;
        public const int HEAL_AMOUNT                        = 25;
        public const int HEAL_START_COST                    = 10;

        public const float ACTION_BAR_TOP_MARGIN_PCT        = 0.3f;

        public const bool ALLOW_SPECTATORS_IN_DUO_MATCH     = true;

        public const int START_GOLD                         = 500;

        /// <summary>
        /// velikost canvasu je zaroven velikost celeho okna
        /// </summary>
        public static Size CANVAS_SIZE = new Size(1000, 700);

        /// <summary>
        /// view port je oblast, kde se odehrava cela hra - mimo ni by se nemelo nic dit (mimo je pak action bar)
        /// </summary>
        public static Size VIEW_PORT_SIZE = new Size(CANVAS_SIZE.Width, CANVAS_SIZE.Height - 50); // 50 JSOU BARY DOLE

        /// <summary>
        /// orbit area je horni oblast obrazovky - pas kde se pohybuji asteroidy
        /// </summary>
        public static Rect ORBIT_AREA = new Rect(0, 0, CANVAS_SIZE.Width, 200);

        public const string CONFIG_FILE                     = "player";
    }

    public enum Gametype
    {
        NONE,
        SOLO_GAME,
        MULTIPLAYER_GAME,
        TOURNAMENT_GAME
    }

    public enum GameEnd
    {
        WIN_GAME,
        LEFT_GAME,
        SERVER_DISCONNECTED,
        TOURNAMENT_FINISHED
    }

    public enum PacketType
    {
        PLAYER_CONNECT,
        PLAYER_ID_HAIL,

        TOURNAMENT_STARTING,

        PLAYER_WON,

        ALL_PLAYER_DATA,
        ALL_PLAYER_DATA_REQUEST,

        ALL_ASTEROIDS,
        NEW_ASTEROID,
        MINOR_ASTEROID_SPAWN,
        NEW_SINGULARITY_MINE,
        NEW_SINGULARITY_BULLET,
        NEW_HOOK,

        SINGULARITY_MINE_HIT,
        HOOK_HIT,
        BULLET_HIT,

        ASTEROID_DESTROYED,
        BASE_INTEGRITY_CHANGE,
        PLAYER_HEAL,

        START_GAME_RESPONSE,
        START_GAME_REQUEST,
        PLAYER_READY,
        CHAT_MESSAGE,
        PLAYER_DISCONNECTED,
        SERVER_SHUTDOWN,
        TOURNAMENT_FINISHED,
        PLAYER_SCORE,
        SCORE_QUERY,
        SCORE_QUERY_RESPONSE,
    }    
}
