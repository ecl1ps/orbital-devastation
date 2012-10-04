using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class PositionCloneControl : Control
    {
        public Vector Offset { get; set; }

        private ISceneObject toFollow;

        public PositionCloneControl(ISceneObject toFollow)
        {
            this.toFollow = toFollow;
        }

        public override void InitControl(ISceneObject me)
        {
        }

        public override void UpdateControl(float tpf)
        {
            if(Offset == null)
                Offset = new Vector(0, 0);

            Vector position = toFollow.Position - Offset;
            me.Position = position;
        }
    }
}
