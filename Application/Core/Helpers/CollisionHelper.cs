using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Orbit.Core.Client;

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

        public static bool IntersectsSquareAndSquare(Vector[] vertices1, Vector[] vertices2)
        {
            // SAT collision detection
            // http://stackoverflow.com/questions/115426/algorithm-to-detect-intersection-of-two-rectangles
            // http://www.sevenson.com.au/actionscript/sat/

            // nejdriv pro jedno teleso
            if (CheckPolygonAndPolygonForSAT(vertices1, vertices2))
                return false;

            // pokud nenajdu, ze se neprotinaji, tak kontroluju jeste druhe teleso
            if (CheckPolygonAndPolygonForSAT(vertices2, vertices1))
                return false;

            return true;
        }

        /// <summary>
        /// vraci true pokud se neprotinaji (je mezi nimi mezera) a false, pokud se to nevi
        /// </summary>
        private static bool CheckPolygonAndPolygonForSAT(Vector[] verts1, Vector[] verts2)
        {
            Vector offsetVect = new Vector(verts1[0].X - verts2[0].X, verts1[0].Y - verts2[0].Y);
            SATCheckInfo res1, res2;
            Vector normal;
            double dist1, dist2;

            // vezmu kazdou stranu prvniho polygonu a udelam k nemu normalu
            for (int i = 0; i < verts1.Length; ++i)
            {
                normal = GetAxisNormal(verts1, i);
                res1 = CheckDistancesForSAT(normal, verts1);
                res2 = CheckDistancesForSAT(normal, verts2);

                // kontrola pruniku
                dist1 = res1.min - res2.max;
                dist2 = res2.min - res1.max;
                // nalezena mezera mezi objekty
                if (dist1 > 0 || dist2 > 0)
                    return true;

                /*if (max0 > max1 || min0 < min1) result.shapeAContained = false;
                if (max1 > max0 || min1 < min0) result.shapeBContained = false;*/
            }

            // nenasli jsme zadnou mezeru - telesa se mohou a nemusi protinat (je treba je kontrolovat navzajem)
            return false;
        }

        /// <summary>
        /// vraci normalu strany polygonu
        /// </summary>
		private static Vector GetAxisNormal(Vector[] verts1, int index)
        {
			Vector pt1 = verts1[index];
			Vector pt2 = (index >= verts1.Length - 1) ? verts1[0] : verts1[index + 1];
			return new Vector(-(pt2.Y - pt1.Y), pt2.X - pt1.X);
		}

        /// <summary>
        /// vraci vzdalenosti bodu promitnutych na normalu
        /// </summary>
        private static SATCheckInfo CheckDistancesForSAT(Vector normal, Vector[] verts)
        {
            double dot;
            SATCheckInfo result = new SATCheckInfo();
            result.min = Vector.Multiply(normal, verts[0]);
            result.max = result.min;

            for (int i = 1; i < verts.Length; ++i)
            {
                dot = Vector.Multiply(normal, verts[i]);
                if (dot < result.min)
                    result.min = dot;
                if (dot > result.max)
                    result.max = dot;
            }

            return result;
        }

        private struct SATCheckInfo
        {
            public double min;
            public double max;
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

        public static bool IntersectsPointAndLine(Vector p1, Vector p2, Vector point)
        {
            return PointToLineDistance(p1, p2, point) == 0;
        }

        public static bool IntersectCircleAndLine(Vector p1, Vector p2, Vector center, double radius)
        {
            return PointToLineDistance(p1, p2, center) < radius;
        }

        public static bool IntersectLineAndLine(Vector a1, Vector a2, Vector b1, Vector b2)
        {
            Vector n = new Vector(-(a2.Y - a1.Y), a2.X - a1.X);
            
            double dot = n * (b1 - b2);
            
            //rovnobezky
            if (dot == 0)
                return false;

            double lenght = ((a1 - b1) * n) / dot;

            return lenght > 0 && lenght < 1;
        }

        public static double PointToLineDistance(Vector l1, Vector l2, Vector p)
        {
            Vector closest = ClosestPointOnSegment(l1, l2, p);
            return (p - closest).Length;
        }

        public static Vector ClosestPointOnSegment(Vector A, Vector B, Vector P)
        {
            Vector D = B-A;
            double numer = (P-A) * D;

            if (numer <= 0.0f)
                return A;
            
            double denom = D * D;
            
            if (numer >= denom)
                return B;
            
            return A + (numer/denom) * D;
        }

        public static bool IntersectLineAndSquare(Vector l1, Vector l2, Vector sCenter, Size sSize)
        {
            Vector p2 = new Vector(sCenter.X + sSize.Width, sCenter.Y);
            Vector p3 = new Vector(sCenter.X, sCenter.Y + sSize.Height);
            Vector p4 = new Vector(sCenter.X + sSize.Width, sCenter.Y + sSize.Width);

            if (IntersectLineAndLine(l1, l2, sCenter, p2))
                return true;
            if (IntersectLineAndLine(l1, l2, sCenter, p3))
                return true;
            if (IntersectLineAndLine(l1, l2, p2, p4))
                return true;
            if (IntersectLineAndLine(l1, l2, p3, p4))
                return true;

            return false;
        }
    }
}
