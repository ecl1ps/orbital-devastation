using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public enum AsteroidType
    {
        NORMAL,
        GOLDEN,
        UNSTABLE,
        SPAWNED
    }

    public class Asteroid : Sphere, ISendable, IContainsGold, IDestroyable, ICatchable
    {
        public bool IsHeadingRight { get; set; }
        public int TextureId { get; set; }
        public int Gold { get; set; }
        public AsteroidType AsteroidType { get; set; }

        public Asteroid(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.ASTEROIDS;
        }

        protected override void UpdateGeometricState()
        {
            (geometryElement.Children[0] as ImageDrawing).Rect = new Rect(0, 0, Radius * 2, Radius * 2);
        }

        public override void OnRemove()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.ASTEROID_DESTROYED);
            msg.Write(Id);
            SceneMgr.SendMessage(msg);
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

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = Center;
            cs.Radius = Radius;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }

        public virtual void TakeDamage(int damage, ISceneObject from)
        {
            if (damage == 0)
                return;

            if (from is IProjectile && (from as IProjectile).Owner.IsCurrentPlayer())
                SceneMgr.FloatingTextMgr.AddFloatingText(damage, Center, FloatingTextManager.TIME_LENGTH_1, FloatingTextType.DAMAGE);

            Radius -= damage;
            Position = new Vector(Position.X + damage / 2, Position.Y + damage / 2);
            if (Radius < SharedDef.ASTEROID_THRESHOLD_RADIUS)
            {
                Radius = 0;
                DoRemoveMe();
            }
        }

        public virtual float GetHp()
        {
            return Radius;
        }
    }

}
