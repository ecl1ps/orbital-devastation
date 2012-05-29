using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;

namespace Orbit.src.Core.Scene.Entities
{
    class SingularityBullet : SpherePoint
    {

        public Player Player { get; set; }
        public int Damage { get; set; }

        protected override void UpdateGeometricState()
        {
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is Meteor)
            {
                HitMeteor(other as Meteor);
                DoRemoveMe();
            }
        }

        private void HitMeteor(Meteor meteor)
        {
            meteor.Radius -= Damage;
            if (Radius < 0)
                meteor.DoRemoveMe();
        }
    }
}
