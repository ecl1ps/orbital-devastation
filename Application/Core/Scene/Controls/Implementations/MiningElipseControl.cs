using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class MiningEllipseControl : Control
    {
        public Line LineToFollow { get; set; }

        private float time = 0;

        protected override void InitControl(ISceneObject me)
        {
            events.AddEvent(1, new Event(SharedDef.SPECTATOR_ORBITS_TRAVELLING_TIME, EventType.ONE_TIME, new Action(() => { me.DoRemoveMe(); })));
        }

        protected override void UpdateControl(float tpf)
        {
            time += tpf;

            if (LineToFollow.Dead)
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
            
            me.Position = (LineToFollow.End + (travellingDirection * (travellingLenght * timeProgress))) + (me.Position - me.Center);
        }
    }
}
