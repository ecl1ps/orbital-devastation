using System;
using Orbit.Core.Scene.Entities;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using Lidgren.Network;
using System.Windows.Media;
using System.Windows.Threading;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Shapes;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class BouncingSingularityBulletControl : ExplodingSingularityBulletControl
    {
        public override void HitAsteroid(IDestroyable asteroid)
        {
            base.HitAsteroid(asteroid);
            (me as SingularityBouncingBullet).SpawnNewBullet(asteroid);
        }
    }
}
