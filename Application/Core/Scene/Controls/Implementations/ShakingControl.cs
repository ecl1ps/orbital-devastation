using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ShakingControl : Control
    {
        private bool perpendicular;
        private IMovementControl control;
        private Vector2 dir;
        private float strength;
        private float speed;

        public ShakingControl(float strength, bool perpendicularToDirection = false, float speed = 0.1f)
        {
            perpendicular = perpendicularToDirection;
            this.strength = strength;
            this.speed = speed;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (perpendicular && !(me is IMovable))
                throw new ArgumentException("ShakingControl must be attached an IMovable object or it has to have perpendicularToDirection flag set to false");

            if (perpendicular)
            {
                control = me.GetControlOfType<IMovementControl>();
                if (control != null)
                    dir = control.RealDirection;
                else
                    dir = (me as IMovable).Direction;
            }
            else
                dir = new Vector2(0, 1);
            
            events.AddEvent(1, new Event(speed, EventType.REPEATABLE, new Action(() => DoShake())));
        }

        protected override void UpdateControl(float tpf)
        {
            if (perpendicular)
            {
                control = me.GetControlOfType<IMovementControl>();
                if (control != null)
                    dir = control.RealDirection;
                else
                    dir = (me as IMovable).Direction;
            }
        }

        private void DoShake()
        {
            me.Position += (dir.Normal() * strength);
            strength *= -1;
        }
    }
}
