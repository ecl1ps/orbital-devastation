using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Orbit.Core.Scene.Entities;
using System.Windows;
using System.Windows.Media;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Weapons;
using Orbit.Core.Helpers;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Orbit.Core.Scene.Controls.Health.Implementations;
using Orbit.Core.Scene.Controls.Health.implementations;
using Orbit.Core.Scene.Controls.Health;
using Orbit.Core.Server.Match;
using Orbit.Core.Server;
using Orbit.Core.Server.Level;

namespace Orbit.Core.Helpers
{
    static class NetDataHelper
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // objects

        public static void WriteObjectSceneObject(this NetOutgoingMessage msg, SceneObject s)
        {
            msg.Write(s.Id);
            msg.Write(s.Dead);
            msg.Write(s.Position);
        }

        public static void ReadObjectSceneObject(this NetIncomingMessage msg, SceneObject s)
        {
            s.Id = msg.ReadInt64();
            s.Dead = msg.ReadBoolean();
            s.Position = msg.ReadVector();
        }

        public static void WriteObjectSphere(this NetOutgoingMessage msg, Sphere s)
        {
            msg.WriteObjectSceneObject(s);

            msg.Write(s.Direction);
            msg.Write(s.Radius);
            msg.Write(s.Color);
        }

        public static void ReadObjectSphere(this NetIncomingMessage msg, Sphere s)
        {
            msg.ReadObjectSceneObject(s);

            s.Direction = msg.ReadVector();
            s.Radius = msg.ReadInt32();
            s.Color = msg.ReadColor();
        }

        public static void WriteObjectAsteroid(this NetOutgoingMessage msg, Asteroid a)
        {
            msg.Write((byte)a.AsteroidType);
            msg.WriteObjectSphere(a);

            msg.Write(a.IsHeadingRight);
            msg.Write(a.Rotation);
            msg.Write(a.TextureId);
            msg.Write(a.Gold);
        }

        public static void ReadObjectAsteroid(this NetIncomingMessage msg, Asteroid a)
        {
            msg.ReadObjectSphere(a);

            a.IsHeadingRight = msg.ReadBoolean();
            a.Rotation = msg.ReadFloat();
            a.TextureId = msg.ReadInt32();
            a.Gold = msg.ReadInt32();
        }

        public static void WriteObjectMinorAsteroid(this NetOutgoingMessage msg, MinorAsteroid a)
        {
            msg.WriteObjectAsteroid(a);
            msg.Write(a.Direction);
            msg.Write(a.Parent.Id);
        }

        public static void ReadObjectMinorAsteroid(this NetIncomingMessage msg, MinorAsteroid a)
        {
            msg.ReadObjectAsteroid(a);
            a.Direction = msg.ReadVector();
        }


        public static void WriteObjectSingularityMine(this NetOutgoingMessage msg, SingularityMine s)
        {
            msg.WriteObjectSphere(s);
        }

        public static void ReadObjectSingularityMine(this NetIncomingMessage msg, SingularityMine s)
        {
            msg.ReadObjectSphere(s);
        }

        public static void WriteObjectSingularityBullet(this NetOutgoingMessage msg, SingularityBullet s)
        {
            msg.WriteObjectSphere(s);
            msg.Write(s.Damage);
        }

        public static void ReadObjectSingularityBullet(this NetIncomingMessage msg, SingularityBullet s)
        {
            msg.ReadObjectSphere(s);
            s.Damage = msg.ReadInt32();
        }

        public static void WriteObjectHook(this NetOutgoingMessage msg, Hook h)
        {
            //TYPE prvni
            msg.Write((int)h.HookType);
            msg.WriteObjectSphere(h);
            msg.Write(h.Rotation);
        }

        public static Hook ReadObjectHook(this NetIncomingMessage msg, SceneMgr mgr)
        {
            Hook h = null;
            HookType type = (HookType)msg.ReadInt32();
            switch (type)
            {
                case HookType.HOOK_NORMAL:
                    h = new Hook(mgr, -1);
                    break;
                case HookType.HOOK_POWER:
                    h = new PowerHook(mgr, -1);
                    break;

                default:
                    throw new Exception("Unrecognized hook type");
            }

            return h;
        }

        public static void ReadObjectHook(this NetIncomingMessage msg, Hook h)
        {
            msg.ReadObjectSphere(h);
            h.Rotation = msg.ReadFloat();
        }

