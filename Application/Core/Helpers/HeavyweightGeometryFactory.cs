using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Scene.Entities;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Entities.Implementations.HeavyWeight;
using System.Windows.Media.Imaging;

namespace Orbit.Core.Helpers
{
    public class HeavyweightGeometryFactory
    {
        /*public static Path CreateRadialGradientEllipseGeometry(Sphere s)
        {
            s.HasPositionInCenter = true;

            Path path = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), s.Radius, s.Radius);
                path = new Path();
                path.Data = geom;
                path.Fill = new RadialGradientBrush(s.Color, Color.FromArgb(0x80, s.Color.R, s.Color.G, s.Color.B));
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, s.Position.X);
                Canvas.SetTop(path, s.Position.Y);
            }));

            return path;
        }*/

        /*public static Path CreateLinearGradientRectangleGeometry(StaticShield shield)
        {
            Path path = null;
            shield.SceneMgr.Invoke(new Action(() =>
            {
                RectangleGeometry geom = new RectangleGeometry(new Rect(0, 0, shield.Radius * 2, shield.Radius * 2));
                path = new Path();
                path.Data = geom;
                path.Fill = new LinearGradientBrush(shield.Color, Colors.Black, 90.0);
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, shield.Position.X);
                Canvas.SetTop(path, shield.Position.Y);
                GrowablePoissonDiskEffect eff = new GrowablePoissonDiskEffect();
                eff.Radius = 0.8;
                eff.Height = 30;
                eff.Width = 10;
                path.Effect = eff;
                //shield.SetGeometry(path);
            }));

            return path;
        }*/

        /*public static Path CreateLinearGradientRectangleGeometry(Base baze)
        {
            Path path = null;
            baze.SceneMgr.Invoke(new Action(() =>
            {
                RectangleGeometry geom = new RectangleGeometry(new Rect(0, 0, baze.Size.Width, baze.Size.Height));
                path = new Path();
                path.Data = geom;
                path.Fill = new LinearGradientBrush(baze.Color, Colors.Black, 90.0);
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, baze.Position.X);
                Canvas.SetTop(path, baze.Position.Y);
                GrowablePoissonDiskEffect eff = new GrowablePoissonDiskEffect();
                eff.Radius = 0.8;
                eff.Height = 30;
                eff.Width = 10;
                path.Effect = eff;
                //baze.SetGeometry(path);
            }));

            return path;
        }*/

        /*public static System.Windows.Shapes.Line CreateLineGeometry(VectorLine l)
        {
            System.Windows.Shapes.Line line = null;
            l.SceneMgr.Invoke(new Action(() =>
            {
                line = new System.Windows.Shapes.Line();
                line.Stroke = new SolidColorBrush(l.Color);
                line.X1 = l.Position.X;
                line.Y1 = l.Position.Y;
                line.X2 = l.Position.X + l.Direction.X;
                line.Y2 = l.Position.Y + l.Direction.Y;
                line.StrokeThickness = 1;
            }));

            return line;
        }*/

        public static Image CreateForceField(ForcePullField m)
        {
            Image img = null;
            m.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/actions/force_pull.png");
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = m.Radius * 2;
                img.Height = m.Radius * 2;
                img.Opacity = 0.5;

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(m.Position.X, m.Position.Y));
                img.RenderTransform = tg;

                Canvas.SetZIndex(img, 1);
            }));

            return img;
        }

        public static Path CreateConstantColorEllipseGeometry(SphereField s)
        {
            Path path = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(s.Radius, s.Radius), s.Radius, s.Radius);
                path = new Path();
                path.Data = geom;
                path.Fill = new SolidColorBrush(s.Color);

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(s.Position.X, s.Position.Y));
                path.RenderTransform = tg;

                Canvas.SetZIndex(path, -1);
            }));

            return path;
        }

        public static UIElement CreateConstantColorCircleGeometry(double radius, Color color)
        {
            Ellipse e = new Ellipse();
            e.Width = radius;
            e.Height = radius;
            SolidColorBrush b = new SolidColorBrush(color);
            e.Fill = b;
            e.Measure(new System.Windows.Size(radius, radius));
            e.Arrange(new Rect(0, 0, radius, radius));

            return e;
        }

        public static System.Windows.Shapes.Line CreateLineGeometry(Vector start, Vector end, Color color, int width)
        {
            System.Windows.Shapes.Line line = null;
            
            line = new System.Windows.Shapes.Line();
            line.Stroke = new SolidColorBrush(color);
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
            line.StrokeThickness = width;

            return line;
        }
    }
}
