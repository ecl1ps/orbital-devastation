﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using Orbit.Core.Scene.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Orbit.Core.Scene.Controls.Health.Implementations;
using Orbit.Core.Scene.Entities.Implementations.HeavyWeight;

namespace Orbit.Core.Helpers
{
    static class SceneObjectFactory
    {
        public static Base CreateBase(SceneMgr mgr, Player plr)
        {
            Base baze = new Base(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            baze.Owner = plr;
            baze.BasePosition = plr.Data.PlayerPosition;
            baze.Color = plr.Data.PlayerColor;
            baze.Position = new Vector(plr.GetBaseLocation().X, plr.GetBaseLocation().Y);
            baze.Size = new Size(plr.GetBaseLocation().Width, plr.GetBaseLocation().Height);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = new Vector(baze.Center.X, baze.Position.Y + 2.5 * baze.Size.Height);
            cs.Radius = (int)(baze.Size.Width / 1.6);
            baze.CollisionShape = cs;

            BaseHealthControl hc = new BaseHealthControl();
            baze.AddControl(hc);

            baze.AddControl(new BaseCollisionControl());

            baze.LoadImages();
            baze.SetGeometry(baze.Image100);

            return baze;
        }

        public static SingularityMine CreatePowerlessMine(SceneMgr mgr, Vector pos, Vector dir, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            mine.Position = pos;
            mine.Owner = plr;
            mine.Radius = 2;
            mine.Direction = dir;
            mine.Color = Colors.BlueViolet;
            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));
                
            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = mine.Center;
            cs.Radius = mine.Radius;
            mine.CollisionShape = cs;

            PowerlessSingularityControl sc = new PowerlessSingularityControl();
            sc.Speed = plr.Data.MineGrowthSpeed;
            sc.Strength = plr.Data.MineStrength;
            mine.AddControl(sc);

            mine.AddControl(new StickySphereCollisionShapeControl());
            return mine;
        }

