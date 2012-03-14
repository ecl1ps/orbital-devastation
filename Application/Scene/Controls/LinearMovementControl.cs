using System;
using Orbit.Scene.Entities;

namespace Orbit.Scene.Controls
{
    public class LinearMovementControl : Control
    {
        public float Speed { get; set; }

        public override void InitControl(ISceneObject me)
        {
            throw new Exception("Not implemented");
        }

        public override void UpdateControl(float tpf)
        {
            throw new Exception("Not implemented");
        }
    }
}
