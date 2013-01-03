using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class SphereField : Sphere
    {
        public SphereField(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }
    }
}
