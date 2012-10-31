using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client.GameStates
{
    public class LevelEnvironment : IGameState
    {
        public float CurrentGravity { get; set; }
        public float StableOrbitRelative { get; set; }
        private float gravityChangeTimer = 0;

        public LevelEnvironment()
        {
            CurrentGravity = SharedDef.GRAVITY;
            StableOrbitRelative = SharedDef.STABLE_ORBIT_RELATIVE;
        }

        public void Update(float tpf)
        {
            if (gravityChangeTimer != 0)
            {
                if (gravityChangeTimer < tpf)
                {
                    CurrentGravity = SharedDef.GRAVITY;
                    StableOrbitRelative = SharedDef.STABLE_ORBIT_RELATIVE;
                    gravityChangeTimer = 0;
                }
                else // TODO interpolace aby konec zvetsene gravitace nebyl prudky
                    gravityChangeTimer -= tpf;
            }
        }

        public void ChangeGravity(float newGravity, float forIntervalInSeconds)
        {
            CurrentGravity = newGravity;
            StableOrbitRelative = 1.1f;
            gravityChangeTimer = forIntervalInSeconds;
        }
    }
}
