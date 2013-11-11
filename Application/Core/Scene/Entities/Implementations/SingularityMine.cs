using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using System.Collections.Generic;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class SingularityMine : TexturedSphere, ISendable, IProjectile
    {
        public Player Owner { get; set; } // neposilan

        public SingularityMine(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_MINE);
            msg.WriteObjectSingularityMine(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityMine(this);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = Center;
            cs.Radius = Radius;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
