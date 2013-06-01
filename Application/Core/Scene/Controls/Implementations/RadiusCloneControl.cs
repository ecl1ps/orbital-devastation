using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class RadiusCloneControl : Control
    {
        private ISpheric cloned;

        public RadiusCloneControl(ISceneObject cloned)
        {
            if (!(cloned is ISpheric))
                throw new ArgumentException("RadiusCloneControl must clone an ISpheric object");

            this.cloned = cloned as ISpheric;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is ISpheric))
                throw new ArgumentException("RadiusCloneControl must by attached to an ISpheric object");

            (me as ISpheric).Radius = cloned.Radius;
        }

        protected override void UpdateControl(float tpf)
        {
            (me as ISpheric).Radius = cloned.Radius;
        }
    }
}
