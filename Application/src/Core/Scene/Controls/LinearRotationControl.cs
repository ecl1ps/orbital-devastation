using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public class LinearRotationControl : Control
    {
        private IRotable meRotable;

        public float RotationSpeed { get; set; }

        public override void InitControl(ISceneObject obj)
        {
            if (!(obj is IRotable))
            {
                Console.Error.WriteLine("LinearRotationControl cannot be attached to non-rotable object!");
                return;
            }

            meRotable = obj as IRotable;
        }

        public override void UpdateControl(float tpf)
        {
            if (meRotable == null)
                return;

            meRotable.Rotation = (int)((meRotable.Rotation + RotationSpeed * tpf * 100) % 360);
        }
    }
}
