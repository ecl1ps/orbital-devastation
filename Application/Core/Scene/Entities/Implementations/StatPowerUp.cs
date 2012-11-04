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
using System.Windows.Media;
using System.Windows.Controls;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class StatPowerUp : Square, IMovable, IRotable, ISendable, ICatchable
    {
        public Vector Direction { get; set; }
        public float Rotation { get; set; }
        public DeviceType PowerUpType { get; set; }

        public StatPowerUp(SceneMgr mgr) : base(mgr) { }

        protected override void UpdateGeometricState()
        {
            geometryElement.RenderTransform = new RotateTransform(Rotation);
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
            cs.Size = Size;
            cs.Rotation = Rotation;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Orbit.Core.Scene.Controls.Control c in controls)
                AddControl(c);
        }
    }
}
