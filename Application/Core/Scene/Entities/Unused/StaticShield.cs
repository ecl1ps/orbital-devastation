using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class StaticShield : Sphere
    {
        public StaticShield(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }
    }
}
