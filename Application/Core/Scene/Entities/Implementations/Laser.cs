using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class Laser : SolidLine, IProjectile
    {
        public Player Owner { get; set; }

        public Laser(Player owner, SceneMgr mgr, Point start, Point end, Color color, Brush brush, int width) 
            : base(mgr, start, end, color, brush, width)
        {
            Owner = owner;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is IDestroyable)
                (other as IDestroyable).TakeDamage((int) (SharedDef.BULLET_DMG * tpf), this);
        }
    }
}
