using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;
using Orbit.Core;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class UnstableAsteroid : Asteroid
    {
        public UnstableAsteroid(SceneMgr mgr) : base(mgr)
        {
        }

        private void SpawnSmallMeteors(int radius)
        {
            CreateSmallAsteroid(radius, Math.PI / 12);
            CreateSmallAsteroid(radius, 0);
            CreateSmallAsteroid(radius, -Math.PI / 12);
        }

        private void CreateSmallAsteroid(int radius, double rotation)
        {
            Asteroid asteroid = new MinorAsteroid(SceneMgr);
            asteroid.AsteroidType = AsteroidType.SPAWNED;
            asteroid.Rotation = SceneMgr.GetRandomGenerator().Next(360);
            asteroid.Direction = Direction.Rotate(rotation);
            asteroid.Radius = radius;
            asteroid.Position = Center;
            asteroid.TextureId = SceneMgr.GetRandomGenerator().Next(1, 18);
            asteroid.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(asteroid));

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = 1;
            asteroid.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = SceneMgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            asteroid.AddControl(lrc);

            SceneMgr.DelayedAttachToScene(asteroid);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            DoRemoveMe();
            SpawnSmallMeteors((int)(Radius * 0.7f));
        }
    }
}
