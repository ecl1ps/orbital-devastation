using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Particles.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class EmmitorDirectionCloneControl : Control
    {
        private IMovable cloned;
        private IMovementControl control;

        public float DirectionOfsetRotation { get; set; }

        public EmmitorDirectionCloneControl(ISceneObject cloned)
        {
            if (!(cloned is IMovable))
                throw new ArgumentException("DirectionCloneControl must clone an IMovable object");

            this.cloned = cloned as IMovable;
            DirectionOfsetRotation = 0;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is ParticleEmmitor))
                throw new ArgumentException("EmmitorDirectionCloneControl must by attached to an ParticleEmmitor object");

            control = cloned.GetControlOfType<IMovementControl>();
            Vector direction = control != null ? control.RealDirection : cloned.Direction;

            if (DirectionOfsetRotation > 0)
                direction = direction.Rotate(DirectionOfsetRotation);

            (me as ParticleEmmitor).EmmitingDirection = direction;
        }

        protected override void UpdateControl(float tpf)
        {
            control = cloned.GetControlOfType<IMovementControl>();
            Vector direction = control != null ? control.RealDirection : cloned.Direction;

            if (DirectionOfsetRotation > 0)
                direction = direction.Rotate(DirectionOfsetRotation);

            (me as ParticleEmmitor).EmmitingDirection = direction;
        }
    }
}
