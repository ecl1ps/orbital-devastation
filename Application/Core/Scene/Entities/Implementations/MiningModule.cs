using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class MiningModule : Sphere
    {
        public MiningModule(SceneMgr mgr)
            : base(mgr)
        {
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
