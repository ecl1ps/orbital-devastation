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

namespace Orbit.Server
{
    static class ServerSceneObjectFactory
    {
        public static Asteroid CreateNewRandomAsteroid(ServerMgr mgr, bool headingRight)
        {
            Rect actionArea = mgr.GetOrbitArea();
            Random randomGenerator = mgr.GetRandomGenerator();
            Asteroid s = new Asteroid(null);
            s.Id = IdMgr.GetNewId(0);
            s.IsHeadingRight = headingRight;
            s.Direction = headingRight ? new Vector(1, 0) : new Vector(-1, 0);
            s.AsteroidType = AsteroidType.NORMAL;

            s.Radius = randomGenerator.Next(SharedDef.MIN_ASTEROID_RADIUS, SharedDef.MAX_ASTEROID_RADIUS);
            s.Gold = s.Radius / 2;
            if (mgr.GetRandomGenerator().Next(100) <= SharedDef.ASTEROID_GOLD_CHANCE)
            {
                s.Gold *= SharedDef.GOLD_ASTEROID_BONUS_MULTIPLY;
                s.AsteroidType = AsteroidType.GOLDEN;
            }
            s.Position = new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius)));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));
            s.TextureId = mgr.GetRandomGenerator().Next(1, s.Gold > 0 ? 6 : 18);
            s.Rotation = mgr.GetRandomGenerator().Next(360);

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

    }
}
