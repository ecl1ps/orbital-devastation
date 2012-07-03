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

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class StatPowerUp : Square, IMovable, IDestroyable, IRotable, ISendable
    {
        public Vector Direction { get; set; }
        public float Rotation { get; set; }
        public DeviceType PowerUpType { get; set; }

        public StatPowerUp(SceneMgr mgr) : base(mgr) { }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is Base)
            {
                SceneMgr.StatsMgr.OnPlayerCaughtPowerUp((other as Base).Owner, PowerUpType);
                DoRemoveMe();
            }
        }

        public override void UpdateGeometric()
        {
            geometryElement.Dispatcher.Invoke(new Action(() =>
            {
                Canvas.SetLeft(geometryElement, Position.X);
                Canvas.SetTop(geometryElement, Position.Y);
                if (geometryElement is Image)
                    (geometryElement as Image).RenderTransform = new RotateTransform(Rotation);
            }));
        }

        public void TakeDamage(int damage, ISceneObject from)
        {
            DoRemoveMe();
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
            IList<IControl> controls = msg.ReadControls();
            foreach (Orbit.Core.Scene.Controls.Control c in controls)
                AddControl(c);
        }
    }
}
