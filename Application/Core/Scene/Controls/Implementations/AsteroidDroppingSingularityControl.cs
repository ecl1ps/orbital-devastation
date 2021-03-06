﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class AsteroidDroppingSingularityControl : DroppingSingularityControl
    {
        private IList<long> collided = new List<long>();

        public bool Detonated { get; set; }
        public bool AsteroidSpawned { get; set; }

        public AsteroidDroppingSingularityControl()
            : base()
        {
            Detonated = false;
            AsteroidSpawned = false;
        }

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            // jinak by mohly miny kolidovat navzajem a spawnovat dalsi a dalsi powerless miny
            if (other is SingularityMine)
                return;

            if (collided.Contains(other.Id))
                return;

            collided.Add(other.Id);

            base.DoCollideWith(other, tpf);

            SingularityMine mine = SceneObjectFactory.CreatePowerlessMine(me.SceneMgr, me.Position, meMine.Direction, meMine.Owner);
            collided.Add(mine.Id);
            me.SceneMgr.DelayedAttachToScene(mine);
        }

        public override void StartDetonation()
        {
            if (Detonated || GetDistToExplosionPct() > 0)
                return;

            base.StartDetonation();
            Detonated = true;
            HighlightingControl c = me.GetControlOfType<HighlightingControl>();
            if (c != null)
                c.Enabled = false;
        }

        public void SpawnAsteroid()
        {
            if (AsteroidSpawned)
                return;

            AsteroidSpawned = true;
            me.RemoveControlsOfType<HighlightingControl>();

            Asteroid ast = new Asteroid(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));
            ast.Gold = 0;
            ast.Radius = 20;
            ast.Position = new Vector(me.Position.X - ast.Radius, me.Position.Y - ast.Radius);
            ast.Direction = (me as IMovable).Direction;

            ast.AsteroidType = AsteroidType.NORMAL;
            ast.TextureId = me.SceneMgr.GetRandomGenerator().Next(1, 18);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = ast.Center;
            cs.Radius = ast.Radius;
            ast.CollisionShape = cs;

            ast.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(ast));

            me.SceneMgr.DelayedAttachToScene(ast);

            collided.Add(ast.Id);

            ast.AddControl(new StickySphereCollisionShapeControl());

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.Speed = me.SceneMgr.GetCurrentPlayer().Data.MineFallingSpeed;
            ast.AddControl(nmc);

            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            ast.WriteObject(msg);

            me.SceneMgr.SendMessage(msg);
        }
    }
}
