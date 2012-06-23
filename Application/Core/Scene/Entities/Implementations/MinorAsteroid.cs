using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class MinorAsteroid : Asteroid
    {
        public UnstableAsteroid Parent { get; set; }
        private Player lastHitTakenFrom;

        public MinorAsteroid(SceneMgr mgr) : base(mgr)
        {
        }

        public override void TakeDamage(int damage, ISceneObject from)
        {
            base.TakeDamage(damage, from);
            if (from is SingularityBullet)
                lastHitTakenFrom = (from as SingularityBullet).Owner;
        }

        public override void OnRemove()
        {
            if (lastHitTakenFrom != null)
                Parent.NoticeChildAsteroidDestroyedBy(lastHitTakenFrom, this);
        }

        public override void DoCollideWith(ICollidable other)
        {
            base.DoCollideWith(other);
            if (!(other is SingularityBullet))
                lastHitTakenFrom = null;
        }

        public new void WriteObject(NetOutgoingMessage msg)
        {
            msg.WriteObjectMinorAsteroid(this);
            msg.WriteControls(GetControlsCopy());
        }

        public new void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectMinorAsteroid(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        } 
    }
}
