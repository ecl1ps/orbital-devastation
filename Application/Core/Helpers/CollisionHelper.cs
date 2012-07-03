﻿using System;
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

        public static bool IntersectsSquareAndSquare(Vector pos1, Size size1, Vector pos2, Size size2)
        {
            // SAT collision detection
            // http://stackoverflow.com/questions/115426/algorithm-to-detect-intersection-of-two-rectangles
            // http://www.sevenson.com.au/actionscript/sat/

            // zatim pocitam jen s obdelniky, ktere jsou rovnobezne s osami
            // jinak by bylo potreba souradnice jejich bodu dal rotovat
            // dalsi postup uz by byl stejny

            // vrcholy prvniho telesa
            Vector[] vertices1 = new Vector[4];
            vertices1[0] = pos1;
            vertices1[1] = new Vector(pos1.X + size1.Width, pos1.Y);
            vertices1[2] = new Vector(pos1.X, pos1.Y + size1.Height);
            vertices1[3] = new Vector(pos1.X + size1.Width, pos1.Y + size1.Height);

            // vrcholy prvniho telesa
            Vector[] vertices2 = new Vector[4];
            vertices2[0] = pos2;
            vertices2[1] = new Vector(pos2.X + size2.Width, pos2.Y);
            vertices2[2] = new Vector(pos2.X, pos2.Y + size2.Height);
            vertices2[3] = new Vector(pos2.X + size2.Width, pos2.Y + size2.Height);

            // TODO: kontrolovat jestli nejsou uplne v sobe

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
            double offset, dist1, dist2;

            // vezmu kazdou stranu prvniho polygonu a udelam k nemu normalu
            for (int i = 0; i < verts1.Length; ++i)
            {
                normal = GetAxisNormal(verts1, i);
                res1 = CheckDistancesForSAT(normal, verts1);
                res2 = CheckDistancesForSAT(normal, verts2);

                // posunuti promitnutych bodu prvniho polygonu
                offset = Vector.Multiply(normal, offsetVect);
                res1.min += offset;
                res1.max += offset;

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
