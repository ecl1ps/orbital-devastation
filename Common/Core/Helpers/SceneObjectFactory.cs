using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Microsoft.Xna.Framework;
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
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core.SpecialActions;

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
            baze.Position = new Vector2(plr.GetBaseLocation().X, plr.GetBaseLocation().Y);
            baze.Rectangle = new Rectangle(0, 0, plr.GetBaseLocation().Width, plr.GetBaseLocation().Height);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = new Vector2(baze.Center.X, baze.Position.Y + 2.5f * baze.Rectangle.Height);
            cs.Radius = (int)(baze.Rectangle.Width / 1.6);
            baze.CollisionShape = cs;

            BaseHealthControl hc = new BaseHealthControl();
            baze.AddControl(hc);

            baze.AddControl(new BaseCollisionControl());
            baze.Texture = SceneGeometryFactory.Base_100;

            return baze;
        }

        public static SingularityMine CreatePowerlessMine(SceneMgr mgr, Vector2 pos, Vector2 dir, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            mine.Position = pos;
            mine.Owner = plr;
            mine.Radius = 2;
            mine.Direction = dir;
            mine.Texture = SceneGeometryFactory.Mine;
                
            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = mine.Center;
            cs.Radius = mine.Radius;
            mine.CollisionShape = cs;

            PowerlessSingularityControl sc = new PowerlessSingularityControl();
            sc.Speed = plr.Data.MineGrowthSpeed;
            sc.Strength = plr.Data.MineStrength;
            sc.StatsReported = true;
            mine.AddControl(sc);

            mine.AddControl(new StickySphereCollisionShapeControl());
            return mine;
        }

        public static SingularityMine CreateDroppingSingularityMine(SceneMgr mgr, Vector2 point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            mine.Position = new Vector2(point.X, 0);
            mine.Owner = plr;
            mine.Radius = 12;
            mine.Direction = new Vector2(0, 1);

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

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = (float) FastMath.LinearInterpolate(0, Math.PI, mgr.GetRandomGenerator().NextDouble());
            mine.AddControl(lrc);

            mine.AddControl(new StickySphereCollisionShapeControl());

            mine.Texture = SceneGeometryFactory.Mine;

            return mine;
        }

        public static SingularityMine CreateAsteroidDroppingSingularityMine(SceneMgr mgr, Vector2 point, Player plr)
        {
            SingularityMine mine = new SingularityMine(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            mine.Position = new Vector2(point.X, 0);
            mine.Owner = plr;
            mine.Radius = 12;
            mine.Direction = new Vector2(0, 1);

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

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = (float)FastMath.LinearInterpolate(0, Math.PI / 2, mgr.GetRandomGenerator().NextDouble());
            mine.AddControl(lrc);

            mine.AddControl(new StickySphereCollisionShapeControl());

            mine.Texture = SceneGeometryFactory.Mine;

            return mine;
        }

        public static SingularityBullet CreateSingularityBullet(SceneMgr mgr, Vector2 point, Player plr)
        {
            Vector2 position = new Vector2(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);
            Vector2 direction = point.ToVector() - position;
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

            return bullet;
        }

        public static Hook CreateHook(SceneMgr mgr, Vector2 point, Player player)
        {
            Vector2 position = new Vector2(player.GetBaseLocation().X + player.GetBaseLocation().Width / 2, player.GetBaseLocation().Y - 5);
            Vector2 direction = point.ToVector() - position;
            direction.Normalize();

            Hook hook = new Hook(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            hook.Owner = player;
            hook.Radius = 8;
            position.X -= hook.Radius;
            position.Y -= hook.Radius;
            hook.Position = position;
            hook.Rotation = new Vector2(0, -1).AngleBetween(direction);
            hook.Direction = direction;
            hook.Color = player.GetPlayerColor();

            hook.Texture = SceneGeometryFactory.Hook;

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Radius = hook.Radius / 2;
            cs.Center = hook.Center;
            hook.CollisionShape = cs;

            HookControl hookControl = new HookControl();
            hookControl.Origin = new Vector2(hook.Center.X, hook.Center.Y);
            hookControl.Speed = player.Data.HookSpeed;
            hookControl.Lenght = player.Data.HookLenght;

            hook.AddControl(hookControl);
            hook.AddControl(new StickySphereCollisionShapeControl());

            return hook;
        }

        public static PowerHook CreatePowerHook(SceneMgr mgr, Vector2 point, Player player)
        {
            Vector2 position = new Vector2(player.GetBaseLocation().X + player.GetBaseLocation().Width / 2, player.GetBaseLocation().Y - 5);
            Vector2 direction = point.ToVector() - position;
            direction.Normalize();

            PowerHook hook = new PowerHook(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            hook.Owner = player;
            hook.Radius = 8;
            position.X -= hook.Radius;
            position.Y -= hook.Radius;
            hook.Position = position;
            hook.Rotation = new Vector2(0, -1).AngleBetween(direction);
            hook.Direction = direction;
            hook.Color = Color.RoyalBlue;

            hook.Texture = SceneGeometryFactory.Hook;

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Radius = hook.Radius / 2;
            cs.Center = hook.Center;
            hook.CollisionShape = cs;

            PowerHookControl hookControl = new PowerHookControl();
            hookControl.Origin = new Vector2(hook.Center.X, hook.Center.Y);
            hookControl.Speed = player.Data.HookSpeed;
            hookControl.Lenght = player.Data.HookLenght;

            hook.AddControl(hookControl);
            hook.AddControl(new StickySphereCollisionShapeControl());

            return hook;
        }

        public static MinorAsteroid CreateSmallAsteroid(SceneMgr mgr, Vector2 direction, Vector2 center, int rot, int textureId, int radius,float speed, float rotation)
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
            asteroid.Texture = SceneGeometryFactory.GetAsteroidTexture(asteroid);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = asteroid.Center;
            cs.Radius = asteroid.Radius;
            asteroid.CollisionShape = cs;

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.Speed = speed;
            asteroid.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = MathHelper.Lerp(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED, (float)mgr.GetRandomGenerator().NextDouble());
            asteroid.AddControl(lrc);

            asteroid.AddControl(new MinorAsteroidCollisionReactionControl());
            asteroid.AddControl(new StickySphereCollisionShapeControl());

            return asteroid;
        }

        /*public static VectorLine CreateVectorLine(SceneMgr mgr, Vector2 origin, Vector2 vector, Color color, ISceneObject parent = null)
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

        /*public static Circle CreateCircle(SceneMgr mgr, Vector2 point, Color color)
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

        public static SingularityExplodingBullet CreateSingularityExploadingBullet(SceneMgr mgr, Vector2 point, Player plr)
        {
            Vector2 position = new Vector2(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);

            return CreateSingularityExploadingBullet(mgr, point, position, plr);
        }

        public static SingularityExplodingBullet CreateSingularityExploadingBullet(SceneMgr mgr, Vector2 point, Vector2 position, Player plr)
        {
            Vector2 direction = point - position;
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

        public static void InitSingularityBullet(SingularityBullet bullet, SceneMgr mgr, Vector2 point, Vector2 position, Player plr)
        {
            Vector2 direction = point - position;
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
        }

        public static SingularityBouncingBullet CreateSingularityBouncingBullet(SceneMgr mgr, Vector2 point, Player plr)
        {
            Vector2 position = new Vector2(plr.GetBaseLocation().X + plr.GetBaseLocation().Width / 2, plr.GetBaseLocation().Y);
            Vector2 direction = point.ToVector() - position;
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

            return bullet;
        }

        public static MiningModule CreateMiningModule(SceneMgr mgr, Vector2 position, Player owner)
        {
            MiningModule module = new MiningModule(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()), owner);
            module.Position = position;
            module.Radius = 10;


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

            module.Texture = SceneGeometryFactory.MiningModule;

            return module;
        }

        public static Arc CreateMiningModuleIntegrityBar(SceneMgr mgr, MiningModule module, Player owner)
        {
            Arc arc = new Arc(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            arc.Color = owner.Data.SpecialColor;
            arc.Radius = module.Radius + 5;

            CenterCloneControl pControl = new CenterCloneControl(module);
            arc.AddControl(pControl);

            HpBarControl hControl = new HpBarControl(arc);
            module.AddControl(hControl);

            return arc;
        }

        public static Sphere CreateOrbitEllipse(SceneMgr mgr, Vector2 position, int radius, Color color)
        {
            Sphere s = new Sphere(mgr, IdMgr.GetNewId(mgr.GetCurrentPlayer().GetId()));
            s.Color = color;
            s.Position = position;
            s.Radius = radius;

            return s;
        }

        public static void CreateSpectatorActionReadinessIndicators(Player p)
        {
            List<ISpecialAction> actions = p.GetActions<ISpectatorAction>();
            for (int i = 0; i < actions.Count; ++i)
            {
                ISpectatorAction a = (ISpectatorAction)actions[i];
                float rotation = ((2 * MathHelper.Pi / actions.Count) * i) + MathHelper.Pi / (actions.Count * 2) + MathHelper.Pi;

                SpectatorActionIndicatorControl saic = new SpectatorActionIndicatorControl();
                saic.Action = a;
                saic.Indicator = SceneObjectFactory.SpectatorActionReadinessIndicator(a, p.Device, rotation);
                Color strokeColor = a.CastingColor;
                strokeColor.A = 0x60;
                saic.ExactIndicator = SceneObjectFactory.SpectatorActionReadinessIndicator(a, p.Device, rotation);

                p.SceneMgr.DelayedAttachToScene(saic.Indicator);
                p.SceneMgr.DelayedAttachToScene(saic.ExactIndicator);

                p.Device.AddControl(saic);
            }
        }

        public static ISceneObject SpectatorActionReadinessIndicator(ISpectatorAction a, MiningModule parent, float rotation)
        {
            Sphere s = new Sphere(a.SceneMgr, IdMgr.GetNewId(a.SceneMgr.GetCurrentPlayer().GetId()));
            s.Color = a.CastingColor;
            s.Radius = 6;

            Vector2 offset = new Vector2(parent.Radius + 15, 0).Rotate(rotation);

            CenterCloneControl ccc = new CenterCloneControl(parent);
            ccc.Offset = offset;
            s.AddControl(ccc);

            return s;
        }
    }
}
