using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class LimitedLifeControl : Control
    {
        private float timeLeft;

        public LimitedLifeControl(float timeToDie)
        {
            timeLeft = timeToDie;
        }

        public override void InitControl(Entities.ISceneObject me)
        {
        }

        public override void UpdateControl(float tpf)
        {
            timeLeft -= tpf;
            if (timeLeft <= 0)
                me.DoRemoveMe();
        }
    }
}
