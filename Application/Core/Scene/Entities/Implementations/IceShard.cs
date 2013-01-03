using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class IceShard : IceSquare, IMovable, IRotable
    {
        public float Rotation { get; set; }
        public System.Windows.Vector Direction { get; set; }
        public int TextureId { get; set; }

        public IceShard(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        protected override void UpdateGeometricState()
        {
            base.UpdateGeometricState();
            geometryElement.RenderTransform = new RotateTransform(Rotation);
        }
    }
}
