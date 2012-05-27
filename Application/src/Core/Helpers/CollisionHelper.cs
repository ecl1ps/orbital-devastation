using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core
{
    class CollisionHelper
    {
        private static Vector tempVector = new Vector();

        public static bool intersectsCircleAndPoint(Vector point, Vector center, int radius)
        {
            return (point - center).Length <= radius;
        }

        public static bool intersectsCircleAndCircle(Vector center1, int radius1, Vector center2, int radius2)
        {
            return (center1 - center2).Length <= radius1 + radius2;
        }

        public static bool intersectsCircleAndSquare(Vector circleCenter, int circleRadius, Vector rectPosition, Size rectSize)
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

        public static bool intersectSquareAndSquare(Vector center1, Size rectSize1, Vector center2, Size rectSize2)
        {
            double lenght = (center1 - center2).Length;
            if(intersectSquareAndPoint(center1, center2, rectSize2))
                return true;

            if(lenght < ((rectSize1.Height / 2) + (rectSize2.Height / 2)))
                return true;
            if(lenght < ((rectSize1.Width / 2) + (rectSize2.Width / 2)))
                return true;

            return false;
        }

        public static bool intersectSquareAndPoint(Vector point, Vector center, Size size)
        {
            if (point.X > center.X - (size.Width / 2))
                return true;
            if (point.Y > center.Y - (size.Height / 2))
                return true;

            return false;
        }

        public static bool intersectPointAndPoint(Vector point1, Vector point2)  
        {
            return point1.Equals(point2);
        }
    }
}