        public static SingularityMine CreateDroppingSingularityMine(SceneMgr mgr, Point point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            mine.Position = new Vector(point.X, 0);
            mine.Owner = plr;
            mine.Radius = 2;
            mine.Direction = new Vector(0, 1);
            mine.Color = Colors.BlueViolet;

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = mine.Center;
            cs.Radius = mine.Radius;
            mine.CollisionShape = cs;

            DroppingSingularityControl sc = new DroppingSingularityControl();
            sc.Speed = plr.Data.MineGrowthSpeed;
            sc.Strength = plr.Data.MineStrength;
            mine.AddControl(sc);

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.Speed = plr.Data.MineFallingSpeed;
            mine.AddControl(lmc);

            mine.AddControl(new StickySphereCollisionShapeControl());

            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));

            return mine;
        }

        public static SingularityMine CreateAsteroidDroppingSingularityMine(SceneMgr mgr, Point point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            mine.Position = new Vector(point.X, 0);
            mine.Owner = plr;
            mine.Radius = 2;
            mine.Direction = new Vector(0, 1);
            mine.Color = Colors.BlueViolet;

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = mine.Center;
            cs.Radius = mine.Radius;
            mine.CollisionShape = cs;

            AsteroidDroppingSingularityControl sc = new AsteroidDroppingSingularityControl();
            sc.Speed = plr.Data.MineGrowthSpeed;
            sc.Strength = plr.Data.MineStrength;
            mine.AddControl(sc);

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.Speed = plr.Data.MineFallingSpeed;
            mine.AddControl(lmc);

            mine.AddControl(new StickySphereCollisionShapeControl());

            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));

            return mine;
        }

        public static SingularityBullet CreateSingularityBullet(SceneMgr mgr, Point point, Player plr)
        {
            Vector position = new Vector(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            SingularityBullet bullet = new SingularityBullet(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            bullet.Position = position;
            bullet.Owner = plr;
            bullet.Radius = 2;
            bullet.Damage = plr.Data.BulletDamage;
            bullet.Direction = direction;
            bullet.Color = plr.Data.PlayerColor;

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = bullet.Center;
            bullet.CollisionShape = cs;

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.Speed = plr.Data.BulletSpeed;
            bullet.AddControl(lmc);

            bullet.AddControl(new SingularityBulletCollisionReactionControl());
            bullet.AddControl(new StickyPointCollisionShapeControl());

            bullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(bullet));

            return bullet;
        }

        public static Hook CreateHook(SceneMgr mgr, Point point, Player player)
        {
            Vector position = new Vector(player.GetBaseLocation().X + player.GetBaseLocation().Width / 2, player.GetBaseLocation().Y - 5);
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            Hook hook = new Hook(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            hook.Owner = player;
            hook.Radius = 8;
            position.X -= hook.Radius;
            position.Y -= hook.Radius;
            hook.Position = position;
            hook.Rotation = (float)Vector.AngleBetween(new Vector(0, -1), direction);
            hook.Direction = direction;
            hook.Color = player.GetPlayerColor();

            hook.SetGeometry(SceneGeometryFactory.CreateHookHead(hook));

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Radius = hook.Radius / 2;
            cs.Center = hook.Center;
            hook.CollisionShape = cs;

            HookControl hookControl = new HookControl();
            hookControl.Origin = new Vector(hook.Center.X, hook.Center.Y);
            hookControl.Speed = player.Data.HookSpeed;
            hookControl.Lenght = player.Data.HookLenght;

            hook.AddControl(hookControl);
            hook.AddControl(new StickySphereCollisionShapeControl());

            hook.PrepareLine();
            
            return hook;
        }

        public static PowerHook CreatePowerHook(SceneMgr mgr, Point point, Player player)
        {
            Vector position = new Vector(player.GetBaseLocation().X + player.GetBaseLocation().Width / 2, player.GetBaseLocation().Y - 5);
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            PowerHook hook = new PowerHook(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            hook.Owner = player;
            hook.Radius = 8;
            position.X -= hook.Radius;
            position.Y -= hook.Radius;
            hook.Position = position;
            hook.Rotation = (float)Vector.AngleBetween(new Vector(0, -1), direction);
            hook.Direction = direction;
            hook.Color = Colors.RoyalBlue;

            hook.SetGeometry(SceneGeometryFactory.CreateHookHead(hook));

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Radius = hook.Radius / 2;
            cs.Center = hook.Center;
            hook.CollisionShape = cs;

            PowerHookControl hookControl = new PowerHookControl();
            hookControl.Origin = new Vector(hook.Center.X, hook.Center.Y);
            hookControl.Speed = player.Data.HookSpeed;
            hookControl.Lenght = player.Data.HookLenght;

            hook.AddControl(hookControl);
            hook.AddControl(new StickySphereCollisionShapeControl());

            hook.PrepareLine();

            return hook;
        }

        public static MinorAsteroid CreateSmallAsteroid(SceneMgr mgr, Vector direction, Vector center, int rot, int textureId, int radius,float speed, double rotation)
        {
            MinorAsteroid asteroid = new MinorAsteroid(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            asteroid.AsteroidType = AsteroidType.SPAWNED;
            asteroid.Rotation = rot;
            asteroid.Direction = direction.Rotate(rotation);
            asteroid.Radius = radius;
            asteroid.Position = center;
            asteroid.Gold = radius * 2;
            asteroid.TextureId = textureId;
            asteroid.Enabled = true;
            asteroid.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(asteroid));

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = asteroid.Center;
            cs.Radius = asteroid.Radius;
            asteroid.CollisionShape = cs;

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.Speed = speed;
            asteroid.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = mgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            asteroid.AddControl(lrc);

            asteroid.AddControl(new MinorAsteroidCollisionReactionControl());
            asteroid.AddControl(new StickySphereCollisionShapeControl());

            return asteroid;
        }

        /*public static VectorLine CreateVectorLine(SceneMgr mgr, Vector origin, Vector vector, Color color, ISceneObject parent = null)
        {
            VectorLine l = new VectorLine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            l.Position = origin;
            l.Direction = vector;
            l.Color = color;

            if (parent != null)
            {
                VectorLineObjectMovementControl c = new VectorLineObjectMovementControl();
                c.Parent = parent;
                l.AddControl(c);
            }

            //l.SetGeometry(SceneGeometryFactory.CreateLineGeometry(l));

            return l;
        }*/

        /*public static Circle CreateCircle(SceneMgr mgr, Vector point, Color color)
        {
            Circle c = new Circle(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            c.Position = point;
            c.Radius = 4;
            c.Color = color;

            c.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(c));

            mgr.BeginInvoke(new Action(() =>
            {
                Canvas.SetZIndex(c.GetGeometry(), 500);
            }));

            return c;
        }*/

        public static SingularityExplodingBullet CreateSingularityExploadingBullet(SceneMgr mgr, Vector point, Player plr)
        {
            Vector position = new Vector(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);

            return CreateSingularityExploadingBullet(mgr, point, position, plr);
        }

        public static SingularityExplodingBullet CreateSingularityExploadingBullet(SceneMgr mgr, Vector point, Vector position, Player plr)
        {
            Vector direction = point - position;
            direction.Normalize();

            SingularityExplodingBullet bullet = new SingularityExplodingBullet(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            InitSingularityBullet(bullet, mgr, point, position, plr);

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = bullet.Center;
            bullet.CollisionShape = cs;

            ExplodingSingularityBulletControl c = new ExplodingSingularityBulletControl();
            c.Speed = SharedDef.BULLET_EXPLOSION_SPEED;
            c.Strength = SharedDef.BULLET_EXPLOSION_STRENGTH;
            bullet.AddControl(c);

            bullet.AddControl(new StickyPointCollisionShapeControl());

            return bullet;
        }

        public static void InitSingularityBullet(SingularityBullet bullet, SceneMgr mgr, Vector point, Vector position, Player plr)
        {
            Vector direction = point - position;
            direction.Normalize();

            bullet.Position = position;
            bullet.Owner = plr;
            bullet.Radius = 2;
            bullet.Damage = plr.Data.BulletDamage;
            bullet.Direction = direction;
            bullet.Direction.Normalize();
            bullet.Color = plr.Data.PlayerColor;

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.Speed = plr.Data.BulletSpeed;
            bullet.AddControl(lmc);

            bullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(bullet));
        }

        public static SingularityBouncingBullet CreateSingularityBouncingBullet(SceneMgr mgr, Point point, Player plr)
        {
            Vector position = new Vector(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);
            Vector direction = point.ToVector() - position;
            direction.Normalize();

            SingularityBouncingBullet bullet = new SingularityBouncingBullet(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            bullet.Position = position;
            bullet.Owner = plr;
            bullet.Radius = 2;
            bullet.Damage = plr.Data.BulletDamage;
            bullet.Direction = direction;
            bullet.Direction.Normalize();
            bullet.Color = plr.Data.PlayerColor;

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = bullet.Center;
            bullet.CollisionShape = cs;

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.Speed = plr.Data.BulletSpeed;
            bullet.AddControl(lmc);

            BouncingSingularityBulletControl c = new BouncingSingularityBulletControl();
            c.Speed = SharedDef.BULLET_EXPLOSION_SPEED;
            c.Strength = SharedDef.BULLET_EXPLOSION_STRENGTH;
            bullet.AddControl(c);

            bullet.AddControl(new StickyPointCollisionShapeControl());

            bullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(bullet));

            return bullet;
        }

        public static MiningModule CreateMiningModule(SceneMgr mgr, Vector position, Player owner)
        {
            MiningModule module = new MiningModule(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()), owner);
            module.Position = position;
            module.Radius = 10;
            module.Color = Colors.Crimson;


            ModuleDamageControl mc = new ModuleDamageControl();
            mc.MaxHp = SharedDef.SPECTATOR_MAX_HP;
            module.AddControl(mc);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = module.Center;
            cs.Radius = module.Radius;
            module.CollisionShape = cs;

            ControlableDeviceControl dc = new ControlableMiningModuleControl();
            module.AddControl(dc);

            LinearRotationControl rc = new LinearRotationControl();
            rc.RotationSpeed = SharedDef.SPECTATOR_MODULE_ROTATION_SPEED;
            module.AddControl(rc);

            HpRegenControl hc = new HpRegenControl();
            hc.MaxRegenTime = SharedDef.SPECTATOR_HP_REGEN_CD;
            hc.RegenTimer = SharedDef.SPECTATOR_HP_REGEN_CD;
            hc.RegenSpeed = SharedDef.SPECTATOR_REGEN_SPEED;
            module.AddControl(hc);

            module.AddControl(new RespawningObjectControl());
            module.AddControl(new StickySphereCollisionShapeControl());

            module.SetGeometry(SceneGeometryFactory.CrateMiningModule(module));

            return module;
        }

        public static PercentageEllipse CreatePercentageEllipse(SceneMgr mgr, Player owner)
        {
            PercentageEllipse arc = new PercentageEllipse(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            Color c = owner.GetPlayerColor();
            c.A = 0x55;
            arc.Color = c;
            arc.FullAngle = (float) Math.PI;
            arc.A = (float)owner.Baze.Size.Width / 2 + 2;
            arc.B = (float)owner.Baze.Size.Height + 3; //hack kvuli neeliptickym rozmerum baze

            arc.Position = owner.Baze.Position + (new Vector(owner.Baze.Size.Width / 2, owner.Baze.Size.Height));
            arc.StartPoint = new Point(0, owner.Baze.Size.Height + 3);

            arc.SetGeometry(SceneGeometryFactory.CreateArcSegments(arc));

            return arc;
        }

        public static PercentageArc CreatePercentageArc(SceneMgr mgr, MiningModule module, Player owner)
        {
            PercentageArc arc = new PercentageArc(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            arc.Color = owner.GetPlayerColor();
            arc.Radius = module.Radius + 5;

            PositionCloneControl pControl = new PositionCloneControl(module);
            arc.AddControl(pControl);

            HpBarControl hControl = new HpBarControl(arc);
            module.AddControl(hControl);

            arc.SetGeometry(SceneGeometryFactory.CreateArcSegments(arc));

            return arc;
        }

        public static OrbitEllipse CreateOrbitEllipse(SceneMgr mgr, Vector position, float radiusX, float radiusY)
        {
            OrbitEllipse ellipse = new OrbitEllipse(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()), radiusX, radiusY);
            ellipse.Position = position;
            ellipse.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(ellipse));

            return ellipse;
        }

        public static SphereField CreateSphereField(SceneMgr mgr, Vector position, int radius, Color color) 
        {
            SphereField f = new SphereField(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            f.Radius = radius;
            f.Color = color;
            f.Position = position;
            f.HeavyWeightGeometry = HeavyweightGeometryFactory.CreateConstantColorEllipseGeometry(f);

            SphereCollisionShape shape = new SphereCollisionShape();
            shape.Radius = radius;
            shape.Center = f.Center;
            f.CollisionShape = shape;

            return f;
        }

        public static IceSquare CreateIceSquare(SceneMgr mgr, Vector position, Size size)
        {
            IceSquare s = new IceSquare(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            s.Size = size;
            s.Position = position;
            s.Category = Gui.Visuals.DrawingCategory.BACKGROUND;
            s.SetGeometry(SceneGeometryFactory.CreateIceCube(s));

            return s;
        }

        public static IceShard CreateIceShard(SceneMgr mgr, Vector position, Size size, int texture)
        {
            IceShard s = new IceShard(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            s.Size = size;
            s.Position = position;
            s.TextureId = texture;
            s.SetGeometry(SceneGeometryFactory.CreateIceShard(s));

            return s;
        }
    }
}
