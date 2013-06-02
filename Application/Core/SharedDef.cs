
using System.Windows;
using Orbit.Core.Server;
using Orbit.Core.Players;
using Orbit.Core.Server.Level;
namespace Orbit.Core
{
    class SharedDef
    {
                                                            // 0.8 alfa, 0.9 beta, 1.0 RC
                                                            // major change .01 - .99
                                                            // minor change .001 - .999
        public const string VERSION                         = "0.8.01.003";

        public const string DOWNLOAD_LINK                   = "http://77.236.211.123/testbuilds/OrbitalDevastation.exe";

        // Application
        public const long MINIMUM_UPDATE_TIME               = 17;

        public const string MASTER_SERVER_ADDRESS           = "77.236.211.123";
        public const int MASTER_SERVER_PORT                 = 40;

        public const int TOURNAMENT_LIST_REQUEST_INTERVAL   = 5000; //ms

        public const int VICTORY_MAX_SCORE                  = 1300;
        public const int VICTORY_MIN_SCORE                  = 1000;

        public const int LOOSE_MAX_SCORE                    = 500;
        public const int LOOSE_MIN_SCORE                    = 200;

        public const float MIN_TIME                         = 180;
        public const float MAX_TIME                         = 1200;

        public const int SOLO_SPECTATOR_MAX_SCORE           = 1500;
        public const int SOLO_SPECTATOR_MIN_SCORE           = 0;

        public const float SOLO_SPECTATOR_MIN_TIME          = 0;
        public const float SOLO_SPECTATOR_MAX_TIME          = 1800;

        public const float SPECTATOR_SCORE_BONUS            = 0.5f;

        // Scene
        public const int MIN_ASTEROID_RADIUS                = 10;
        public const int MAX_ASTEROID_RADIUS                = 30;

        public const int MIN_ASTEROID_SPEED                 = 70;
        public const int MAX_ASTEROID_SPEED                 = 150;

        public const int MIN_POWERUP_SPEED                  = 100;
        public const int MAX_POWERUP_SPEED                  = 150;

        public const int ASTEROID_THRESHOLD_RADIUS          = 10;
        public const int ASTEROID_MAX_GROWN_RADIUS          = 35;

        public const int FIRST_COSMICAL_SPEED               = 120;

        public const int MIN_ASTEROID_ROTATION_SPEED        = -10;
        public const int MAX_ASTEROID_ROTATION_SPEED        =  10;

        public const int ASTEROID_COUNT                     = 25;
        public const float NEW_ASTEROID_TIMER               = 30; // seconds
        public const int GOLD_ASTEROID_BONUS_MULTIPLY       = 10;

        public const int ASTEROID_GOLD_CHANCE               = 25; // (0 - 100)
        public const int ASTEROID_UNSTABLE_CHANCE           = 50; // 25 procent musi se brat ohled na sance ostatnich mozna dodelat nejaky dopocitani. Ale co s normalnima potom?

        public const int NEW_STAT_POWERUP_TIMER_MIN         = 2; // seconds
        public const int NEW_STAT_POWERUP_TIMER_MAX         = 4; // seconds

        // pouzito pouze v SingularityControl
        public const float MINE_INVISIBILITY_TIME           = 0.5f;

        public const float MINE_LIFE_TIME                   = 0.3f;

        public const float MINE_GROWTH_SPEED                = 1f;
        public const float MINE_STRENGTH                    = 100f;
        public const float MINE_COOLDOWN                    = 1f;
        public const int MINE_FALLING_SPEED                 = 100;
        public const float MINE_LAUNCHER_SPEED_MODIFIER     = 2;

        // polovina gravitacniho zrychleni
        public const float GRAVITY                          = 5f;

        public const int MIN_GRAVITY_HEIGHT                 = 0;
        public const int MAX_GRAVITY_HEIGHT                 = 100;
        public const float STABLE_ORBIT_RELATIVE            = 0.9f;

