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
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Weapons;

namespace Orbit.Core.Server
{
    static class ServerSceneObjectFactory
    {
        public static Asteroid CreateNewRandomAsteroid(ServerMgr mgr, bool headingRight)
        {
            Random randomGenerator = mgr.GetRandomGenerator();

            Asteroid s;
            int chance = randomGenerator.Next(100);
            if (chance <= SharedDef.ASTEROID_GOLD_CHANCE)
            {
                s = new Asteroid(null);
                s.AsteroidType = AsteroidType.GOLDEN;
                s.TextureId = randomGenerator.Next(1, 6);
                s.Radius = randomGenerator.Next(SharedDef.MIN_ASTEROID_RADIUS, SharedDef.MAX_ASTEROID_RADIUS);
                s.Gold = (s.Radius / 2) * SharedDef.GOLD_ASTEROID_BONUS_MULTIPLY;
            }
            else if (chance <= SharedDef.ASTEROID_UNSTABLE_CHANCE)
            {
                s = new UnstableAsteroid(null);
                s.AsteroidType = AsteroidType.UNSTABLE;
                s.TextureId = randomGenerator.Next(1, 6);
                s.Radius = randomGenerator.Next(SharedDef.MIN_ASTEROID_RADIUS, SharedDef.MAX_ASTEROID_RADIUS);
                s.Gold = s.Radius / 2;
            }
            else
            {
                s = new Asteroid(null);
                s.AsteroidType = AsteroidType.NORMAL;
                s.TextureId = randomGenerator.Next(1, 18);
                s.Radius = randomGenerator.Next(SharedDef.MIN_ASTEROID_RADIUS, SharedDef.MAX_ASTEROID_RADIUS);
                s.Gold = s.Radius / 2;
            }

            s.Id = IdMgr.GetNewId(0);
            s.IsHeadingRight = headingRight;
            s.Direction = headingRight ? new Vector(1, 0) : new Vector(-1, 0);

            s.Position = GetRandomPositionInOrbitArea(randomGenerator, s.Radius);
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));
            s.Rotation = mgr.GetRandomGenerator().Next(360);

            CreateAsteroidControls(mgr, s);

            return s;
        }

        public static Vector GetRandomPositionInOrbitArea(Random randomGenerator, int objectRadius)
        {
            return new Vector(randomGenerator.Next((int)(SharedDef.ORBIT_AREA.X + objectRadius), (int)(SharedDef.ORBIT_AREA.Width - objectRadius)),
                randomGenerator.Next((int)(SharedDef.ORBIT_AREA.Y + objectRadius), (int)(SharedDef.ORBIT_AREA.Height - objectRadius)));
        }

        public static Asteroid CreateCustomAsteroid(ServerMgr mgr, int rad, Vector pos, Vector dir)
        {
            Random randomGenerator = mgr.GetRandomGenerator();

            Asteroid s;
            s = new Asteroid(null);
            s.AsteroidType = AsteroidType.NORMAL;
            s.TextureId = randomGenerator.Next(1, 18);
            s.Radius = rad;

            s.Id = IdMgr.GetNewId(0);
            s.Direction = dir;
            s.Position = pos;
            s.Rotation = mgr.GetRandomGenerator().Next(360);

            CreateAsteroidControls(mgr, s);

            return s;
        }

        public static Asteroid CreateNewAsteroidOnEdge(ServerMgr mgr, bool headingRight)
        {
            Asteroid s = CreateNewRandomAsteroid(mgr, headingRight);

            s.Position = GetPositionOnEdgeOfOrbitArea(mgr.GetRandomGenerator(), headingRight, s.Radius);

            return s;
        }

        private static Vector GetPositionOnEdgeOfOrbitArea(Random rand, bool onLeftSide, int objectRadius)
        {
            return new Vector(onLeftSide ? (int)(-4 * objectRadius) : (int)(SharedDef.ORBIT_AREA.Width + 2 * objectRadius),
                rand.Next((int)(SharedDef.ORBIT_AREA.Y), (int)(SharedDef.ORBIT_AREA.Height - 2 * objectRadius)));
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

        public static StatPowerUp CreateStatPowerUp(ServerMgr mgr, WeaponType type)
        {
            Random randomGenerator = mgr.GetRandomGenerator();

            StatPowerUp s = new StatPowerUp(null);
            s.Id = IdMgr.GetNewId(0);
            s.Size = new Size(20, 20);
            bool headingRight = randomGenerator.Next(10) > 5 ? true : false;
            s.Position = GetPositionOnEdgeOfOrbitArea(randomGenerator, headingRight, (int)s.Size.Width / 2);
            s.Direction = headingRight ? new Vector(1, 0) : new Vector(-1, 0);
            s.Rotation = mgr.GetRandomGenerator().Next(360);

            s.PowerUpWeaponType = type;

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
