using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;

namespace Orbit.Core
{
    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate() { };


        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public static Point ToPoint(this Vector vec)
        {
            return new Point(vec.X, vec.Y);
        }

        public static double Distance(this Point p1, Point p2)
        {
            double d = Math.Pow(p1.X - p2.X, 2) + Math.Pow(p2.Y - p2.Y, 2);
            return Math.Sqrt(d);
        }

        public static float GetHorizontalLenght(this Vector vec)
        {
            return (float)new Vector(vec.X, 0).Length;
        }

        public static Vector Clone(this Vector vec)
        {
            return new Vector(vec.X, vec.Y);
        }

        public static Vector ToVector(this Point p)
        {
            return new Vector(p.X, p.Y);
        }

        public static Vector NormalizeV(this Vector v)
        {
            return new Vector(v.X / v.Length, v.Y / v.Length);
        }

        public static Vector Rotate(this Vector vec, double angle, Vector rotationOrigin, bool inRadians = true)
        {
            if (!inRadians)
                angle = Math.PI * angle / 180;

            return rotationOrigin + Rotate(vec - rotationOrigin, angle);
        }

        public static Vector Rotate(this Vector vec, double angle, bool inRadians = true)
        {
            return Rotate(vec, new Vector(0, 0), angle, inRadians);
        }

        public static Vector Rotate(this Vector vec, Vector center, double angle, bool inRadians = true)
        {
            if (!inRadians)
                angle = Math.PI * angle / 180;

            double x = vec.X - center.X;
            double y = vec.Y - center.Y;

            x = ((vec.X * Math.Cos(angle)) - (vec.Y * Math.Sin(angle)));
            y = ((vec.X * Math.Sin(angle)) + (vec.Y * Math.Cos(angle)));

            return new Vector(x + center.X, y + center.Y);
        }

        public static Vector Normal(this Vector v)
        {
            return new Vector(-v.Y, v.X);
        }
    }
}
