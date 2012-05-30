using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;

namespace Orbit.Core.Scene.Entities
{

    public class Meteor : Sphere, IRotable, ISendable
    {
        public bool IsHeadingRight { get; set; }
        public float Rotation { get; set; }
        public int TextureId { get; set; }

        public Meteor()
        {
        }

        protected override void UpdateGeometricState()
        {
            (geometryElement as System.Windows.Controls.Image).RenderTransform = new RotateTransform(Rotation);
        }

        public override void DoCollideWith(ICollidable other)
        {
        }

        public override void OnRemove()
        {
            if (SceneMgr.GameType == Gametype.SOLO_GAME)
            {
                SceneMgr.AttachToScene(SceneObjectFactory.CreateNewSphereOnEdge(this));
                return;
            }

            if (SceneMgr.IsServer())
            {
                Meteor s = SceneObjectFactory.CreateNewSphereOnEdge(this);
                SceneMgr.AttachToScene(s);
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                s.WriteObject(msg);
                SceneMgr.SendMessage(msg);
            }
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_ASTEROID);
            msg.WriteObjectMeteor(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectMeteor(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }

}
