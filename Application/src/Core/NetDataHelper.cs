using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Orbit.Core.Scene.Entities;
using System.Windows;
using System.Windows.Media;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core
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
            msg.Write(s.IsHeadingRight);
            msg.Write(s.Radius);
            msg.Write(s.Rotation);
            msg.Write(s.TextureId);
        }

        public static void ReadObjectSphere(this NetIncomingMessage msg, Sphere s)
        {
            msg.ReadObjectSceneObject(s);

            s.Direction = msg.ReadVector();
            s.IsHeadingRight = msg.ReadBoolean();
            s.Radius = msg.ReadInt32();
            s.Rotation = msg.ReadFloat();
            s.TextureId = msg.ReadInt32();
        }

        public static void WriteObjectSingularityMine(this NetOutgoingMessage msg, SingularityMine s)
        {
            msg.WriteObjectSceneObject(s);
        }

        public static void ReadObjectSingularityMine(this NetIncomingMessage msg, SingularityMine s)
        {
            msg.ReadObjectSceneObject(s);
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
            msg.Write(d.MineCooldown);
            msg.Write(d.MineGrowthSpeed);
            msg.Write(d.MineStrength);
            msg.Write(d.PlayerColor);
            msg.Write((byte)d.PlayerPosition);
            msg.Write(d.Score);
        }

        public static void ReadObjectPlayerData(this NetIncomingMessage msg, PlayerData d)
        {
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
