﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

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

        public static Vector Rotate(this Vector vec, double angle)
        {
            double x = ((vec.X * Math.Cos(angle)) - (vec.Y * Math.Sin(angle)));
            double y = ((vec.X * Math.Sin(angle)) + (vec.Y * Math.Cos(angle)));
            return new Vector(x, y);
        }
    }
}
