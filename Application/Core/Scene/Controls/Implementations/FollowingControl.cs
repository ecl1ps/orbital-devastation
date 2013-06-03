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
        public float RealSpeed { get { return Speed; } }
        private Vector move;
        public Vector RealDirection { get { return move; } }

        public FollowingControl(ISceneObject toFollow)
        {
            this.toFollow = toFollow;
            if (toFollow is IMovable)
                move = (toFollow as IMovable).Direction;
            else
                move = (me as IMovable).Direction;
        }

        protected override void UpdateControl(float tpf)
        {
            move = toFollow.Center - me.Center;
            double distToTarget = move.Length;
            move.Normalize();

            Vector newMovemenet = move * Speed * tpf;
            me.Position += newMovemenet.Length < distToTarget ? newMovemenet : move * distToTarget;
        }
    }
}
