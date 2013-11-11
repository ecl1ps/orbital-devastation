using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Lidgren.Network;
using System.Windows;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Orbit.Core.AI;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class SingularityBouncingBullet : SingularityExplodingBullet
    {
        public SingularityBouncingBullet(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_BOUNCING_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public override void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityBullet(this);

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = Center;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
