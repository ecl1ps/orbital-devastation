using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core.Helpers
{
    class CollisionHelper
    {
        private static Vector tempVector = new Vector();

        public static bool IntersectsCircleAndPoint(Vector point, Vector center, int radius)
        {
            return (point - center).Length <= radius;
        }

        public static bool IntersectsCircleAndCircle(Vector center1, int radius1, Vector center2, int radius2)
        {
            return (center1 - center2).Length <= radius1 + radius2;
        }

        public static bool IntersectsCircleAndSquare(Vector circleCenter, int circleRadius, Vector rectPosition, Size rectSize)
        {

            tempVector.X = Math.Abs(circleCenter.X - rectPosition.X - rectSize.Width / 2);
            tempVector.Y = Math.Abs(circleCenter.Y - rectPosition.Y - rectSize.Height / 2);

            if (tempVector.X > (rectSize.Width / 2 + circleRadius))
                return false;

            if (tempVector.Y > (rectSize.Height / 2 + circleRadius))
                return false;

            if (tempVector.X <= (rectSize.Width / 2))
                return true;

            if (tempVector.Y <= (rectSize.Height / 2))
                return true;

            double cornerDistanceSquared = Math.Pow((tempVector.X - rectSize.Width / 2), 2) +
                                 Math.Pow((tempVector.Y - rectSize.Height / 2), 2);

            return cornerDistanceSquared <= Math.Pow(circleRadius, 2);
        }

        public static bool IntersectsSquareAndSquare(Vector center1, Size rectSize1, Vector center2, Size rectSize2)
        {
            double lenght = (center1 - center2).Length;
            if (IntersectsPointAndSquare(center1, center2, rectSize2))
                return true;

            if (lenght < ((rectSize1.Height / 2) + (rectSize2.Height / 2)))
                return true;
            if (lenght < ((rectSize1.Width / 2) + (rectSize2.Width / 2)))
                return true;

            return false;
        }

        public static bool IntersectsPointAndSquare(Vector point, Vector squarePos, Size squareSize)
        {
            if (point.X >= squarePos.X && point.X <= squarePos.X + squareSize.Width &&
                point.Y >= squarePos.Y && point.Y <= squarePos.Y + squareSize.Height)
                return true;

            return false;
        }

        public static bool IntersectsPointAndPoint(Vector point1, Vector point2)
        {
            return ((int)point1.X) == ((int)point2.X) && ((int)point1.Y) == ((int)point2.Y);
        }

        public static bool IntersectsPointAndLine(Point p1, Point p2, Point point)
        {
            return PointToLineDistance(p1, p2, point) == 0;
        }

        public static bool IntersectCircleAndLine(Point p1, Point p2, Point center, double radius)
        {
            return PointToLineDistance(p1, p2, center) < radius;
        }

        public static bool IntersectLineAndLine(Point a1, Point a2, Point b1, Point b2)
        {
            Vector n = new Vector(-(a2.Y - a1.Y), a2.X - a1.X);
            
            double dot = n * (b1 - b2);
            
            //rovnobezky
            if (dot == 0)
                return false;

            double lenght = ((a1 - b1) * n) / dot;

            return lenght > 0 && lenght < 1;
        }

        public static double PointToLineDistance(Point l1, Point l2, Point p)
        {
            double normalLength = Math.Sqrt((l2.X - l1.X) * (l2.X - l1.X) + (l2.Y - l1.Y) * (l2.Y - l1.Y));

            return Math.Abs((p.X - p.X) * (l2.Y - l1.Y) - (p.Y - l1.Y) * (l2.X - l1.X)) / normalLength;
        }

        public static bool IntersectLineAndSquare(Point l1, Point l2, Point sCenter, Size sSize)
        {
            Point p1 = sCenter;
            Point p2 = new Point(sCenter.X + sSize.Width, sCenter.Y);
            Point p3 = new Point(sCenter.X, sCenter.Y + sSize.Height);
            Point p4 = new Point(sCenter.X + sSize.Width, sCenter.Y + sSize.Width);

            if (IntersectLineAndLine(l1, l2, p1, p2))
                return true;
            if (IntersectLineAndLine(l1, l2, p1, p3))
                return true;
            if (IntersectLineAndLine(l1, l2, p2, p4))
                return true;
            if (IntersectLineAndLine(l1, l2, p3, p4))
                return true;

            return false;
        }
    }
}
