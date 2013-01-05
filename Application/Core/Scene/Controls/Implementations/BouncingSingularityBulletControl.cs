using System;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class BouncingSingularityBulletControl : ExplodingSingularityBulletControl
    {
        public override void HitAsteroid(IDestroyable asteroid)
        {
            base.HitAsteroid(asteroid);
            if(me.SceneMgr.GetCurrentPlayer().IsActivePlayer())
                (me as SingularityBouncingBullet).SpawnNewBullet(asteroid);
        }
    }
}
