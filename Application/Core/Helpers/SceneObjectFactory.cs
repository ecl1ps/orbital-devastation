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
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Helpers
{
    static class SceneObjectFactory
    {

        public static Base CreateBase(SceneMgr mgr, Player plr)
        {
            Base baze = new Base(mgr);
            baze.Owner = plr;
            baze.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
            baze.BasePosition = plr.Data.PlayerPosition;
            baze.Color = plr.Data.PlayerColor;
            baze.Position = plr.VectorPosition;
            baze.Size = new Size(mgr.ViewPortSize.Width * 0.3, mgr.ViewPortSize.Height * 0.15);

            baze.SetGeometry(SceneGeometryFactory.CreateLinearGradientRectangleGeometry(baze));

            return baze;
        }

        public static SingularityMine CreateSingularityMine(SceneMgr mgr, Point point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr);
            mine.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
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

        public static SingularityMine CreateDroppingSingularityMine(SceneMgr mgr, Point point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr);
            mine.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
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

        public static SingularityBullet CreateSingularityBullet(SceneMgr mgr, Point point, Player plr)
        {
            Vector position = plr.VectorPosition;
            position.X += (plr.Baze.Size.Width / 2);
            Vector direction = point.ToVector() - position;
            direction.Normalize();


            SingularityBullet bullet = new SingularityBullet(mgr);
            bullet.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
            bullet.Position = position;
            bullet.Owner = plr;
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

        public static Hook CreateHook(SceneMgr mgr, Point point, Player player)
        {
            Vector position = player.VectorPosition;
            position.X += (player.Baze.Size.Width / 2);
            position.Y -= 5;
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            Hook hook = new Hook(mgr);
            hook.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
            hook.Owner = player;
            hook.Radius = 8;
            position.X -= hook.Radius;
            position.Y -= hook.Radius;
            hook.Position = position;
            hook.Rotation = (float)Vector.AngleBetween(new Vector(0, -1), direction);
            hook.Direction = direction;
            hook.Color = player.GetPlayerColor();

            hook.SetGeometry(SceneGeometryFactory.CreateHookHead(hook));
            mgr.BeginInvoke(new Action(() =>
            {
                Canvas.SetZIndex(hook.GetGeometry(), 99);
            }));

            HookControl hookControl = new HookControl();
            hookControl.Origin = new Vector(hook.Center.X, hook.Center.Y);
            hookControl.Speed = player.Data.HookSpeed;
            hookControl.Lenght = player.Data.HookLenght;

            hook.AddControl(hookControl);

            hook.PrepareLine();
            
            return hook;
        }

        public static MinorAsteroid CreateSmallAsteroid(SceneMgr mgr, long id, Vector direction, Vector center, int rot, int textureId, int radius, double rotation)
        {
            MinorAsteroid asteroid = new MinorAsteroid(mgr);
            asteroid.AsteroidType = AsteroidType.SPAWNED;
            asteroid.Id = id;
            asteroid.Rotation = rot;
            asteroid.Direction = direction.Rotate(rotation);
            asteroid.Radius = radius;
            asteroid.Position = center;
            asteroid.Gold = radius * 2;
            asteroid.TextureId = textureId;
            asteroid.Enabled = true;
            asteroid.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(asteroid));

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = 1;
            asteroid.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = mgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            asteroid.AddControl(lrc);

            return asteroid;
        }
    }
}
