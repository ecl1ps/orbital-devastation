using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Lidgren.Network;
using Orbit.Core;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System.Windows;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class SingularityBullet : Sphere, ISendable, IProjectile
    {
        public Player Owner { get; set; } // neposilan
        public int Damage { get; set; }

        public SingularityBullet(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.PROJECTILES;
        }

        public virtual void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public virtual void ReadObject(NetIncomingMessage msg)
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