        public static void WriteObjectSquare(this NetOutgoingMessage msg, Square s)
        {
            msg.WriteObjectSceneObject(s);

            msg.Write(s.Size.Width);
            msg.Write(s.Size.Height);
        }

        public static void ReadObjectSquare(this NetIncomingMessage msg, Square s)
        {
            msg.ReadObjectSceneObject(s);

            s.Size = new Size(msg.ReadDouble(), msg.ReadDouble());
        }

        public static void WriteObjectLine(this NetOutgoingMessage msg, Line l)
        {
            msg.WriteObjectSceneObject(l);

            msg.Write(l.Start);
            msg.Write(l.End);
        }

        public static void ReadObjectLine(this NetIncomingMessage msg, Line l)
        {
            msg.ReadObjectSceneObject(l);

            l.Start = msg.ReadVector();
            l.End = msg.ReadVector();
        }

        public static void WriteObjectStatPowerUp(this NetOutgoingMessage msg, StatPowerUp s)
        {
            msg.WriteObjectSquare(s);

            msg.Write(s.Direction);
            msg.Write(s.Rotation);
            msg.Write((byte)s.PowerUpType);
        }

        public static void ReadObjectStatPowerUp(this NetIncomingMessage msg, StatPowerUp s)
        {
            msg.ReadObjectSquare(s);

            s.Direction = msg.ReadVector();
            s.Rotation = msg.ReadFloat();
            s.PowerUpType = (DeviceType)msg.ReadByte();
        }

