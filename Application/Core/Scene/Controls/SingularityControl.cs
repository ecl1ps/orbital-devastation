using System;
using Orbit.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public class SingularityControl : Control
    {
        private float Strength { get; set; }
        private float Speed { get; set; }

        private void Grow()
        {
            throw new Exception("Not implemented");
        }

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
