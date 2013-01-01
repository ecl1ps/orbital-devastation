﻿using System;
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
        public static Path CreateConstantColorEllipseGeometry(Sphere s)
        {
            s.HasPositionInCenter = true;

            Path path = null;
            s.SceneMgr.Invoke(new Action(() =>
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

        public static Path CreateConstantRippedColorEllipseGeometry(Sphere s)
        {
            s.HasPositionInCenter = true;

            Path path = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(), s.Radius, s.Radius);
                path = new Path();
                path.Data = geom;
                path.Fill = new SolidColorBrush(s.Color);
                path.Stroke = Brushes.Black;
                Canvas.SetLeft(path, s.Position.X);
                Canvas.SetTop(path, s.Position.Y);
                Canvas.SetZIndex(path, -10);
            }));

            return path;
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

        public static Image CreateAsteroidImage(Asteroid s)
        {
            s.HasPositionInCenter = false;

            Image img = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                /*BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/Stone0" +
                    SceneMgr.GetInstance().GetRandomGenerator().Next(1, 8) + "_SR.png");
                bi.DecodePixelWidth = s.Radius * 4;
                bi.EndInit();*/

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                //prozatim jenom na gold pak bude vic typu
                if (s.AsteroidType == AsteroidType.GOLDEN)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_gold_" +
                        s.TextureId + ".png");
                else if(s.AsteroidType == AsteroidType.UNSTABLE)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_unstable_" +
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

                Canvas.SetLeft(img, s.Position.X);
                Canvas.SetTop(img, s.Position.Y);
            }));

            return img;
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
                shield.SetGeometry(path);
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
                baze.SetGeometry(path);
            }));

            return path;
        }

        public static UIElement CreateBaseImage(Base baze, String url, bool withShader = true)
        {
            Image img = null;
            baze.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(url);
                bi.EndInit();

                img = new Image();
                img.Source = bi;
                img.Width = baze.Size.Width;
                img.Height = baze.Size.Height;
                img.RenderTransformOrigin = new Point(0.5, 0.5);

                if (withShader)
                {
                    ColorReplaceEffect effect = new ColorReplaceEffect();
                    effect.ColorToOverride = Colors.White;
                    effect.ColorReplace = baze.Owner.GetPlayerColor();

                    img.Effect = effect;
                }

                Canvas.SetLeft(img, baze.Position.X);
                Canvas.SetTop(img, baze.Position.Y);
            }));

            return img;
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

        public static UIElement CreatePowerUpImage(StatPowerUp p)
        {
            Image img = null;
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

                img = new Image();
                img.Source = bi;
                img.Width = p.Size.Width;
                img.Height = p.Size.Height;
                img.RenderTransform = new RotateTransform(p.Rotation);
                img.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetLeft(img, p.Position.X);
                Canvas.SetTop(img, p.Position.Y);
            }));

            return img;
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
