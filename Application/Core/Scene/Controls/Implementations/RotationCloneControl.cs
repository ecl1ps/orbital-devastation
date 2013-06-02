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
        private IRotable cloned;

        public RotationCloneControl(ISceneObject cloned)
        {
            if (!(cloned is IRotable))
                throw new ArgumentException("RotationCloneControl must clone an IRotable object");

            this.cloned = cloned as IRotable;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is IRotable))
                throw new ArgumentException("RotationCloneControl must by attached to an IRotable object");

            (me as IRotable).Rotation = cloned.Rotation;
        }

        protected override void UpdateControl(float tpf)
        {
            (me as IRotable).Rotation = cloned.Rotation;
        }
    }
}
