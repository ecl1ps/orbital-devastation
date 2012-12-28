 using System;
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
    class AsteroidDroppingSingularityControl : DroppingSingularityControl
    {
        private IList<long> collided = new List<long>();

        public bool Detonated { get; set; }
        public bool SpawnActivated { get; set; }

        public AsteroidDroppingSingularityControl()
            : base()
        {
            Detonated = false;
            SpawnActivated = false;
        }

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if (collided.Contains(other.Id))
                return;

            collided.Add(other.Id);

            base.DoCollideWith(other, tpf);
            me.SceneMgr.Invoke(new Action(() => {
                SingularityMine mine = SceneObjectFactory.CreatePowerlessMine(me.SceneMgr, me.Position, meMine.Direction, meMine.Owner);
                collided.Add(mine.Id);
                me.SceneMgr.DelayedAttachToScene(mine);
            }));
        }

        public override void StartDetonation()
        {
            if (Detonated || GetDistToExplosionPct() > 0)
                return;

            if ((me as SingularityMine).Owner.IsCurrentPlayer() && SpawnActivated)
                SpawnAsteroid();

            base.StartDetonation();
            Detonated = true;
        }

        private void SpawnAsteroid()
        {
            Asteroid ast = new Asteroid(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));
            ast.Radius = 10;
            ast.Position = new Vector(me.Position.X - ast.Radius, me.Position.Y - ast.Radius);
            ast.Direction = (me as IMovable).Direction;

            ast.AsteroidType = AsteroidType.NORMAL;
            ast.TextureId = 1;

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
