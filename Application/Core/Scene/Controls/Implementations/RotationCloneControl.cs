using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class RotationCloneControl : Control
    {
        private ISceneObject cloned;

        public RotationCloneControl(ISceneObject cloned)
        {
            this.cloned = cloned;
        }

        protected override void InitControl(ISceneObject me)
        {
            me.Rotation = cloned.Rotation;
        }

        protected override void UpdateControl(float tpf)
        {
            me.Rotation = cloned.Rotation;
        }
    }
}
