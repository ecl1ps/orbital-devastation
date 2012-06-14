using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Lidgren.Network;
using Orbit.Core;
using Orbit.src.Core.Scene.Entities;

namespace Orbit.Core.Scene.Entities
{
    class SingularityBullet : Sphere, ISendable
    {

        public Player Player { get; set; } // neposilan
        public int Damage { get; set; }

        public SingularityBullet(SceneMgr mgr) : base(mgr)
        {
        }

        protected override void UpdateGeometricState()
        {
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is IDestroyable)
            {
                HitAsteroid(other as IDestroyable);
                DoRemoveMe();
            }
        }

        private void HitAsteroid(IDestroyable asteroid)
        {
            asteroid.doDamage(Damage);
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityBullet(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
