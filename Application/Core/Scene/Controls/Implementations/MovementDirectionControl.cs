using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class MovementDirectionControl : Control
    {
        private bool initiated = false;
        private Vector2 lastPosition;
        private IMovable obj;

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);

            if (me is IMovable)
                obj = me as IMovable;
            else
                throw new Exception("MovementDirectionControl must be attached to IMovement object");
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            if (!initiated)
            {
                lastPosition = me.Position;
                initiated = true;
                return;
            }

            obj.Direction = lastPosition - me.Position;
            lastPosition = me.Position;
        }
    }
}