        public static void WriteControls(this NetOutgoingMessage msg, IList<IControl> controls)
        {
            msg.Write(controls.Count);
            foreach (IControl c in controls)
            {
                if (c is BaseCollisionControl)
                {
                    msg.Write(typeof(BaseCollisionControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is BouncingSingularityBulletControl)
                {
                    msg.Write(typeof(BouncingSingularityBulletControl).GUID.GetHashCode());
                    msg.WriteObjectExplodingSingularityBulletControl(c as ExplodingSingularityBulletControl);
                }
                else if (c is AsteroidDroppingSingularityControl)
                {
                    msg.Write(typeof(AsteroidDroppingSingularityControl).GUID.GetHashCode());
                    msg.WriteObjectDroppingSingularityControl(c as DroppingSingularityControl);
                }
                else if (c is DroppingSingularityControl)
                {
                    msg.Write(typeof(DroppingSingularityControl).GUID.GetHashCode());
                    msg.WriteObjectDroppingSingularityControl(c as DroppingSingularityControl);
                }
                else if (c is NewtonianMovementControl)
                {
                    msg.Write(typeof(NewtonianMovementControl).GUID.GetHashCode());
                    msg.WriteObjectNewtonianMovementControl(c as NewtonianMovementControl);
                }
                else if (c is LinearMovementControl)
                {
                    msg.Write(typeof(LinearMovementControl).GUID.GetHashCode());
                    msg.WriteObjectLinearMovementControl(c as LinearMovementControl);
                }
                else if (c is LinearRotationControl)
                {
                    msg.Write(typeof(LinearRotationControl).GUID.GetHashCode());
                    msg.WriteObjectLinearRotationControl(c as LinearRotationControl);
                }
                else if (c is PowerHookControl)
                {
                    msg.Write(typeof(PowerHookControl).GUID.GetHashCode());
                    msg.WriteObjectHookControl(c as HookControl);
                }
                else if (c is HookControl)
                {
                    msg.Write(typeof(HookControl).GUID.GetHashCode());
                    msg.WriteObjectHookControl(c as HookControl);
                }
                else if (c is ExplodingSingularityBulletControl)
                {
                    msg.Write(typeof(ExplodingSingularityBulletControl).GUID.GetHashCode());
                    msg.WriteObjectExplodingSingularityBulletControl(c as ExplodingSingularityBulletControl);
                }
                else if (c is ExcludingExplodingSingularityBulletControl)
                {
                    msg.Write(typeof(ExcludingExplodingSingularityBulletControl).GUID.GetHashCode());
                    msg.WriteObjectExcludingExplodingSingularityBulletControl(c as ExcludingExplodingSingularityBulletControl);
                }
                else if (c is MinorAsteroidCollisionReactionControl)
                {
                    msg.Write(typeof(MinorAsteroidCollisionReactionControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is SingularityBulletCollisionReactionControl)
                {
                    msg.Write(typeof(SingularityBulletCollisionReactionControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is StatPowerUpCollisionReactionControl)
                {
                    msg.Write(typeof(StatPowerUpCollisionReactionControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is StickyPointCollisionShapeControl)
                {
                    msg.Write(typeof(StickyPointCollisionShapeControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is StickySphereCollisionShapeControl)
                {
                    msg.Write(typeof(StickySphereCollisionShapeControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is StickySquareCollisionShapeControl)
                {
                    msg.Write(typeof(StickySquareCollisionShapeControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is BaseHealthControl)
                {
                    msg.Write(typeof(BaseHealthControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is DestroyHpControl)
                {
                    msg.Write(typeof(DestroyHpControl).GUID.GetHashCode());
                    msg.WriteControl(c);

                }
                else if (c is HpRegenControl)
                {
                    msg.Write(typeof(HpRegenControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else if (c is ModuleDamageControl)
                {
                    msg.Write(typeof(ModuleDamageControl).GUID.GetHashCode());
                    msg.WriteControl(c);
                }
                else
                {
                    msg.Write(0);
                    Logger.Error("Sending unspported control (" + c.GetType().Name + ")!");
                }
            }
        }

        public static IList<IControl> ReadControls(this NetIncomingMessage msg)
        {
            int controlCount = msg.ReadInt32();
            IList<IControl> controls = new List<IControl>(controlCount);
            for (int i = 0; i < controlCount; ++i)
            {
                int hash = msg.ReadInt32();
                if (hash == typeof(BaseCollisionControl).GUID.GetHashCode())
                {
                    BaseCollisionControl c = new BaseCollisionControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(BouncingSingularityBulletControl).GUID.GetHashCode())
                {
                    BouncingSingularityBulletControl c = new BouncingSingularityBulletControl();
                    msg.ReadObjectExplodingSingularityBulletControl(c);
                    controls.Add(c);
                } 
                else if (hash == typeof(DroppingSingularityControl).GUID.GetHashCode())
                {
                    DroppingSingularityControl c = new DroppingSingularityControl();
                    msg.ReadObjectDroppingSingularityControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(AsteroidDroppingSingularityControl).GUID.GetHashCode())
                {
                    AsteroidDroppingSingularityControl c = new AsteroidDroppingSingularityControl();
                    msg.ReadObjectDroppingSingularityControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(NewtonianMovementControl).GUID.GetHashCode())
                {
                    NewtonianMovementControl c = new NewtonianMovementControl();
                    msg.ReadObjectNewtonianMovementControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(LinearMovementControl).GUID.GetHashCode())
                {
                    LinearMovementControl c = new LinearMovementControl();
                    msg.ReadObjectLinearMovementControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(LinearRotationControl).GUID.GetHashCode())
                {
                    LinearRotationControl c = new LinearRotationControl();
                    msg.ReadObjectLinearRotationControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(PowerHookControl).GUID.GetHashCode())
                {
                    PowerHookControl c = new PowerHookControl();
                    msg.ReadObjectHookControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(HookControl).GUID.GetHashCode())
                {
                    HookControl c = new HookControl();
                    msg.ReadObjectHookControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(ExplodingSingularityBulletControl).GUID.GetHashCode())
                {
                    ExplodingSingularityBulletControl c = new ExplodingSingularityBulletControl();
                    msg.ReadObjectExplodingSingularityBulletControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(ExcludingExplodingSingularityBulletControl).GUID.GetHashCode())
                {
                    ExcludingExplodingSingularityBulletControl c = new ExcludingExplodingSingularityBulletControl();
                    msg.ReadObjectExcludingExplodingSingularityBulletControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(MinorAsteroidCollisionReactionControl).GUID.GetHashCode())
                {
                    MinorAsteroidCollisionReactionControl c = new MinorAsteroidCollisionReactionControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(SingularityBulletCollisionReactionControl).GUID.GetHashCode())
                {
                    SingularityBulletCollisionReactionControl c = new SingularityBulletCollisionReactionControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(StatPowerUpCollisionReactionControl).GUID.GetHashCode())
                {
                    StatPowerUpCollisionReactionControl c = new StatPowerUpCollisionReactionControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(StickyPointCollisionShapeControl).GUID.GetHashCode())
                {
                    StickyPointCollisionShapeControl c = new StickyPointCollisionShapeControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(StickySphereCollisionShapeControl).GUID.GetHashCode())
                {
                    StickySphereCollisionShapeControl c = new StickySphereCollisionShapeControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(StickySquareCollisionShapeControl).GUID.GetHashCode())
                {
                    StickySquareCollisionShapeControl c = new StickySquareCollisionShapeControl();
                    msg.ReadControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(BaseHealthControl).GUID.GetHashCode())
                {
                    BaseHealthControl c = new BaseHealthControl();
                    msg.ReadHealthControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(DestroyHpControl).GUID.GetHashCode())
                {
                    DestroyHpControl c = new DestroyHpControl();
                    msg.ReadHealthControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(HpRegenControl).GUID.GetHashCode())
                {
                    HpRegenControl c = new HpRegenControl();
                    msg.ReadHealthRegenControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(ModuleDamageControl).GUID.GetHashCode())
                {
                    ModuleDamageControl c = new ModuleDamageControl();
                    msg.ReadModuleDamageControl(c);
                    controls.Add(c);
                }
                else
                {
                    Logger.Error("Received unspported control (" + hash + ")!");
                }
            }
            return controls;
        }


        public static void ReadControl(this NetIncomingMessage msg, Control c)
        {
            c.Enabled = msg.ReadBoolean();
        }

        public static void WriteControl(this NetOutgoingMessage msg, IControl c)
        {
            msg.Write(c.Enabled);
        }

        public static void ReadModuleDamageControl(this NetIncomingMessage msg, ModuleDamageControl c)
        {
            msg.ReadControl(c);
            msg.ReadHealthControl(c);
            c.Vulnerable = msg.ReadBoolean();
        }

        public static void WriteModuleDamageControl(this NetOutgoingMessage msg, ModuleDamageControl c)
        {
            msg.WriteControl(c);
            msg.WriteHealthControl(c);
            msg.Write(c.Vulnerable);
        }

        public static void ReadHealthRegenControl(this NetIncomingMessage msg, HpRegenControl c)
        {
            msg.ReadControl(c);
            c.MaxRegenTime = msg.ReadFloat();
            c.RegenSpeed = msg.ReadFloat();
            c.RegenTimer = msg.ReadFloat();
        }

        public static void WriteHealthRegenControl(this NetOutgoingMessage msg, HpRegenControl c)
        {
            msg.WriteControl(c);
            msg.Write(c.MaxRegenTime);
            msg.Write(c.RegenSpeed);
            msg.Write(c.RegenTimer);
        }

        public static void ReadHealthControl(this NetIncomingMessage msg, IHpControl c)
        {
            msg.ReadControl(c as Control);
            c.Hp = msg.ReadInt32();
            c.MaxHp = msg.ReadInt32();
        }

        public static void WriteHealthControl(this NetOutgoingMessage msg, IHpControl c) 
        {
            msg.WriteControl(c as Control);
            msg.Write(c.Hp);
            msg.Write(c.MaxHp);
        }

        public static void ReadObjectExcludingExplodingSingularityBulletControl(this NetIncomingMessage msg, ExcludingExplodingSingularityBulletControl c)
        {
            msg.ReadObjectExplodingSingularityBulletControl(c);

            int max = msg.ReadInt32();

            for (int i = 0; i < max; i++)
                c.IgnoredObjects.Add(msg.ReadInt64());
        }

        public static void WriteObjectExcludingExplodingSingularityBulletControl(this NetOutgoingMessage msg, ExcludingExplodingSingularityBulletControl c)
        {
            msg.WriteObjectExplodingSingularityBulletControl(c);

            msg.Write((int)c.IgnoredObjects.Count);

            foreach (long id in c.IgnoredObjects)
            {
                msg.Write(id);
            }
        }

        public static void WriteObjectLinearMovementControl(this NetOutgoingMessage msg, LinearMovementControl c)
        {
            msg.WriteControl(c);

            msg.Write(c.Speed);
        }

        public static void ReadObjectLinearMovementControl(this NetIncomingMessage msg, LinearMovementControl c)
        {
            msg.ReadControl(c);

            c.Speed = msg.ReadFloat();
        }

        public static void WriteObjectNewtonianMovementControl(this NetOutgoingMessage msg, NewtonianMovementControl c)
        {
            msg.WriteControl(c);

            msg.Write(c.Speed);
        }

        public static void ReadObjectNewtonianMovementControl(this NetIncomingMessage msg, NewtonianMovementControl c)
        {
            msg.ReadControl(c);

            c.Speed = msg.ReadFloat();
        }

        public static void WriteObjectLinearRotationControl(this NetOutgoingMessage msg, LinearRotationControl c)
        {
            msg.WriteControl(c);

            msg.Write(c.RotationSpeed);
        }

        public static void ReadObjectLinearRotationControl(this NetIncomingMessage msg, LinearRotationControl c)
        {
            msg.ReadControl(c);

            c.RotationSpeed = msg.ReadFloat();
        }

        public static void WriteObjectDroppingSingularityControl(this NetOutgoingMessage msg, DroppingSingularityControl c)
        {
            msg.WriteControl(c);

            msg.Write(c.Speed);
            msg.Write(c.Strength);
        }

        public static void ReadObjectDroppingSingularityControl(this NetIncomingMessage msg, DroppingSingularityControl c)
        {
            msg.ReadControl(c);

            c.Speed = msg.ReadFloat();
            c.Strength = msg.ReadFloat();
        }

        public static void WriteObjectHookControl(this NetOutgoingMessage msg, HookControl c)
        {
            msg.WriteControl(c);

            msg.Write(c.Lenght);
            msg.Write(c.Origin);
            msg.Write(c.Speed);
        }

        public static void ReadObjectHookControl(this NetIncomingMessage msg, HookControl c)
        {
            msg.ReadControl(c);

            c.Lenght = msg.ReadInt32();
            c.Origin = msg.ReadVector();
            c.Speed = msg.ReadInt32();
        }

        public static void WriteObjectExplodingSingularityBulletControl(this NetOutgoingMessage msg, ExplodingSingularityBulletControl c)
        {
            msg.WriteControl(c);

            msg.Write(c.Strength);
            msg.Write(c.Speed);
        }

        public static void ReadObjectExplodingSingularityBulletControl(this NetIncomingMessage msg, ExplodingSingularityBulletControl c)
        {
            msg.ReadControl(c);

            c.Strength = msg.ReadFloat();
            c.Speed = msg.ReadFloat();
        }




        public static void WriteObjectBase(this NetOutgoingMessage msg, Base b)
        {
            msg.WriteObjectSceneObject(b);

            msg.Write((byte)b.BasePosition);
            msg.Write(b.Color);
            msg.Write(b.Integrity);
            msg.Write(b.Size);
        }

        public static void ReadObjectBase(this NetIncomingMessage msg, Base b)
        {
            msg.ReadObjectSceneObject(b);

            b.BasePosition = (PlayerPosition)msg.ReadByte();
            b.Color = msg.ReadColor();
            b.Integrity = msg.ReadInt32();
            b.Size = msg.ReadSize();
        }

        // basic types

        public static void Write(this NetOutgoingMessage msg, Vector v)
        {
            msg.Write(v.X);
            msg.Write(v.Y);
        }

        public static Vector ReadVector(this NetIncomingMessage msg)
        {
            return new Vector(msg.ReadDouble(), msg.ReadDouble());
        }

        public static void Write(this NetOutgoingMessage msg, Color c)
        {
            msg.Write(c.A);
            msg.Write(c.R);
            msg.Write(c.G);
            msg.Write(c.B);
        }

        public static Color ReadColor(this NetIncomingMessage msg)
        {
            return Color.FromArgb(msg.ReadByte(), msg.ReadByte(), msg.ReadByte(), msg.ReadByte());
        }

        public static void Write(this NetOutgoingMessage msg, Size s)
        {
            msg.Write(s.Width);
            msg.Write(s.Height);
        }

        public static Size ReadSize(this NetIncomingMessage msg)
        {
            return new Size(msg.ReadDouble(), msg.ReadDouble());
        }

        public static void Write(this NetOutgoingMessage msg, Point p)
        {
            msg.Write(p.X);
            msg.Write(p.Y);
        }

        public static Point ReadPoint(this NetIncomingMessage msg)
        {
            return new Point(msg.ReadDouble(), msg.ReadDouble());
        }

        // other

        public static void Write(this NetOutgoingMessage msg, TournamentSettings s)
        {
            msg.Write((int)s.MMType);
            msg.Write((int)s.Level);
            msg.Write(s.RoundCount);
            msg.Write((int)s.BotType);
            msg.Write(s.BotCount);
        }

        public static TournamentSettings ReadTournamentSettings(this NetIncomingMessage msg)
        {
            TournamentSettings s = new TournamentSettings();
            s.MMType = (MatchManagerType)msg.ReadInt32();
            s.Level = (GameLevel)msg.ReadInt32();
            s.RoundCount = msg.ReadInt32();
            s.BotType = (BotType)msg.ReadInt32();
            s.BotCount = msg.ReadInt32();
            return s;
        }
    }
}
