using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Orbit.Core.Scene
{
    class SceneGeometryFactory
    {
        public static Path CreateRadialGradientEllipseGeometry(Sphere s)
        {
            Path path = null;
            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), s.Radius, s.Radius);
                path = new Path();
                path.Data = geom;
                path.Fill = new RadialGradientBrush(s.Color, Color.FromArgb(0x80, s.Color.R, s.Color.G, s.Color.B));
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, s.GetPosition().X);
                Canvas.SetTop(path, s.GetPosition().Y);
            }));

            return path;
        }

        public static Image CreateAsteroidImage(Sphere s)
        {
            Image img = null;
            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                /*BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/Stone0" +
                    SceneMgr.GetInstance().GetRandomGenerator().Next(1, 8) + "_SR.png");
                bi.DecodePixelWidth = s.Radius * 4;
                bi.EndInit();*/

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/Rock" + 
                    SceneMgr.GetInstance().GetRandomGenerator().Next(1, 18) + "_SR.png");
                bi.DecodePixelWidth = s.Radius * 4;
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = s.Radius * 2;
                img.RenderTransform = new RotateTransform(s.GetRotation());
                img.RenderTransformOrigin = new Point(0.5, 0.5);
                Canvas.SetLeft(img, s.GetPosition().X - s.Radius);
                Canvas.SetTop(img, s.GetPosition().Y - s.Radius);
            }));

            return img;
        }

        public static Path CreateRadialGradientEllipseGeometry(SingularityMine mine)
        {
            Path path = null;
            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), 0, 0);
                path = new Path();
                path.Data = geom;
                path.Fill = new RadialGradientBrush(Colors.Black, Color.FromRgb(0x66, 0x00, 0x80));
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, mine.GetPosition().X);
                Canvas.SetTop(path, mine.GetPosition().Y);
            }));

            return path;
        }    

        public static Path CreateLinearGradientRectangleGeometry(Base baze)
        {
            Path path = null;
            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                RectangleGeometry geom = new RectangleGeometry(new Rect(0, 0, baze.Size.Width, baze.Size.Height));
                path = new Path();
                path.Data = geom;
                path.Fill = new LinearGradientBrush(baze.Color, Colors.Black, 90.0);
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, baze.GetPosition().X);
                Canvas.SetTop(path, baze.GetPosition().Y);
                baze.SetGeometry(path);
            }));

            return path;
        } 
    }
}
