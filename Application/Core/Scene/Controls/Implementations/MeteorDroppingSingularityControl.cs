 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class AsteroidDroppingSingularityControl : DroppingSingularityControl
    {
        private IList<IMovable> collided = new List<IMovable>();
        private bool detonated = false;

        public override void CollidedWith(IMovable movable)
        {
            foreach (IMovable obj in collided)
                if (obj.Id == movable.Id)
                    return;

            collided.Add(movable);

            base.CollidedWith(movable);
        }

        public override void StartDetonation()
        {
            if (detonated || GetDistToExplosionPct() > 0)
                return;

            base.StartDetonation();
            if ((me as SingularityMine).Owner.IsCurrentPlayer())
                spawnAsteroid();
        }

        private void spawnAsteroid()
        {
            Asteroid ast = new Asteroid(me.SceneMgr);
            ast.Radius = 10;
            ast.Position = me.Position;
            ast.Direction = (me as IMovable).Direction;

            ast.AsteroidType = AsteroidType.NORMAL;
            ast.TextureId = 1;

            ast.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(ast));

            me.SceneMgr.DelayedAttachToScene(ast);

            collided.Add(ast);
            detonated = true;

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = 1;
            ast.AddControl(nmc);

            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            ast.WriteObject(msg);

            me.SceneMgr.SendMessage(msg);
        }
    }
}
