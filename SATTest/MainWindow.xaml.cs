﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SATTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Polygon polygon;
        private Rectangle r2;
        private Label lbl;
        private double angle1 = 0, angle2 = 0;
        private Vector2 lastPos;

        public MainWindow()
        {
            InitializeComponent();
            polygon = poly;
            r2 = rectangle2;
            lbl = label1;
            MoveTo(new Vector2(150, 150));
        }

        private void canvas_MouseMove(object sender, MouseEventArgs ev)
        {
            ReAddObjectsOnCanvas();

            MoveTo(ev.GetPosition(canvas));
        }

        private void ReAddObjectsOnCanvas()
        {
            canvas.Children.Clear();
            canvas.Children.Add(polygon);
            canvas.Children.Add(r2);
            canvas.Children.Add(lbl);
        }

        private void MoveTo(Vector2 p)
        {
            lastPos = p;
            Canvas.SetLeft(polygon, p.X);
            Canvas.SetTop(polygon, p.Y);
            Vector2 pos2 = new Vector2(Canvas.GetLeft(rectangle2), Canvas.GetTop(rectangle2));
            Rectangle size2 = new Rectangle(rectangle2.Width, rectangle2.Height);

            if (IntersectsPolygonAndSquare(GetVerticesOfMovable(), pos2, size2))
            {
                label1.Content = "Colliding";
                rectangle2.Fill = Brushes.Red;
            }
            else
            {
                label1.Content = "Not Colliding";
                rectangle2.Fill = Brushes.White;
            }
        }

        private void DrawEllipse(Vector2 vec)
        {
            Ellipse e = new Ellipse();
            e.Width = 3;
            e.Height = 3;
            e.Fill = Brushes.Blue;
            canvas.Children.Add(e);
            Canvas.SetLeft(e, vec.X);
            Canvas.SetTop(e, vec.Y);
        }

        private void DrawLine(Vector2 from, Vector2 to)
        {
            Line l = new Line();
            l.Stroke = Brushes.Red;
            l.StrokeThickness = 1;
            l.X1 = from.X;
            l.Y1 = from.Y;
            l.X2 = to.X;
            l.Y2 = to.Y;
            canvas.Children.Add(l);
        }

        public bool IntersectsPolygonAndSquare(Vector2[] vertices1, Vector2 pos2, Rectangle size2)
        {
            // SAT collision detection
            // http://stackoverflow.com/questions/115426/algorithm-to-detect-intersection-of-two-rectangles
            // http://www.sevenson.com.au/actionscript/sat/

            // vrcholy druheho telesa
            Vector2[] vertices2 = GetVerticesOfStatic(pos2, size2, angle2);

            // nejdriv pro jedno teleso
            if (CheckPolygonAndPolygonForSAT(vertices1, vertices2))
                return false;

            // pokud nenajdu, ze se neprotinaji, tak kontroluju jeste druhe teleso
            if (CheckPolygonAndPolygonForSAT(vertices2, vertices1))
                return false;

            return true;
        }

        private Vector2[] GetVerticesOfMovable()
        {
            Vector2[] points = polygon.Points.ToArray();
            Vector2[] vertices1 = new Vector2[points.Length()];
            Vector2 polyPos = new Vector2(Canvas.GetLeft(polygon), Canvas.GetTop(polygon));
            for (int i = 0; i < points.Length(); ++i)
            {
                vertices1[i] = new Vector2(points[i].X, points[i].Y) + polyPos;
            }

            Vector2 squareCenter = new Vector2(polyPos.X + polygon.ActualWidth / 2, polyPos.Y + polygon.ActualHeight / 2);
            for (int i = 0; i < vertices1.Length(); ++i )
            {
                vertices1[i] = Rotate(vertices1[i], angle1, squareCenter, false);
                DrawEllipse(vertices1[i]);
            }

            return vertices1;
        }

        private Vector2[] GetVerticesOfStatic(Vector2 pos1, Rectangle size1, double angle)
        {
            Vector2[] vertices1 = new Vector2[4];
            vertices1[0] = pos1;
            vertices1[1] = new Vector2(pos1.X + size1.Width, pos1.Y);
            vertices1[2] = new Vector2(pos1.X + size1.Width, pos1.Y + size1.Height);
            vertices1[3] = new Vector2(pos1.X, pos1.Y + size1.Height);

            Vector2 squareCenter = new Vector2(pos1.X + size1.Width / 2, pos1.Y + size1.Height / 2);
            for (int i = 0; i < vertices1.Length(); ++i)
            {
                vertices1[i] = Rotate(vertices1[i], angle, squareCenter, false);
                DrawEllipse(vertices1[i]);
            }

            return vertices1;
        }

        public static Vector2 Rotate(Vector2 vec, double angle, Vector2 rotationOrigin, bool inRadians)
        {
            if (!inRadians)
                angle = Math.PI * angle / 180;
            
            return rotationOrigin + Rotate(vec - rotationOrigin, angle);
        }

        public static Vector2 Rotate(Vector2 vec, double angle, bool inRadians = true)
        {
            if (!inRadians)
                angle = Math.PI * angle / 180;

            double x = ((vec.X * Math.Cos(angle)) - (vec.Y * Math.Sin(angle)));
            double y = ((vec.X * Math.Sin(angle)) + (vec.Y * Math.Cos(angle)));
            return new Vector2(x, y);
        }

        /// <summary>
        /// vraci true pokud se neprotinaji (je mezi nimi mezera) a false, pokud se to nevi
        /// </summary>
        private bool CheckPolygonAndPolygonForSAT(Vector2[] verts1, Vector2[] verts2)
        {
            Vector2 offsetVect = new Vector2(verts1[0].X - verts2[0].X, verts1[0].Y - verts2[0].Y);
            SATCheckInfo res1, res2;
            Vector2 normal;
            double dist1, dist2;

            // vezmu kazdou stranu prvniho polygonu a udelam k nemu normalu
            for (int i = 0; i < verts1.Length(); ++i)
            {
                normal = GetAxisNormal(verts1, i);
                //normal.Normalize();
                DrawLine(verts1[i], normal * 500 + verts1[i]);

                res1 = CheckDistancesForSAT(normal, verts1, verts1[i]);
                res2 = CheckDistancesForSAT(normal, verts2, verts1[i]);

                // kontrola pruniku
                dist1 = res1.min - res2.max;
                dist2 = res2.min - res1.max;
                // nalezena mezera mezi objekty
                if (dist1 > 0 || dist2 > 0)
                    return true;
            }

            // nenasli jsme zadnou mezeru - telesa se mohou a nemusi protinat (je treba je kontrolovat navzajem)
            return false;
        }

        /// <summary>
        /// vraci normalu strany polygonu
        /// </summary>
        private Vector2 GetAxisNormal(Vector2[] verts1, int index)
        {
            Vector2 pt1 = verts1[index];
            Vector2 pt2 = (index >= verts1.Length() - 1) ? verts1[0] : verts1[index + 1];
            return new Vector2(-(pt2.Y - pt1.Y), pt2.X - pt1.X);
        }

        /// <summary>
        /// vraci vzdalenosti bodu promitnutych na normalu
        /// </summary>
        private SATCheckInfo CheckDistancesForSAT(Vector2 normal, Vector2[] verts, Vector2 normalOrigin)
        {
            double dot;
            SATCheckInfo result = new SATCheckInfo();
            result.min = Vector2.Multiply(normal, verts[0]);
            result.max = result.min;

            for (int i = 1; i < verts.Length(); ++i)
            {
                dot = Vector2.Multiply(normal, verts[i]);
                //DrawEllipse(normalOrigin + normal * dot);

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

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            angle1 = e.NewValue;
            polygon.RenderTransform = new RotateTransform(angle1);
            polygon.RenderTransformOrigin = new Vector2(0.5, 0.5);
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            angle2 = e.NewValue;
            rectangle2.RenderTransform = new RotateTransform(angle2);
            rectangle2.RenderTransformOrigin = new Vector2(0.5, 0.5);
        }

        private void CreateNewPolygon(PointCollection points)
        {
            polygon = new Polygon();
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 1;
            polygon.Points = points;
            polygon.RenderTransform = new RotateTransform(angle1);
            polygon.RenderTransformOrigin = new Vector2(0.5, 0.5);
            Canvas.SetLeft(polygon, lastPos.X);
            Canvas.SetTop(polygon, lastPos.Y);

            ReAddObjectsOnCanvas();
        }

        private void rb2_Checked(object sender, RoutedEventArgs e)
        {
            PointCollection points = new PointCollection();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(100, 0));
            CreateNewPolygon(points);
        }

        private void rb3_Checked(object sender, RoutedEventArgs e)
        {
            PointCollection points = new PointCollection();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(100, 0));
            points.Add(new Vector2(50, 50));
            CreateNewPolygon(points);
        }

        private void rb4_Checked(object sender, RoutedEventArgs e)
        {
            PointCollection points = new PointCollection();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(100, 0));
            points.Add(new Vector2(100, 100));
            points.Add(new Vector2(0, 100));
            CreateNewPolygon(points);
        }

        private void rb5_Checked(object sender, RoutedEventArgs e)
        {
            PointCollection points = new PointCollection();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(100, 0));
            points.Add(new Vector2(180, 60));
            points.Add(new Vector2(30, 90));
            points.Add(new Vector2(-20, 40));
            CreateNewPolygon(points);
        }   
    }
}
