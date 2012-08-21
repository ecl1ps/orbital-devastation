using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class StaticShield : Square
    {
        public Color Color { get; set; }

        public StaticShield(SceneMgr mgr)
            : base(mgr)
        {
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is IDestroyable)
                other.DoRemoveMe();
        }
    }
}
