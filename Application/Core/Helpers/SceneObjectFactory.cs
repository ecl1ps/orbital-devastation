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
            baze.Position = new Vector(plr.GetBaseLocation().X, plr.GetBaseLocation().Y);
            baze.Size = new Size(plr.GetBaseLocation().Width, plr.GetBaseLocation().Height);

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
            Vector position = new Vector(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);
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
            Vector position = new Vector(player.GetBaseLocation().X + player.GetBaseLocation().Width / 2, player.GetBaseLocation().Y - 5);
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

        public static PowerHook CreatePowerHook(SceneMgr mgr, Point point, Player player)
        {
            Vector position = new Vector(player.GetBaseLocation().X + player.GetBaseLocation().Width / 2, player.GetBaseLocation().Y - 5);
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            PowerHook hook = new PowerHook(mgr);
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

        public static VectorLine CreateVectorLine(SceneMgr mgr, Vector origin, Vector vector, Color color, ISceneObject parent = null)
        {
            VectorLine l = new VectorLine(mgr);
            l.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
            l.Position = origin;
            l.Direction = vector;
            l.Color = color;

            if (parent != null)
            {
                VectorLineObjectMovementControl c = new VectorLineObjectMovementControl();
                c.Parent = parent;
                l.AddControl(c);
            }

            l.SetGeometry(SceneGeometryFactory.CreateLineGeometry(l));

            return l;
        }

        public static Circle CreateCircle(SceneMgr mgr, Vector point, Color color)
        {
            Circle c = new Circle(mgr);
            c.Id = IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId());
            c.Position = point;
            c.Radius = 4;
            c.Color = color;

            c.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(c));

            mgr.BeginInvoke(new Action(() =>
            {
                Canvas.SetZIndex(c.GetGeometry(), 500);
            }));

            return c;
        }

        public static SingularityExplodingBullet CreateSingularityExploadingBullet(SceneMgr mgr, Point point, Player plr)
        {
            Vector position = new Vector(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);

            return CreateSingularityExploadingBullet(mgr, point, position, plr);
        }

        public static SingularityExplodingBullet CreateSingularityExploadingBullet(SceneMgr mgr, Point point, Vector position, Player plr)
        {
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            SingularityExplodingBullet bullet = new SingularityExplodingBullet(mgr);
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

            FiringSingularityControl c = new FiringSingularityControl();
            c.Speed = SharedDef.BULLET_EXPLOSION_SPEED;
            c.Strength = SharedDef.BULLET_EXPLOSION_STRENGTH;
            bullet.AddControl(c);

            bullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(bullet));

            return bullet;
        }

        public static void InitSingularityBullet(SingularityBullet bullet, SceneMgr mgr, Point point, Vector position, Player plr)
        {
            Vector direction = point.ToVector() - position;
            direction.Normalize();

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
        }

        public static SingularityBouncingBullet CreateSingularityBouncingBullet(SceneMgr mgr, Point point, Player plr)
        {
            Vector position = new Vector(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            SingularityBouncingBullet bullet = new SingularityBouncingBullet(mgr);
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

            FiringSingularityControl c = new FiringSingularityControl();
            c.Speed = SharedDef.BULLET_EXPLOSION_SPEED;
            c.Strength = SharedDef.BULLET_EXPLOSION_STRENGTH;
            bullet.AddControl(c);

            bullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(bullet));

            return bullet;
        }

        public static MiningModule CreateMiningModule(SceneMgr mgr, Vector position, Player owner)
        {
            MiningModule module = new MiningModule(mgr);
            module.Position = position;
            module.Radius = 10;
            module.Color = Colors.Crimson;

            MiningModuleControl mc = new MiningModuleControl();
            mc.Owner = owner;
            module.AddControl(mc);

            ControlableDeviceControl dc = new ControlableDeviceControl();
            module.AddControl(dc);

            module.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(module));

            return module;
        }
    }
}
