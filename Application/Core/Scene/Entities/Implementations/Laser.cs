using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class Laser : SolidLine, IProjectile
    {

        public Player Owner { get; set; }
        private LaserDamageControl damageControl;

        public Laser(Player owner, SceneMgr mgr, Point start, Point end, Color color, Brush brush, int width) 
            : base(mgr, start, end, color, brush, width)
        {
            Owner = owner;
        }

        public void initControl(LaserDamageControl control)
        {
            damageControl = control;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is IDestroyable)
                DoDamage(other as IDestroyable);
        }

        private void DoDamage(IDestroyable enemy)
        {
            if (damageControl == null)
                throw new Exception("Laser must have LaserDamageControl attached");

            damageControl.HitObject(enemy);
        }
    }
}
