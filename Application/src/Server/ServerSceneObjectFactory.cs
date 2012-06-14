using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using Orbit.Core.Scene.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Orbit.Core;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Server
{
    static class ServerSceneObjectFactory
    {
        public static Asteroid CreateNewRandomAsteroid(ServerMgr mgr, bool headingRight)
        {
            Rect actionArea = mgr.GetOrbitArea();
            Random randomGenerator = mgr.GetRandomGenerator();

            Asteroid s;
            int chance = randomGenerator.Next(100);
            if (chance <= SharedDef.ASTEROID_GOLD_CHANCE)
            {
                s = new Asteroid(null);
                s.AsteroidType = AsteroidType.GOLDEN;
                s.TextureId = randomGenerator.Next(1, 6);
                s.Gold = (s.Radius / 2) * SharedDef.GOLD_ASTEROID_BONUS_MULTIPLY;
            }
            else if (chance <= SharedDef.ASTEROID_UNSTABLE_CHANCE)
            {
                s = new UnstableAsteroid(null);
                s.AsteroidType = AsteroidType.UNSTABLE;
                s.TextureId = randomGenerator.Next(1, 6);
                s.Gold = s.Radius / 2;
            }
            else
            {
                s = new Asteroid(null);
                s.AsteroidType = AsteroidType.NORMAL;
                s.TextureId = randomGenerator.Next(1, 18);
                s.Gold = s.Radius / 2;
            }

            s.Id = IdMgr.GetNewId(0);
            s.IsHeadingRight = headingRight;
            s.Direction = headingRight ? new Vector(1, 0) : new Vector(-1, 0);

            s.Radius = randomGenerator.Next(SharedDef.MIN_ASTEROID_RADIUS, SharedDef.MAX_ASTEROID_RADIUS);

            s.Position = new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius)));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));
            s.Rotation = mgr.GetRandomGenerator().Next(360);

            CreateAsteroidControls(mgr, s);

            return s;
        }

        public static Asteroid CreateNewAsteroidOnEdge(ServerMgr mgr, bool headingRight)
        {
            Asteroid s = CreateNewRandomAsteroid(mgr, headingRight);

            Rect actionArea = mgr.GetOrbitArea();

            s.Position = new Vector(s.IsHeadingRight ? (int)(- 2 * s.Radius) : (int)(actionArea.Width),
                mgr.GetRandomGenerator().Next((int)(actionArea.Y), (int)(actionArea.Height - 2 * s.Radius)));

            return s;
        }

        private static Asteroid CreateAsteroidControls(ServerMgr mgr, Asteroid s)
        {
            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = mgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_SPEED * 10, SharedDef.MAX_ASTEROID_SPEED * 10) / 10.0f;
            s.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = mgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            s.AddControl(lrc);

            return s;
        }

    }
}