        public const int BULLET_SPEED                       = 400;
        public const float BULLET_COOLDOWN                  = MINE_COOLDOWN / 2;
        public const int BULLET_DMG                         = 5;
        public const float BULLET_LIFE_TIME                 = 1f;
        public const float BULLET_EXPLODE_DURATION          = 0.3f;
        public const float BULLET_EXPLOSION_STRENGTH        = 50;
        public const float BULLET_EXPLOSION_SPEED           = 0.6f;
        public const int BULLET_ACTIVE_SHRAPNEL_COUNT       = 3;

        public const int HOOK_LENGHT                        = 420;
        public const int HOOK_SPEED                         = 150;
        public const float HOOK_COOLDOWN                    = 1.5f;
        public const int HOOK_MAX_OBJ_COUNT                 = 2;

        public const int HOOK_ACTIVE_PULLABLE_WEIGHT        = 40;
        public const float HOOK_ACTIVE_PULL_REACH_DIST      = 100f;

        public const int HEAL_MULTIPLY_COEF                 = 2;
        public const float HEAL_AMOUNT                      = 0.25f; //procenta
        public const int HEAL_START_COST                    = 10;
        public const int BONUS_HEAL                         = 0;

        public const float ACTION_BAR_TOP_MARGIN_PCT        = 0.3f;

        public const bool ALLOW_SPECTATORS_IN_DUO_MATCH     = false;
        public const int MAX_TOURNAMENT_PLAYERS             = 6;

        //konstanty pro spectatory
        public const float SPECTATOR_MAX_TIME_BONUS         = 60 * 30; // pul hodiny
        public const float SPECTATOR_MINING_RADIUS          = 200;
        public const float SPECTATOR_MODULE_SPEED           = 150;
        public const float SPECTATOR_COLLISION_INTERVAL     = 0.5f;
        public const float SPECTATOR_ORBITS_TRAVELLING_TIME = 1.2f;
        public const float SPECTATOR_ORBITS_SPAWN_TIME      = 0.6f;
        public const float SPECTATOR_MODULE_ROTATION_SPEED  = (float) System.Math.PI;
        public const int SPECTATOR_MAX_HP                   = 100;
        public const float SPECTATOR_HP_REGEN_CD            = 3; // sekundy
        public const float SPECTATOR_REGEN_SPEED            = 2; //sekundy
        public const float SPECTATOR_RESPAWN_TIME           = 8; //sekundy

        //Konstanty pro spectator akce
        public const float SPECTATOR_ASTEROID_THROW_SPEED   = 120;
        public const float SPECTATOR_SHIELDING_TIME         = 5;
        public const int SPECTATOR_DAMAGE                   = 6;
        public const int SPECTATOR_GROWTH                   = 6;

        public static readonly Vector DEFAULT_VECTOR        = new Vector(1, 0);

        /// <summary>
        /// velikost canvasu je zaroven velikost celeho okna
        /// </summary>
        public static readonly Size CANVAS_SIZE             = new Size(1000, 700);

        /// <summary>
        /// view port je oblast, kde se odehrava cela hra - mimo ni by se nemelo nic dit (mimo je pak action bar)
        /// </summary>
        public static readonly Size VIEW_PORT_SIZE          = new Size(CANVAS_SIZE.Width, CANVAS_SIZE.Height - 60); // 60 JSOU BARY DOLE

        /// <summary>
        /// orbit area je horni oblast obrazovky - pas kde se pohybuji asteroidy
        /// </summary>
        public static readonly Rect ORBIT_AREA              = new Rect(0, 0, CANVAS_SIZE.Width, 200);

        public static readonly Rect LOWER_ORBIT_AREA        = new Rect(0, 240, CANVAS_SIZE.Width, 80);

        public const string CONFIG_FILE                     = "data";
        public const GameLevel STARTING_LEVEL               = GameLevel.BASIC_MAP;
        public const BotType DEFAULT_BOT                    = BotType.LEVEL2;

