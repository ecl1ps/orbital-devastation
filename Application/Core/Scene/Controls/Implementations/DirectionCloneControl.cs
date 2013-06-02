using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class DirectionCloneControl : Control
    {
        private IMovable cloned;
        private IMovementControl control;

        public DirectionCloneControl(ISceneObject cloned)
        {
            if (!(cloned is IMovable))
                throw new ArgumentException("DirectionCloneControl must clone an IMovable object");

            this.cloned = cloned as IMovable;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is IMovable))
                throw new ArgumentException("DirectionCloneControl must by attached to an IMovable object");

            control = cloned.GetControlOfType<IMovementControl>();
            if (control != null)
                (me as IMovable).Direction = control.RealDirection;
            else
                (me as IMovable).Direction = cloned.Direction;
        }

        protected override void UpdateControl(float tpf)
        {
            control = cloned.GetControlOfType<IMovementControl>();
            if (control != null)
                (me as IMovable).Direction = control.RealDirection;
            else
                (me as IMovable).Direction = cloned.Direction;
        }
    }
}
