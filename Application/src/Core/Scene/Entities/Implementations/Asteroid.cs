using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.src.Core.Scene.Entities;

namespace Orbit.Core.Scene.Entities
{
    public enum AsteroidType
    {
        NORMAL,
        GOLDEN,
        UNSTABLE,
    }

    public class Asteroid : Sphere, IRotable, ISendable, IContainsGold, IDestroyable
    {
        public bool IsHeadingRight { get; set; }
        public float Rotation { get; set; }
        public int TextureId { get; set; }
        public int Gold { get; set; }
        public AsteroidType AsteroidType { get; set; }

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
                SceneMgr.AttachToScene(SceneObjectFactory.CreateNewAsteroidOnEdge(this));
                return;
            }

            if (SceneMgr.IsServer())
            {
                Asteroid a = SceneObjectFactory.CreateNewAsteroidOnEdge(this);
                SceneMgr.AttachToScene(a);
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                a.WriteObject(msg);
                SceneMgr.SendMessage(msg);
            }
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_ASTEROID);
            msg.WriteObjectAsteroid(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectAsteroid(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }

        public virtual void doDamage(int damage)
        {
            Radius -= damage;
            if (Radius < SharedDef.ASTEROID_THRESHOLD_RADIUS)
            {
                Radius = 0;
                DoRemoveMe();
            }
        }
    }

}
