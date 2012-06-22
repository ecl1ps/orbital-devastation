using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Client;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class MinorAsteroid : Asteroid
    {
        public UnstableAsteroid Parent { get; set; }
        private Player lastHitTakenFrom;

        public MinorAsteroid(SceneMgr mgr) : base(mgr)
        {
        }

        public override void TakeDamage(int damage, ISceneObject from)
        {
            base.TakeDamage(damage, from);
            if (from is SingularityBullet)
                lastHitTakenFrom = (from as SingularityBullet).Owner;
        }

        public override void OnRemove()
        {
            if (lastHitTakenFrom != null)
                Parent.NoticeChildAsteroidDestroyedBy(lastHitTakenFrom, this);
        }

        public override void DoCollideWith(ICollidable other)
        {
            base.DoCollideWith(other);
            if (!(other is SingularityBullet))
                lastHitTakenFrom = null;
        }
    }
}
