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
        public static Asteroid CreateNewRandomAsteroid(bool headingRight)
        {
            Rect actionArea = SceneMgr.GetInstance().GetOrbitArea();
            Random randomGenerator = SceneMgr.GetInstance().GetRandomGenerator();
            Asteroid s = new Asteroid();
            s.Id = IdMgr.GetNewId();
            s.IsHeadingRight = headingRight;
            s.Direction = headingRight ? new Vector(1, 0) : new Vector(-1, 0);
            s.AsteroidType = AsteroidType.NORMAL;

            s.Radius = randomGenerator.Next(SharedDef.MIN_ASTEROID_RADIUS, SharedDef.MAX_ASTEROID_RADIUS);
            s.Gold = s.Radius / 2;
            if (SceneMgr.GetInstance().GetRandomGenerator().Next(100) <= SharedDef.ASTEROID_GOLD_CHANCE)
            {
                s.Gold *= SharedDef.GOLD_ASTEROID_BONUS_MULTIPLY;
                s.AsteroidType = AsteroidType.GOLDEN;
            }
            s.Position = new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius)));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));
            s.TextureId = SceneMgr.GetInstance().GetRandomGenerator().Next(1, s.Gold > 0 ? 6 : 18);
            s.Rotation = SceneMgr.GetInstance().GetRandomGenerator().Next(360);

            CreateAsteroidControls(s);

            s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));

            return s;
        }

        private static Asteroid CreateAsteroidControls(Asteroid s)
        {
            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = SceneMgr.GetInstance().GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_SPEED * 10, SharedDef.MAX_ASTEROID_SPEED * 10) / 10.0f;
            s.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = SceneMgr.GetInstance().GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            s.AddControl(lrc);

            return s;
        }

        public static Base CreateBase(PlayerData data)
        {
            Base baze = new Base();
            baze.Id = IdMgr.GetNewId();
            baze.BasePosition = data.PlayerPosition;
            baze.Color = data.PlayerColor;
            baze.Integrity = SharedDef.BASE_MAX_INGERITY;
            baze.Position = PlayerPositionProvider.getVectorPosition(data.PlayerPosition);
            baze.Size = new Size(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * 0.3, 
                                 SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.15);

            baze.SetGeometry(SceneGeometryFactory.CreateLinearGradientRectangleGeometry(baze));

            return baze;
        }

        public static Asteroid CreateNewAsteroidOnEdge(Asteroid oldSphere)
        {
            Asteroid s = CreateNewRandomAsteroid(oldSphere.IsHeadingRight);

            Rect actionArea = SceneMgr.GetInstance().GetOrbitArea();

            s.Position = new Vector(s.IsHeadingRight ? (int)(- 2 * s.Radius) : (int)(actionArea.Width),
                SceneMgr.GetInstance().GetRandomGenerator().Next((int)(actionArea.Y), (int)(actionArea.Height - 2 * s.Radius)));

            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Canvas.SetLeft(s.GetGeometry(), s.Position.X);
                Canvas.SetTop(s.GetGeometry(), s.Position.Y);
            }));

            return s;
        }

        public static SingularityMine CreateSingularityMine(Point point, Player plr)
        {
            SingularityMine mine = new SingularityMine();
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

        public static SingularityMine CreateDroppingSingularityMine(Point point, Player plr)
        {

            SingularityMine mine = new SingularityMine();
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

        public static SingularityBullet CreateSingularityBullet(Point point, Player plr)
        {
            Vector position = PlayerPositionProvider.getVectorPosition(plr.Baze.BasePosition);
            position.X += (plr.Baze.Size.Width / 2);
            Vector direction = point.ToVector() - position;
            direction.Normalize();


            SingularityBullet bullet = new SingularityBullet();
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

        public static Hook CreateHook(Point point, Player player)
        {
            Vector position = PlayerPositionProvider.getVectorPosition(player.Baze.BasePosition);
            position.X += (player.Baze.Size.Width / 2);
            position.Y -= 5;
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            Hook hook = new Hook();
            hook.Player = player;
            hook.Position = position;
            hook.Radius = 8;
            hook.Rotation = (float)Vector.AngleBetween(new Vector(0, -1), direction);
            hook.Direction = direction;
            hook.Color = player.GetPlayerColor();

            hook.SetGeometry(SceneGeometryFactory.CreateHookHead(hook));
            SceneMgr.GetInstance().GetUIDispatcher().BeginInvoke(DispatcherPriority.Send, new Action(() =>
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
