using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Players
{
    class ScoreDefines
    {
        public const int CANNON_HIT                                             = 5;
        public const int MINE_HIT                                               = 5;
        public const int HOOK_HIT                                               = 10;
        public const float GOLD_TAKEN_COEF                                      = 0.2f;
        public const int DAMAGE_DEALT                                           = 1;

        public const int MINE_HIT_MULTIPLE_EXPONENT                             = 3;
        public const int CANNON_DESTROYED_ENTIRE_UNSTABLE_ASTEROID              = 100;
        public const int CANNON_DESTROYED_UNSTABLE_ASTEROID_ABOVE_ENEMY         = 50;
        public const int HOOK_CAUGHT_OBJECT_AFTER_90PCT_DISTANCE                = 100;
    }
}
