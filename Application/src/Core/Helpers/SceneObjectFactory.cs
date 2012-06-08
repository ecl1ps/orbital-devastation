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

namespace Orbit.Core.Scene
{
    static class SceneObjectFactory
    {
        public static Asteroid CreateNewRandomAsteroid(ISceneMgr mgr, bool headingRight)
        {
            Rect actionArea = mgr.GetOrbitArea();
            Random randomGenerator = mgr.GetRandomGenerator();
            Asteroid s = new Asteroid(mgr);
            s.Id = IdMgr.GetNewId();
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

            CreateAsteroidControls(s);

            s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));

            return s;
        }

        private static Asteroid CreateAsteroidControls(Asteroid s)
        {
            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = s.SceneMgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_SPEED * 10, SharedDef.MAX_ASTEROID_SPEED * 10) / 10.0f;
            s.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = s.SceneMgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            s.AddControl(lrc);

            return s;
        }

        public static Base CreateBase(ISceneMgr mgr, PlayerData data)
        {
            Base baze = new Base(mgr);
            baze.Id = IdMgr.GetNewId();
            baze.BasePosition = data.PlayerPosition;
            baze.Color = data.PlayerColor;
            baze.Integrity = SharedDef.BASE_MAX_INGERITY;
            baze.Position = PlayerPositionProvider.GetVectorPosition(data.PlayerPosition);
            baze.Size = new Size(mgr.ViewPortSizeOriginal.Width * 0.3, mgr.ViewPortSizeOriginal.Height * 0.15);

            baze.SetGeometry(SceneGeometryFactory.CreateLinearGradientRectangleGeometry(baze));

            return baze;
        }

        public static Asteroid CreateNewAsteroidOnEdge(ISceneMgr mgr, Asteroid oldSphere)
        {
            Asteroid s = CreateNewRandomAsteroid(mgr, oldSphere.IsHeadingRight);

            Rect actionArea = mgr.GetOrbitArea();

            s.Position = new Vector(s.IsHeadingRight ? (int)(- 2 * s.Radius) : (int)(actionArea.Width),
                mgr.GetRandomGenerator().Next((int)(actionArea.Y), (int)(actionArea.Height - 2 * s.Radius)));

            mgr.Invoke(new Action(() =>
            {
                Canvas.SetLeft(s.GetGeometry(), s.Position.X);
                Canvas.SetTop(s.GetGeometry(), s.Position.Y);
            }));

            return s;
        }

        public static SingularityMine CreateSingularityMine(ISceneMgr mgr, Point point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr);
            mine.Id = IdMgr.GetNewId();
            mine.Position = point.ToVector();
            mine.Owner = plr;
            mine.FillBrush = new RadialGradientBrush(Colors.Black, Color.FromRgb(0x66, 0x00, 0x80));

            SingularityControl sc = new SingularityControl();
            sc.Speed = plr.Data.MineGrowthSpeed;
            sc.Strength = plr.Data.MineStrength;
            mine.AddControl(sc);

            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));

            return mine;
        }

        public static SingularityMine CreateDroppingSingularityMine(ISceneMgr mgr, Point point, Player plr)
        {

            SingularityMine mine = new SingularityMine(mgr);
            mine.Id = IdMgr.GetNewId();
            mine.Position = new Vector(point.X, 0);
            mine.Owner = plr;
            mine.Radius = 2;
            mine.Direction = new Vector(0, 1);

            DroppingSingularityControl sc = new DroppingSingularityControl();
            sc.Speed = plr.Data.MineGrowthSpeed;
            sc.Strength = plr.Data.MineStrength;
            mine.AddControl(sc);

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.InitialSpeed = plr.Data.MineFallingSpeed;
            mine.AddControl(lmc);

            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));

            return mine;
        }

        public static SingularityBullet CreateSingularityBullet(ISceneMgr mgr, Point point, Player plr)
        {
            Vector position = PlayerPositionProvider.GetVectorPosition(plr.Baze.BasePosition);
            position.X += (plr.Baze.Size.Width / 2);
            Vector direction = point.ToVector() - position;
            direction.Normalize();


            SingularityBullet bullet = new SingularityBullet(mgr);
            bullet.Id = IdMgr.GetNewId();
            bullet.Position = position;
            bullet.Player = plr;
            bullet.Radius = 2;
            bullet.Damage = plr.Data.BulletDamage;
            bullet.Direction = direction;
            bullet.Direction.Normalize();
            bullet.Color = plr.Data.PlayerColor;

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.InitialSpeed = plr.Data.BulletSpeed;
            bullet.AddControl(lmc);

            bullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(bullet));

            return bullet;
        }

        public static Hook CreateHook(ISceneMgr mgr, Point point, Player player)
        {
            Vector position = PlayerPositionProvider.GetVectorPosition(player.Baze.BasePosition);
            position.X += (player.Baze.Size.Width / 2);
            position.Y -= 5;
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            Hook hook = new Hook(mgr);
            hook.Player = player;
            hook.Position = position;
            hook.Radius = 8;
            hook.Rotation = (float)Vector.AngleBetween(new Vector(0, -1), direction);
            hook.Direction = direction;
            hook.Color = player.GetPlayerColor();

            hook.SetGeometry(SceneGeometryFactory.CreateHookHead(hook));
            mgr.BeginInvoke(new Action(() =>
            {
                Canvas.SetZIndex(hook.GetGeometry(), 99);
            }));

            HookControl hookControl = new HookControl();
            hookControl.Origin = new Vector(hook.Center.X, hook.Center.Y - hook.Radius);
            hookControl.Speed = player.Data.HookSpeed;
            hookControl.Lenght = player.Data.HookLenght;

            hook.AddControl(hookControl);

            hook.PrepareLine();
            
            return hook;
        }
    }
}
