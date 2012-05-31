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
using ShaderEffectLibrary;

namespace Orbit.Core.Scene
{
    class SceneGeometryFactory
    {
        public static Path CreateConstantColorEllipseGeometry(Sphere s)
        {
            Path path = null;
            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), s.Radius, s.Radius);
                path = new Path();
                path.Data = geom;
                path.Fill = new SolidColorBrush(s.Color);
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, s.Position.X);
                Canvas.SetTop(path, s.Position.Y);
            }));

            return path;
        }

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
                Canvas.SetLeft(path, s.Position.X);
                Canvas.SetTop(path, s.Position.Y);
            }));

            return path;
        }

        public static Image CreateAsteroidImage(Asteroid s)
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
                if (s.Gold > 0)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_gold_" +
                        s.TextureId + ".png");
                else
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_normal_" + 
                        s.TextureId + ".png");
                bi.DecodePixelWidth = s.Radius * 4;
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = s.Radius * 2;
                img.RenderTransform = new RotateTransform(s.Rotation);
                img.RenderTransformOrigin = new Point(0.5, 0.5);

                /*if (s.Gold > 0)
                {
                    ColorToneEffect eff = new ColorToneEffect();
                    eff.Desaturation = 2;
                    eff.Toned = 1;
                    eff.LightColor = Colors.Goldenrod;
                    img.Effect = eff;
                    Canvas.SetZIndex(img, -1); // tone shader spatne pracuje s pruhlednymi castmi obrazku - musi byt uplne dole
                }*/

                Canvas.SetLeft(img, s.Position.X - s.Radius);
                Canvas.SetTop(img, s.Position.Y - s.Radius);
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
                path.Fill = mine.FillBrush;
                path.Stroke = mine.BorderBrush;
                RippleEffect eff = new RippleEffect();
                eff.Amplitude = 0.2;
                eff.Frequency = 60;
                path.Effect = eff;
                Canvas.SetLeft(path, mine.Position.X);
                Canvas.SetTop(path, mine.Position.Y);
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
                Canvas.SetLeft(path, baze.Position.X);
                Canvas.SetTop(path, baze.Position.Y);
                GrowablePoissonDiskEffect eff = new GrowablePoissonDiskEffect();
                eff.Radius = 0.8;
                eff.Height = 30;
                eff.Width = 10;
                path.Effect = eff;
                baze.SetGeometry(path);
            }));

            return path;
        } 
    }
}
