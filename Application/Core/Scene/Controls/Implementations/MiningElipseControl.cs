using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class MiningEllipseControl : Control
    {
        public SolidLine LineToFollow { get; set; }
        private float time = 0;

        public override void InitControl(Entities.ISceneObject me)
        {
        }

        public override void UpdateControl(float tpf)
        {
            time += tpf;
            if (LineToFollow.Dead || time >= SharedDef.SPECTATOR_ORBITS_TRAVELLING_TIME)
            {
                me.DoRemoveMe();
                return;
            }

            UpdateMinePosition();
        }

        private void UpdateMinePosition()
        {
            Vector travellingDirection = LineToFollow.Start - LineToFollow.End;
            double travellingLenght = travellingDirection.Length;
            travellingDirection = travellingDirection.NormalizeV();
            float timeProgress = time / SharedDef.SPECTATOR_ORBITS_TRAVELLING_TIME;

            me.Position = (LineToFollow.End + (travellingDirection * (travellingLenght * timeProgress))).ToVector();
        }
    }
}
