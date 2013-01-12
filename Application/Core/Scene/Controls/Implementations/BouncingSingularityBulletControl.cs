using System;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Lidgren.Network;
using Orbit.Core.AI;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class BouncingSingularityBulletControl : ExplodingSingularityBulletControl
    {
        public override void HitAsteroid(IDestroyable asteroid)
        {
            base.HitAsteroid(asteroid);
            if (me.SceneMgr.GetCurrentPlayer().IsActivePlayer())
                EmitBulletDueToCollision(asteroid);
        }

        private void EmitBulletDueToCollision(ISceneObject collidedWith)
        {
            List<ISceneObject> nearbyObjects = me.FindNearbyObjects<Asteroid>(200);
            nearbyObjects.Remove(collidedWith);
            IOrderedEnumerable<ISceneObject> ordered = nearbyObjects.OrderBy(o => (me.Center - o.Center).Length);

            SpawnNewBullet(ordered.FirstOrDefault(), collidedWith);
        }

        private void SpawnNewBullet(ISceneObject target, ISceneObject ignored = null)
        {
            SingularityExplodingBullet bullet = new SingularityExplodingBullet(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));

            Vector targettedPosition = AIUtils.ComputeDestinationPositionToHitTarget(target, meBullet.Owner.Data.BulletSpeed, me.Center, me.SceneMgr.GetRandomGenerator());
            SceneObjectFactory.InitSingularityBullet(bullet, me.SceneMgr, targettedPosition, me.Position, meBullet.Owner);

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = bullet.Center;
            bullet.CollisionShape = cs;

            ExcludingExplodingSingularityBulletControl c = new ExcludingExplodingSingularityBulletControl();
            c.Speed = SharedDef.BULLET_EXPLOSION_SPEED;
            c.Strength = SharedDef.BULLET_EXPLOSION_STRENGTH;
            c.StatReported = true;
            if (ignored != null)
                c.IgnoredObjects.Add(ignored.Id);
            bullet.AddControl(c);

            bullet.AddControl(new StickyPointCollisionShapeControl());

            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            (bullet as ISendable).WriteObject(msg);
            me.SceneMgr.SendMessage(msg);

            me.SceneMgr.DelayedAttachToScene(bullet);
        }

        public void EmitBullets()
        {
            me.GetControlOfType<HighlightingControl>().Enabled = false;

            List<ISceneObject> nearbyObjects = me.FindNearbyObjects<Asteroid>(300);
            IEnumerable<ISceneObject> picked = nearbyObjects.OrderBy(o => (me.Center - o.Center).Length).Take(SharedDef.BULLET_ACTIVE_SHRAPNEL_COUNT);

            foreach (ISceneObject o in picked)
                SpawnNewBullet(o);
        }
    }
}
