﻿using System;
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
            SceneObjectFactory.InitSingularityBullet(bullet, SceneMgr, GetComputedPoint(collidedWith), Position, Owner);

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = bullet.Center;
            bullet.CollisionShape = cs;

            ExcludingExplodingSingularityBulletControl c = new ExcludingExplodingSingularityBulletControl();
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

        private double GetRandomRotation()
        {
            int b = Owner.SceneMgr.GetRandomGenerator().Next(2);
            double d = Owner.SceneMgr.GetRandomGenerator().NextDouble() * 3.14;

            return b == 0 ? d : -d;
        }

        private Vector GetComputedPoint(ISceneObject except)
        {
            Asteroid nearest = GetNearestAsteroid(except);
            Vector random = Direction.Rotate(GetRandomRotation());

            if (nearest == null)
                return random;

            // rychlost bulletu
            double v1 = Owner.Data.BulletSpeed;
            // rychlost objektu
            IMovementControl mc = nearest.GetControlOfType<IMovementControl>();
            double v2 = mc == null ? 0 : mc.Speed;
            // vektor od objketu k launcheru hooku
            Vector cVec = Position - nearest.Center;
            // vektor smeru objektu
            Vector dVec = (nearest as IMovable).Direction;

            // vzdalenost mezi nabojem a objektem
            double c = cVec.Length;
            // cosinus uhlu, ktery sviraji vektory pohybu objektu a smeru k launcheru
            double cosAlpha = (cVec.X * dVec.X + cVec.Y * dVec.Y) / (cVec.Length * dVec.Length);

            // diskriminant pro kvadratickou rovnici cosinovy vety
            double D = Math.Pow(2 * c * cosAlpha, 2) - 4 * c * c * (1 - Math.Pow(v1 / v2, 2));
            // nebyl nalezen trojuhelnik (komplexni cisla)
            if (D < 0)
                return random;

            double sqrtD = Math.Sqrt(D);

            // kvadraticka rovnice cosinovy vety
            // odectenim D ziskame bod pred telesem, prictenim bychom ziskali bod za telesem (ve smeru jeho pohybu)
            double x1 = (2 * c * cosAlpha - sqrtD) / (2 - 2 * Math.Pow(v1 / v2, 2));

            dVec.Normalize();

            return nearest.Center + (dVec * x1);
        }

        private Asteroid GetNearestAsteroid(ISceneObject except)
        {
            ISceneObject nearest = null;
            double nearestDistSqr = 10000000;
            double objDistantSqr = 10000000;

            List<ISceneObject> objects = Owner.SceneMgr.GetSceneObjects();

            foreach (ISceneObject obj in objects)
            {
                if (!(obj is Asteroid) || !SceneMgr.IsPointInViewPort(obj.Position.ToPoint()) || obj.Id == except.Id)
                    continue;

                objDistantSqr = (obj.Position - Position).LengthSquared;
                if (objDistantSqr < nearestDistSqr)
                {
                    nearestDistSqr = objDistantSqr;
                    nearest = obj;
                }
            }
            return nearest == null ? null : nearest as Asteroid;
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
