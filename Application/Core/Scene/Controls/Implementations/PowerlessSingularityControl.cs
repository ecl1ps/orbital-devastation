using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class PowerlessSingularityControl : DroppingSingularityControl
    {
        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);
            StartDetonation();
        }

        public override void DoCollideWith(Orbit.Core.Scene.Entities.ISceneObject other, float tpf)
        {
            return;
        }
    }
}
