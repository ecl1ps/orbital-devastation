using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client
{
    public class LevelEnvironment : IGameState
    {
        public float CurrentGravity { get; set; }
        private float gravityChangeTimer = 0;

        public LevelEnvironment()
        {
            CurrentGravity = SharedDef.GRAVITY;
        }

        public void Update(float tpf)
        {
            if (gravityChangeTimer != 0)
            {
                if (gravityChangeTimer < tpf)
                {
                    CurrentGravity = SharedDef.GRAVITY;
                    gravityChangeTimer = 0;
                }
                else
                    gravityChangeTimer -= tpf;
            }
        }

        public void ChangeGravity(float newGravity, float forIntervalInSeconds)
        {
            CurrentGravity = newGravity;
            gravityChangeTimer = forIntervalInSeconds;
        }
    }
}
