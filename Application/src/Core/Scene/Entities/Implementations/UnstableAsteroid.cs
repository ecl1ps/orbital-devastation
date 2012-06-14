﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;
using Orbit.Core;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class UnstableAsteroid : Asteroid
    {
        public UnstableAsteroid(SceneMgr mgr) : base(mgr)
        {
        }

        private void spawnSmallMeteors(int radius)
        {
            createSmallAsteroid(radius, Math.PI / 12);
            createSmallAsteroid(radius, 0);
            createSmallAsteroid(radius, -Math.PI / 12);
        }

        private void createSmallAsteroid(int radius, double rotation)
        {
            Asteroid asteroid = new MinorAsteroid(SceneMgr);
            asteroid.AsteroidType = AsteroidType.SPAWNED;
            asteroid.Rotation = Rotation;
            asteroid.Direction = Direction.Rotate(rotation);
            asteroid.Radius = radius;
            asteroid.Position = Position;
            asteroid.TextureId = SceneMgr.GetRandomGenerator().Next(1, 18);
            asteroid.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(asteroid));

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = 1;
            asteroid.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = 1;
            asteroid.AddControl(lrc);

            SceneMgr.AttachToScene(asteroid);
        }

        public override void doDamage(int damage)
        {
            base.doDamage(damage);
            spawnSmallMeteors(Radius);
        }
    }
}
