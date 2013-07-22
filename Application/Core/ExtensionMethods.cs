using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace Orbit.Core
{
    public static class ExtensionMethods
    {

        public static double Distance(this Vector2 p1, Vector2 p2)
        {
            double d = Math.Pow(p1.X - p2.X, 2) + Math.Pow(p2.Y - p2.Y, 2);
            return Math.Sqrt(d);
        }

        public static float GetHorizontalLenght(this Vector2 vec)
        {
            return (float)new Vector2(vec.X, 0).Length();
        }

        public static Vector2 Clone(this Vector2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static Vector2 ToVector(this Vector2 p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static Vector2 NormalizeV(this Vector2 v)
        {
            return new Vector2(v.X / v.Length(), v.Y / v.Length());
        }

        public static Vector2 Rotate(this Vector2 vec, float angle, Vector2 rotationOrigin, bool inRadians = true)
        {
            if (!inRadians)
                angle = (float) (Math.PI * angle / 180);

            return rotationOrigin + Rotate(vec - rotationOrigin, angle);
        }

        public static Vector2 Rotate(this Vector2 vec, float angle, bool inRadians = true)
        {
            return Rotate(vec, new Vector2(0, 0), angle, inRadians);
        }

        public static Vector2 Rotate(this Vector2 vec, Vector2 center, float angle, bool inRadians = true)
        {
            if (!inRadians)
                angle = FastMath.DegToRad(angle);

            float x = vec.X - center.X;
            float y = vec.Y - center.Y;

            x = (float) ((vec.X * Math.Cos(angle)) - (vec.Y * Math.Sin(angle)));
            y = (float) ((vec.X * Math.Sin(angle)) + (vec.Y * Math.Cos(angle)));

            return new Vector2(x + center.X, y + center.Y);
        }

        public static Vector2 Normal(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static float AngleBetween(this Vector2 v1, Vector2 v2)
        {
            v1.Normalize();
            v2.Normalize();
            double Angle = (float)Math.Acos(Vector2.Dot(v1, v2));
            return (float)Angle;
        }

        public static System.Windows.Media.Color ToWindowsColor(this Color c)
        {
            return System.Windows.Media.Color.FromArgb((byte)(c.A * 255), (byte)(c.R * 255), (byte)(c.G * 255), (byte)(c.B * 255));
        }

        public static Color ToXnaColor(this System.Windows.Media.Color c)
        {
            return new Color(c.A, c.R, c.G, c.B);
        }

        public static Microsoft.Xna.Framework.Point ToPoint(this Vector2 v)
        {
            return new Microsoft.Xna.Framework.Point((int) v.X, (int) v.Y);
        }

        public static Vector2 ToVector(this Microsoft.Xna.Framework.Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}
