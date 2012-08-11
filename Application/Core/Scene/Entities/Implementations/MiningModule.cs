using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class MiningModule : Sphere
    {
        public MiningModule(SceneMgr mgr)
            : base(mgr)
        {
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            //i dont collide with anything
        }
    }
}
