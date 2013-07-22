using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Weapons;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Helpers;
using Microsoft.Xna.Framework;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class StatPowerUp : TexturedSquare, IMovable, ISendable, ICatchable
    {
        public DeviceType PowerUpType { get; set; }

        public StatPowerUp(SceneMgr mgr, long id) 
            : base(mgr, id) 
        {
            Category = DrawingCategory.LOOTABLES;
            scale = 0.4f;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_STAT_POWERUP);
            msg.WriteObjectStatPowerUp(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectStatPowerUp(this);

            SquareCollisionShape cs = new SquareCollisionShape();
            cs.Position = Position;
            cs.Rectangle = Rectangle;
            cs.Rotation = Rotation;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
