using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LineTravelingControl : Control
    {
        public Line LineToFollow { get; set; }

        private float time = 0;

        public virtual float TravellingTime { get; set; }

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

            UpdateMinePosition(TravellingTime);
        }

        private void UpdateMinePosition(float travellingTime)
        {
            Vector travellingDirection = LineToFollow.Start - LineToFollow.End;
            double travellingLenght = travellingDirection.Length;
            travellingDirection = travellingDirection.NormalizeV();
            float timeProgress = time / travellingTime;

            me.Position = (LineToFollow.End + (travellingDirection * (travellingLenght * timeProgress))) + (me.Position - me.Center);
        }
    }
}
