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

        public void SpawnNewBullet(ISceneObject collidedWith)
        {
            SingularityExplodingBullet bullet = new SingularityExplodingBullet(SceneMgr, IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId()));

            List<ISceneObject> nearbyObjects = FindNearbyObjects<Asteroid>(200);
            nearbyObjects.Remove(collidedWith);
            IOrderedEnumerable<ISceneObject> ordered = nearbyObjects.OrderBy(o => (Center - o.Center).Length);

            Vector targettedPosition = AIUtils.ComputeDestinationPositionToHitTarget(ordered.FirstOrDefault(), Owner.Data.BulletSpeed, Center, SceneMgr.GetRandomGenerator());
            SceneObjectFactory.InitSingularityBullet(bullet, SceneMgr, targettedPosition, Position, Owner);

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = bullet.Center;
            bullet.CollisionShape = cs;

            ExcludingExplodingSingularityBulletControl c = new ExcludingExplodingSingularityBulletControl();
            c.StatReported = true;
            c.Speed = SharedDef.BULLET_EXPLOSION_SPEED;
            c.Strength = SharedDef.BULLET_EXPLOSION_STRENGTH;
            c.IgnoredObjects.Add(collidedWith.Id);
            bullet.AddControl(c);

            bullet.AddControl(new StickyPointCollisionShapeControl());

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            (bullet as ISendable).WriteObject(msg);
            SceneMgr.SendMessage(msg);

            SceneMgr.DelayedAttachToScene(bullet);
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
