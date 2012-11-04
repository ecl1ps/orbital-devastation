using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LinearMovementControl : Control, IMovementControl
    {
        private IMovable meMovable;
        public float Speed { get; set; }

        protected override void InitControl(ISceneObject obj)
        {
            if (!(obj is IMovable))
            {
                Console.Error.WriteLine("LinearMovementControl cannot be attached to non-movable object!");
                return;
            }

            meMovable = obj as IMovable;
        }

        protected override void UpdateControl(float tpf)
        {
            if (meMovable == null)
                return;

            me.Position += (meMovable.Direction * Speed * tpf);
        }
    }
}
