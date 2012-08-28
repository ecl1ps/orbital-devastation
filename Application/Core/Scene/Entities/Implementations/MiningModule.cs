using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class MiningModule : Sphere, IRotable
    {
        public float Rotation { get; set; }

        public MiningModule(SceneMgr mgr)
            : base(mgr)
        {
        }

        protected override void UpdateGeometricState()
        {
            base.UpdateGeometricState();
            geometryElement.RenderTransform = new RotateTransform(Rotation);
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            //i dont want to be destroyed when moving off screen
            return true;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            (GetControlOfType(typeof(MiningModuleControl)) as MiningModuleControl).Collide(other);
        }
    }
}
