using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ExcludingExplodingSingularityBulletControl : ExplodingSingularityBulletControl
    {
        public List<long> IgnoredObjects = new List<long>();

        public override void DoCollideWith(Entities.ISceneObject other, float tpf)
        {
            foreach (long id in IgnoredObjects)
                if (other.Id == id)
                    return;

            base.DoCollideWith(other, tpf);
        }
    }
}
