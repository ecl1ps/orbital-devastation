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

namespace Orbit.Core.Helpers
{
    static class NetDataHelper
    {
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

            Vector dir = s.Direction.Clone();
            dir.Normalize();
            msg.Write(dir);
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
            msg.WriteObjectSphere(h);
            msg.Write(h.Rotation);
        }

        public static void ReadObjectHook(this NetIncomingMessage msg, Hook h)
        {
            msg.ReadObjectSphere(h);
            h.Rotation = msg.ReadFloat();
        }
        
        public static void WriteControls(this NetOutgoingMessage msg, IList<IControl> controls)
        {
            msg.Write(controls.Count);
            foreach (IControl c in controls)
            {
                if (c is SingularityControl)
                {
                    msg.Write(typeof(SingularityControl).GUID.GetHashCode());
                    msg.WriteObjectSingularityControl(c as SingularityControl);
                }
                else if (c is DroppingSingularityControl)
                {
                    msg.Write(typeof(DroppingSingularityControl).GUID.GetHashCode());
                    msg.WriteObjectDroppingSingularityControl(c as DroppingSingularityControl);
                }
                if (c is NewtonianMovementControl)
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
                else if (c is HookControl)
                {
                    msg.Write(typeof(HookControl).GUID.GetHashCode());
                    msg.WriteObjectHookControl(c as HookControl);
                }
                else
                    Console.Error.WriteLine("Sending unspported control (" + typeof(LinearMovementControl).Name + ")!");
            }
        }

        public static IList<IControl> ReadControls(this NetIncomingMessage msg)
        {
            IList<IControl> controls = new List<IControl>();
            int controlCount = msg.ReadInt32();
            for (int i = 0; i < controlCount; ++i)
            {
                int hash = msg.ReadInt32();
                if (hash == typeof(SingularityControl).GUID.GetHashCode())
                {
                    SingularityControl c = new SingularityControl();
                    msg.ReadObjectSingularityControl(c);
                    controls.Add(c);
                }
                else if (hash == typeof(DroppingSingularityControl).GUID.GetHashCode())
                {
                    DroppingSingularityControl c = new DroppingSingularityControl();
                    msg.ReadObjectDroppingSingularityControl(c);
                    controls.Add(c);
                }
                if (hash == typeof(NewtonianMovementControl).GUID.GetHashCode())
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
                else if (hash == typeof(HookControl).GUID.GetHashCode())
                {
                    HookControl c = new HookControl();
                    msg.ReadObjectHookControl(c);
                    controls.Add(c);
                }
                else
                    Console.Error.WriteLine("Received unspported control (" + hash + ")!");
            }
            return controls;
        }

        public static void WriteObjectLinearMovementControl(this NetOutgoingMessage msg, LinearMovementControl c)
        {
            msg.Write(c.InitialSpeed);
        }

        public static void ReadObjectLinearMovementControl(this NetIncomingMessage msg, LinearMovementControl c)
        {
            c.InitialSpeed = msg.ReadFloat();
        }

        public static void WriteObjectNewtonianMovementControl(this NetOutgoingMessage msg, NewtonianMovementControl c)
        {
            msg.Write(c.InitialSpeed);
        }

        public static void ReadObjectNewtonianMovementControl(this NetIncomingMessage msg, NewtonianMovementControl c)
        {
            c.InitialSpeed = msg.ReadFloat();
        }

        public static void WriteObjectLinearRotationControl(this NetOutgoingMessage msg, LinearRotationControl c)
        {
            msg.Write(c.RotationSpeed);
        }

        public static void ReadObjectLinearRotationControl(this NetIncomingMessage msg, LinearRotationControl c)
        {
            c.RotationSpeed = msg.ReadFloat();
        }

        public static void WriteObjectSingularityControl(this NetOutgoingMessage msg, SingularityControl c)
        {
            msg.Write(c.Speed);
            msg.Write(c.Strength);
        }

        public static void ReadObjectSingularityControl(this NetIncomingMessage msg, SingularityControl c)
        {
            c.Speed = msg.ReadFloat();
            c.Strength = msg.ReadFloat();
        }

        public static void WriteObjectDroppingSingularityControl(this NetOutgoingMessage msg, DroppingSingularityControl c)
        {
            msg.Write(c.Speed);
            msg.Write(c.Strength);
        }

        public static void ReadObjectDroppingSingularityControl(this NetIncomingMessage msg, DroppingSingularityControl c)
        {
            c.Speed = msg.ReadFloat();
            c.Strength = msg.ReadFloat();
        }

        public static void WriteObjectHookControl(this NetOutgoingMessage msg, HookControl c)
        {
            msg.Write(c.Lenght);
            msg.Write(c.Origin);
            msg.Write(c.Speed);
        }

        public static void ReadObjectHookControl(this NetIncomingMessage msg, HookControl c)
        {
            c.Lenght = msg.ReadInt32();
            c.Origin = msg.ReadVector();
            c.Speed = msg.ReadInt32();
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

        public static void WriteObjectPlayerData(this NetOutgoingMessage msg, PlayerData d)
        {
            msg.Write(d.Id);
            msg.Write(d.Name);
            msg.Write(d.Active);
            msg.Write(d.MineCooldown);
            msg.Write(d.MineGrowthSpeed);
            msg.Write(d.MineStrength);
            msg.Write(d.PlayerColor);
            msg.Write((byte)d.PlayerPosition);
            msg.Write(d.Score);
        }

        public static void ReadObjectPlayerData(this NetIncomingMessage msg, PlayerData d)
        {
            d.Id = msg.ReadInt32();
            d.Name = msg.ReadString();
            d.Active = msg.ReadBoolean();
            d.MineCooldown = msg.ReadInt32();
            d.MineGrowthSpeed = msg.ReadFloat();
            d.MineStrength = msg.ReadFloat();
            d.PlayerColor = msg.ReadColor();
            d.PlayerPosition = (PlayerPosition)msg.ReadByte();
            d.Score = msg.ReadInt32();
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
    }
}
