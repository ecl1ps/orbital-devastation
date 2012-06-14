using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class MinorAsteroid : Asteroid
    {
        public MinorAsteroid(SceneMgr mgr) : base(mgr)
        {
        }

        public override void OnRemove()
        {
            //do nothing
        }
    }
}
