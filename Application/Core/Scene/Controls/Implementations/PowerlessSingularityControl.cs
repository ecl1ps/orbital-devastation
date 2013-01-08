using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class PowerlessSingularityControl : DroppingSingularityControl
    {
        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            StartDetonation();
        }

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            return;
        }
    }
}
