using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public class LinearMovementControl : Control
    {
        private IMovable meMovable;

        public float Speed { get; set; }

        public override void InitControl(ISceneObject obj)
        {
            if (!(obj is IMovable))
            {
                Console.Error.WriteLine("LinearMovementControl cannot be attached to non-movable object!");
                return;
            }

            meMovable = obj as IMovable;
        }

        public override void UpdateControl(float tpf)
        {
            if (meMovable == null)
                return;

            me.Position = me.Position + (meMovable.Direction * Speed * tpf);
        }
    }
}