        //Jmena zvuku
        public const string MUSIC_METEOR_HIT                = "meteor_hit";
        public const string MUSIC_DAMAGE_TO_BASE            = "base_dmg";
        public const string MUSIC_SHOOT                     = "shoot";
        public const string MUSIC_EXPLOSION                 = "explosion";
        public const string MUSIC_VICTORY                   = "victory";
        public const string MUSIC_LOSE                      = "lose";
        public const string MUSIC_BACKGROUND_CALM           = "bg_calm";
        public const string MUSIC_BACKGROUND_ACTION         = "bg_action";
        public const string MUSIC_ALERT                     = "music_alert";

        public const string SALT                            = "Kj5dfO0OR";

        public const int LEVEL_SURVIVAL_ASTEROID_COUNT              = 8;
        public const float LEVEL_SURVIVAL_ASTEROID_TIMER            = 0.4f;
        public const float LEVEL_SURVIVAL_ASTEROID_TIMER_CAP        = 0.2f;

#if DEBUG
        public const bool SKIP_STATISTICS                   = true;
        public const int START_GOLD                         = 25000;
#else
        public const bool SKIP_STATISTICS                   = false;
        public const int START_GOLD                         = 250;
#endif
        public const int BASE_MAX_INGERITY                  = 100;

    }

    public enum WindowState
    {
        IN_MAIN_MENU,
        IN_LOBBY,
        IN_GAME
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
        VERSION_MISMATCH,

        PLAYER_KICK_REQUEST,
        PLAYER_READY_CHECK,

        TOURNAMENT_STARTING,

        PLAYER_WON,

        ALL_PLAYER_DATA,
        ALL_PLAYER_DATA_REQUEST,

        ALL_ASTEROIDS,
        NEW_ASTEROID,
        MINOR_ASTEROID_SPAWN,
        NEW_SINGULARITY_MINE,
        NEW_SINGULARITY_BULLET,
        NEW_SINGULARITY_EXPLODING_BULLET,
        NEW_SINGULARITY_BOUNCING_BULLET,
        NEW_HOOK,
        NEW_STAT_POWERUP,

        SINGULARITY_MINE_HIT,
        HOOK_HIT,
        HOOK_FORCE_PULL,
        BULLET_HIT,

        REMOVE_OBJECT,

        ASTEROID_DESTROYED,
        BASE_INTEGRITY_CHANGE,
        PLAYER_HEAL,
        PLAYER_COLOR_CHANGED,

        START_GAME_RESPONSE,
        START_GAME_REQUEST,
        PLAYER_READY,
        CHAT_MESSAGE,
        PLAYER_DISCONNECTED,
        SERVER_SHUTDOWN,
        TOURNAMENT_FINISHED,
        PLAYER_SCORE_AND_GOLD,
        SCORE_QUERY,
        SCORE_QUERY_RESPONSE,
        PLAYER_RECEIVED_POWERUP,
        PLAYER_BOUGHT_UPGRADE,
        FLOATING_TEXT,
        PLAY_SOUND,

        MOVE_STATE_CHANGED,
        MINING_MODULE_DMG_TAKEN,
        ASTEROIDS_DIRECTIONS_CHANGE,
        OBJECTS_TAKE_DAMAGE,
        MODULE_COLOR_CHANGE,

        TOURNAMENT_SETTINGS,
        TOURNAMENT_SETTINGS_REQUEST,
        PLAYER_RECONNECTED,
        OBJECTS_HEAL_AMOUNT,
        SPECTATOR_ACTION_START,
        PLAYER_SCORE_UPDATE,
        SHOW_ALLERT_MESSAGE,
        SCHEDULE_SPECTATOR_ACTION,
        PARTICLE_EMMITOR_CREATE,
        PARTICLE_EMMITOR_START,

        AVAILABLE_TOURNAMENTS_REQUEST,
        AVAILABLE_TOURNAMENTS_RESPONSE,
        AVAILABLE_RECONNECT_REQUEST,
        AVAILABLE_RECONNECT_RESPONSE,

        SHAKING_START,
    }    
}
