using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ExcludingExplodingSingularityBulletControl : ExplodingSingularityBulletControl
    {
        public List<long> IgnoredObjects = new List<long>();

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if (!CanCollideWithObject(other))
                return;

            foreach (long id in IgnoredObjects)
                if (other.Id == id)
                    return;

            base.DoCollideWith(other, tpf);
        }
    }
}
