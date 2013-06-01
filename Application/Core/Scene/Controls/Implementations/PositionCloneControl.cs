using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class PositionCloneControl : Control
    {
        private ISceneObject toFollow;

        public PositionCloneControl(ISceneObject toFollow)
        {
            this.toFollow = toFollow;
        }

        protected override void InitControl(ISceneObject me)
        {
            me.Position = toFollow.Position;
        }

        protected override void UpdateControl(float tpf)
        {
            me.Position = toFollow.Position;
        }
    }
}
