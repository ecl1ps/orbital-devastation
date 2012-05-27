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

        protected override void UpdateGeometricState()
        {
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is Meteor)
            {
                (other as Meteor).DoRemoveMe();
                DoRemoveMe();
            }
        }
    }
}
