using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class FollowingControl : Control, IMovementControl
    {
        private ISceneObject toFollow;

        public float Speed { get; set; }

        public FollowingControl(ISceneObject toFollow)
        {
            this.toFollow = toFollow;
        }

        protected override void UpdateControl(float tpf)
        {
            Vector newDir = toFollow.Center - me.Center;
            double distToTarget = newDir.Length;
            newDir.Normalize();

            Vector newMovemenet = newDir * Speed * tpf;
            me.Position += newMovemenet.Length < distToTarget ? newMovemenet : newDir * distToTarget;
        }
    }
}
