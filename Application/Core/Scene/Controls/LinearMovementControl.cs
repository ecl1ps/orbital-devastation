using System;
using System.Windows;
using Orbit.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public class LinearMovementControl : Control
    {
        private IMovable me;

        public float Speed { get; set; }

        public override void InitControl(ISceneObject obj)
        {
            if (!(me is IMovable))
            {
                Console.Error.WriteLine("LinearMovementControl cannot be attached to non-movable object!");
                return;
            }

            me = (IMovable)obj;
        }

        public override void UpdateControl(float tpf)
        {
            if (me == null)
                return;

            ((ISceneObject)me).SetPosition(((ISceneObject)me).GetPosition() + (me.GetDirection() * Speed));
        }
    }
}
