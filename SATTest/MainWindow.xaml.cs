using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        private Rectangle r1;
        private Rectangle r2;
        private Label lbl;
        private double angle1 = 0, angle2 = 0;

        public MainWindow()
        {
            InitializeComponent();
            r1 = rectangle1;
            r2 = rectangle2;
            lbl = label1;
            MoveTo(new Point(150, 150));
        }

        private void canvas_MouseMove(object sender, MouseEventArgs ev)
        {
            canvas.Children.Clear();
            canvas.Children.Add(r1);
            canvas.Children.Add(r2);
            canvas.Children.Add(lbl);

            MoveTo(ev.GetPosition(canvas));
        }

        private void MoveTo(Point p)
        {
            Vector pos1 = new Vector(p.X, p.Y);
            Canvas.SetLeft(rectangle1, pos1.X);
            Canvas.SetTop(rectangle1, pos1.Y);
            Vector pos2 = new Vector(Canvas.GetLeft(rectangle2), Canvas.GetTop(rectangle2));
            Size size1 = new Size(rectangle1.Width, rectangle1.Height);
            Size size2 = new Size(rectangle2.Width, rectangle2.Height);

            if (IntersectsSquareAndSquare(pos1, size1, pos2, size2))
            {
                label1.Content = "Colliding";
                rectangle1.Fill = Brushes.Red;
            }
            else
            {
                label1.Content = "Not Colliding";
                rectangle1.Fill = Brushes.White;
            }
        }

        private void DrawEllipse(Vector vec)
        {
            Ellipse e = new Ellipse();
            e.Width = 3;
            e.Height = 3;
            e.Fill = Brushes.Blue;
            canvas.Children.Add(e);
            Canvas.SetLeft(e, vec.X);
            Canvas.SetTop(e, vec.Y);
        }

        private void DrawLine(Vector from, Vector to)
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

        public bool IntersectsSquareAndSquare(Vector pos1, Size size1, Vector pos2, Size size2)
        {
            // SAT collision detection
            // http://stackoverflow.com/questions/115426/algorithm-to-detect-intersection-of-two-rectangles
            // http://www.sevenson.com.au/actionscript/sat/

            // zatim pocitam jen s obdelniky, ktere jsou rovnobezne s osami
            // jinak by bylo potreba souradnice jejich bodu dal rotovat
            // dalsi postup uz by byl stejny

            // vrcholy prvniho telesa
            Vector[] vertices1 = GetVertices(pos1, size1, angle1);

            // vrcholy prvniho telesa
            Vector[] vertices2 = GetVertices(pos2, size2, angle2);

            // nejdriv pro jedno teleso
            if (CheckPolygonAndPolygonForSAT(vertices1, vertices2))
                return false;

            // pokud nenajdu, ze se neprotinaji, tak kontroluju jeste druhe teleso
            if (CheckPolygonAndPolygonForSAT(vertices2, vertices1))
                return false;

            return true;
        }

        private Vector[] GetVertices(Vector pos1, Size size1, double angle)
        {
            Vector[] vertices1 = new Vector[4];
            vertices1[0] = pos1;
            vertices1[1] = new Vector(pos1.X + size1.Width, pos1.Y);
            vertices1[2] = new Vector(pos1.X + size1.Width, pos1.Y + size1.Height);
            vertices1[3] = new Vector(pos1.X, pos1.Y + size1.Height);

            Vector squareCenter = new Vector(pos1.X + size1.Width / 2, pos1.Y + size1.Height / 2);
            for (int i = 0; i < vertices1.Length; ++i )
            {
                vertices1[i] = Rotate(vertices1[i], angle, squareCenter, false);
                DrawEllipse(vertices1[i]);
            }


            return vertices1;
        }

        public static Vector Rotate(Vector vec, double angle, Vector rotationOrigin, bool inRadians)
        {
            if (!inRadians)
                angle = Math.PI * angle / 180;
            
            return rotationOrigin + Rotate(vec - rotationOrigin, angle);
        }

        public static Vector Rotate(Vector vec, double angle, bool inRadians = true)
        {
            if (!inRadians)
                angle = Math.PI * angle / 180;

            double x = ((vec.X * Math.Cos(angle)) - (vec.Y * Math.Sin(angle)));
            double y = ((vec.X * Math.Sin(angle)) + (vec.Y * Math.Cos(angle)));
            return new Vector(x, y);
        }

        /// <summary>
        /// vraci true pokud se neprotinaji (je mezi nimi mezera) a false, pokud se to nevi
        /// </summary>
        private bool CheckPolygonAndPolygonForSAT(Vector[] verts1, Vector[] verts2)
        {
            Vector offsetVect = new Vector(verts1[0].X - verts2[0].X, verts1[0].Y - verts2[0].Y);
            SATCheckInfo res1, res2;
            Vector normal;
            double dist1, dist2;

            // vezmu kazdou stranu prvniho polygonu a udelam k nemu normalu
            for (int i = 0; i < verts1.Length; ++i)
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
        private Vector GetAxisNormal(Vector[] verts1, int index)
        {
            Vector pt1 = verts1[index];
            Vector pt2 = (index >= verts1.Length - 1) ? verts1[0] : verts1[index + 1];
            return new Vector(-(pt2.Y - pt1.Y), pt2.X - pt1.X);
        }

        /// <summary>
        /// vraci vzdalenosti bodu promitnutych na normalu
        /// </summary>
        private SATCheckInfo CheckDistancesForSAT(Vector normal, Vector[] verts, Vector normalOrigin)
        {
            double dot;
            SATCheckInfo result = new SATCheckInfo();
            result.min = Vector.Multiply(normal, verts[0]);
            result.max = result.min;

            for (int i = 1; i < verts.Length; ++i)
            {
                dot = Vector.Multiply(normal, verts[i]);
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
            rectangle1.RenderTransform = new RotateTransform(angle1);
            rectangle1.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            angle2 = e.NewValue;
            rectangle2.RenderTransform = new RotateTransform(angle2);
            rectangle2.RenderTransformOrigin = new Point(0.5, 0.5);
        }   
    }
}
