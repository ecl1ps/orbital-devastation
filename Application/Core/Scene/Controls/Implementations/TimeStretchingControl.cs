using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class TimeStretchingControl : StretchingLineControl
    {
        public float Time { get; set; }
        private float timeLeft = 0;

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            if (timeLeft >= Time)
            {
                timeLeft = Time;
                return;
            }

            timeLeft += tpf;
        }

        protected override void UpdateLine()
        {
            Vector travellingDirection = SecondObj.Center - FirstObj.Center;
            double travellingLenght = travellingDirection.Length;
            travellingDirection = travellingDirection.NormalizeV();
            float timeProgress = timeLeft / Time;

            line.Start = FirstObj.Center;
            line.End = (FirstObj.Center + (travellingDirection * (travellingLenght * timeProgress)));
        }
    }
}
