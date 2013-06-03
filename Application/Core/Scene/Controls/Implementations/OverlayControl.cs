using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class OverlayControl : Control
    {
        private Asteroid asteroid;
        private ITransparent meTransparent;
        private double maxHeatDistance;
        private double minHeatDistance;

        public OverlayControl(Asteroid parent)
        {
            asteroid = parent;
            maxHeatDistance = PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT).Top - 150;
            minHeatDistance = SharedDef.ORBIT_AREA.Bottom;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is ITransparent))
                throw new ArgumentException("OverlayControl must by attached to an ITransparent object");

            meTransparent = me as ITransparent;
        }

        protected override void UpdateControl(float tpf)
        {
            if (asteroid.Center.Y < minHeatDistance || asteroid.Center.Y > maxHeatDistance)
                return;

            meTransparent.Opacity = FastMath.LinearInterpolate(0.3f, 1f, Math.Min((float)((asteroid.Center.Y - minHeatDistance) / (maxHeatDistance - minHeatDistance)), 1));
        }
    }
}
