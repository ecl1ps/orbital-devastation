using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class MiningEllipseControl : LineTravelingControl
    {
        private float travellingTime = SharedDef.SPECTATOR_ORBITS_TRAVELLING_TIME;
        public override float TravellingTime
        {
            get
            {
                return travellingTime;
            }
            set
            {
            }
        }
    }
}
