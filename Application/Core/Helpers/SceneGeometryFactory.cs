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
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using Orbit.Core.Client.Shaders;
using Orbit.Core.Client;

namespace Orbit.Core.Helpers
{
    class SceneGeometryFactory
    {
        public static DrawingGroup CreateConstantColorEllipseGeometry(Sphere s)
        {
            s.HasPositionInCenter = true;

            DrawingGroup d = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), s.Radius, s.Radius);
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(new SolidColorBrush(s.Color), new Pen(Brushes.Black, 1), geom));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(s.Position.X, s.Position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        public static Path CreateRadialGradientEllipseGeometry(Sphere s)
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
        }

        public static Path CreateRadialGradientEllipseGeometry(SceneMgr mgr, int radius, Color start, Color end, Color stroke, Vector position)
        {
            Path path = null;
            mgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), radius, radius);
                path = new Path();
                path.Data = geom;
                path.Fill = new RadialGradientBrush(end, start);
                path.Stroke = new SolidColorBrush(stroke);
                Canvas.SetLeft(path, position.X);
                Canvas.SetTop(path, position.Y);
            }));

            return path;
        }

        public static DrawingGroup CreateAsteroidImage(Asteroid a)
        {
            a.HasPositionInCenter = false;

            DrawingGroup g = null;

            a.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();

                if (a.AsteroidType == AsteroidType.GOLDEN)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_gold_" +
                        a.TextureId + ".png");
                else if(a.AsteroidType == AsteroidType.UNSTABLE)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_unstable_" +
                        a.TextureId + ".png");
                else
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_normal_" + 
                        a.TextureId + ".png");
                bi.DecodePixelWidth = a.Radius * 4;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.ImageSource = bi;
                img.Rect = new Rect(new Size(a.Radius * 2, a.Radius * 2));
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(a.Position.X, a.Position.Y));
                tg.Children.Add(new RotateTransform(a.Rotation, a.Radius, a.Radius));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static Path CreateRadialGradientEllipseGeometry(SingularityMine mine)
        {
            mine.HasPositionInCenter = true;

            Path path = null;
            mine.SceneMgr.Invoke(new Action(() =>
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

        public static Path CreateLinearGradientRectangleGeometry(StaticShield shield)
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
        }

        public static Path CreateLinearGradientRectangleGeometry(Base baze)
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
        }

        public static DrawingGroup CreateBaseImage(Base baze, String url, bool withShader = true)
        {
            DrawingGroup g = null;

            baze.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(url);
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.Rect = new Rect(baze.Size);

                if (withShader)
                {
                    ColorReplaceEffect effect = new ColorReplaceEffect();
                    effect.ColorToOverride = Colors.White;
                    effect.ColorReplace = baze.Owner.GetPlayerColor();

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)baze.Size.Width * 2, (int)baze.Size.Height * 2, 96, 96, PixelFormats.Pbgra32);

                    Rectangle visual = new Rectangle();
                    visual.Fill = new ImageBrush(bi);
                    visual.Effect = effect;

                    Size sz = new Size(baze.Size.Width * 2, baze.Size.Height * 2);
                    visual.Measure(sz);
                    visual.Arrange(new Rect(sz));
                    
                    rtb.Render(visual);
                    img.ImageSource = rtb;
                }
                else
                    img.ImageSource = bi;

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(baze.Position.X, baze.Position.Y));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static UIElement CreateHookHead(Hook hook)
        {
            hook.HasPositionInCenter = false;

            Image img = null;
            hook.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/hook/hook_head.png");
                bi.DecodePixelWidth = hook.Radius * 4;
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = hook.Radius * 2;
                img.RenderTransform = new RotateTransform(hook.Rotation);
                img.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetLeft(img, hook.Position.X);
                Canvas.SetTop(img, hook.Position.Y);
            }));

            return img;
        }

        public static System.Windows.Shapes.Line CreateLineGeometry(VectorLine l)
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
        }

        public static DrawingGroup CreatePowerUpImage(StatPowerUp p)
        {
            DrawingGroup g = null;

            p.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (p.PowerUpType == DeviceType.MINE)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/box/box_blue1.png");
                else if (p.PowerUpType == DeviceType.CANNON)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/box/box_brown.png");
                else if (p.PowerUpType == DeviceType.HOOK)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/box/box_purple.png");
                else
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/box/box_green.png");

                bi.DecodePixelWidth = (int)p.Size.Width * 2;
                bi.DecodePixelHeight = (int)p.Size.Height * 2;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.ImageSource = bi;
                img.Rect = new Rect(p.Size);
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(p.Position.X, p.Position.Y));
                tg.Children.Add(new RotateTransform(p.Rotation, p.Size.Width / 2, p.Size.Height / 2));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static Path CreateEllipseGeometry(OrbitEllipse e)
        {
            Path path = null;
            e.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), e.RadiusX, e.RadiusY);
                path = new Path();
                path.Data = geom;
                path.Fill = new SolidColorBrush(Colors.Black);
                Canvas.SetLeft(path, e.Position.X);
                Canvas.SetTop(path, e.Position.Y);
            }));

            return path;
        }

        public static Path CreateArcSegments(PercentageArc arc)
        {
            Path path = null;

            arc.SceneMgr.Invoke(new Action(() => {
                path = new Path();
                path.Data = createArc(arc);
                path.Stroke = new SolidColorBrush(arc.Color);
                path.StrokeThickness = 2;

                Canvas.SetLeft(path, arc.Position.X);
                Canvas.SetTop(path, arc.Position.Y);
            }));

            return path;
        }

        public static Path CreateArcSegments(PercentageEllipse arc)
        {
            Path path = null;

            arc.SceneMgr.Invoke(new Action(() =>
            {
                path = new Path();
                path.Data = createArc(arc);
                path.Stroke = new SolidColorBrush(Colors.Black);
                path.StrokeThickness = 1;
                path.Fill = new SolidColorBrush(arc.Color);


                Canvas.SetLeft(path, arc.Position.X);
                Canvas.SetTop(path, arc.Position.Y);
            }));

            return path;
        }

        private static PathGeometry createArc(PercentageEllipse e)
        {
            ArcSegment arc = new ArcSegment();

            PathFigure figure = new PathFigure();
            figure.StartPoint = e.StartPoint;

            arc = new ArcSegment();
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = true;
            arc.Size = new Size(e.A, e.B);
            arc.Point = e.ComputeEllipsePoint(0);

            e.SetArc(arc);

            figure.Segments.Add(arc);

            PathGeometry geom = new PathGeometry();
            geom.Figures.Add(figure);

            return geom;
        }

        private static PathGeometry createArc(PercentageArc a)
        {
            ArcSegment arc = new ArcSegment();
            Point point = new Point(a.Radius, 0);
            a.CenterOfArc = new Point(0, 0);

            PathFigure figure = new PathFigure();
            figure.StartPoint = point;


            arc = new ArcSegment();
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = true;
            arc.Size = new Size(a.Radius, a.Radius);
            arc.Point = a.ComputePointOnCircle(Math.PI * 2);

            a.SetArc(arc);

            figure.Segments.Add(arc);

            PathGeometry geom = new PathGeometry();
            geom.Figures.Add(figure);

            return geom;
        }

        public static Image CrateMiningModule(MiningModule m)
        {
            Image img = null;
            m.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/mining-module/module.png");
                bi.DecodePixelWidth = m.Radius * 4;
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = m.Radius * 2;
                img.RenderTransformOrigin = new Point(0.5, 0.5);
                img.RenderTransform = new RotateTransform(m.Rotation);

                Canvas.SetLeft(img, m.Position.X);
                Canvas.SetTop(img, m.Position.Y);
            }));

            return img;
        }

        public static Image CreateShield(StaticShield m)
        {
            Image img = null;
            m.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/actions/shield.png");
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = m.Radius * 2;
                img.Height = m.Radius * 2;

                Canvas.SetLeft(img, m.Position.X);
                Canvas.SetTop(img, m.Position.Y);
            }));

            return img;
        }
    }
}
