using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;

namespace Orbit.Core.Scene.Entities
{

    public class Meteor : Sphere, IMovable, ICollidable, IRotable, ISendable
    {
        public bool IsHeadingRight { get; set; }
        public float Rotation { get; set; }
        public int TextureId { get; set; }

        public Meteor()
        {
        }

        protected override void UpdateGeometricState()
        {
            (geometryElement as Image).RenderTransform = new RotateTransform(Rotation);
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (!(other is Meteor))
                DoRemoveMe();
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
            msg.WriteObjectSphere(this);

            IList<IControl> controls = GetControlsCopy();
            msg.Write(controls.Count);
            foreach (IControl c in controls)
            {
                if (c is NewtonianMovementControl)
                {
                    msg.Write(typeof(NewtonianMovementControl).GUID.GetHashCode());
                    msg.WriteObjectNewtonianMovementControl(c as NewtonianMovementControl);
                }
                else if (c is LinearMovementControl)
                {
                    msg.Write(typeof(LinearMovementControl).GUID.GetHashCode());
                    msg.WriteObjectLinearMovementControl(c as LinearMovementControl);
                }
                else if (c is LinearRotationControl)
                {
                    msg.Write(typeof(LinearRotationControl).GUID.GetHashCode());
                    msg.WriteObjectLinearRotationControl(c as LinearRotationControl);
                }
                else
                    Console.Error.WriteLine("Sending Sphere with unspported control!");
            }
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSphere(this);

            int controls = msg.ReadInt32();
            for (int i = 0; i < controls; ++i)
            {
                int hash = msg.ReadInt32();
                if (hash == typeof(NewtonianMovementControl).GUID.GetHashCode())
                {
                    NewtonianMovementControl c = new NewtonianMovementControl();
                    msg.ReadObjectNewtonianMovementControl(c);
                    AddControl(c);
                }
                else if (hash == typeof(LinearMovementControl).GUID.GetHashCode())
                {
                    LinearMovementControl c = new LinearMovementControl();
                    msg.ReadObjectLinearMovementControl(c);
                    AddControl(c);
                }
                else if (hash == typeof(LinearRotationControl).GUID.GetHashCode())
                {
                    LinearRotationControl c = new LinearRotationControl();
                    msg.ReadObjectLinearRotationControl(c);
                    AddControl(c);
                }
                else
                    Console.Error.WriteLine("Reading Sphere with unspported control guid!");
            }
        }
    }

}
