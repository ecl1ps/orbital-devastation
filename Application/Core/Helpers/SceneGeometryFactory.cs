﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using System.Windows.Media;
using ShaderEffectLibrary;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using Orbit.Core.Client.Shaders;
using Orbit.Core.Client;
using System.Windows.Media.Imaging;

namespace Orbit.Core.Helpers
{
    public class SceneGeometryFactory
    {
        public static DrawingGroup CreateConstantColorEllipseGeometry(Sphere s)
        {
            DrawingGroup d = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(s.Radius, s.Radius), s.Radius, s.Radius);
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(new SolidColorBrush(s.Color), new Pen(Brushes.Black, 1), geom));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(s.Position.X, s.Position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        public static DrawingGroup CreateRadialGradientEllipseGeometry(SceneMgr mgr, int radius, Color start, Color end, Color stroke, Vector position, double strokeThickness)
        {
            DrawingGroup d = null;
            mgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(radius, radius), radius, radius);
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(new RadialGradientBrush(end, start), new Pen(new SolidColorBrush(stroke), strokeThickness), geom));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(position.X, position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        public static DrawingGroup CreateAsteroidImage(Asteroid a, bool burning = false)
        {
            DrawingGroup g = null;
            a.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();

                if (a.AsteroidType == AsteroidType.GOLDEN)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_gold_" +
                        a.TextureId + (burning ? "_burn" : "") + ".png");
                else if(a.AsteroidType == AsteroidType.UNSTABLE)
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_unstable_" +
                        a.TextureId + (burning ? "_burn" : "") + ".png");
                else
                    bi.UriSource = new Uri("pack://application:,,,/resources/images/rock/rock_normal_" +
                        a.TextureId + (burning ? "_burn" : "") + ".png");
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

        public static DrawingGroup CreateMineImage(SingularityMine mine)
        {
            DrawingGroup g = null;
            mine.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();

                bi.UriSource = new Uri("pack://application:,,,/resources/images/projectiles/mine.png");
                bi.DecodePixelWidth = mine.Radius * 4;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.Rect = new Rect(new Size(mine.Radius * 2, mine.Radius * 2));

                ColorReplaceEffect effect = new ColorReplaceEffect();
                effect.ColorToOverride = Colors.White;
                effect.ColorReplace = mine.Owner.GetPlayerColor();

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)mine.Radius * 2, (int)mine.Radius * 2, 96, 96, PixelFormats.Pbgra32);

                System.Windows.Shapes.Rectangle visual = new System.Windows.Shapes.Rectangle();
                visual.Fill = new ImageBrush(bi);
                visual.Effect = effect;

                Size sz = new Size(mine.Radius * 2, mine.Radius * 2);
                visual.Measure(sz);
                visual.Arrange(new Rect(sz));

                rtb.Render(visual);
                img.ImageSource = rtb;

                g = new DrawingGroup();
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(mine.Position.X, mine.Position.Y));
                tg.Children.Add(new RotateTransform(mine.Rotation, mine.Radius, mine.Radius));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static DrawingGroup CreateSolidColorEllipseGeometry(SingularityMine mine)
        {
            DrawingGroup d = null;
            mine.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(mine.Radius, mine.Radius), mine.Radius, mine.Radius);
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(mine.FillBrush, new Pen(mine.BorderBrush, 1), geom));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(mine.Position.X, mine.Position.Y));
                d.Transform = tg;

                /*
                RippleEffect eff = new RippleEffect();
                eff.Amplitude = 0.2;
                eff.Frequency = 60;
                path.Effect = eff;
                */
            }));

            return d;
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

                    System.Windows.Shapes.Rectangle visual = new System.Windows.Shapes.Rectangle();
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

        public static DrawingGroup CreateHookHead(Hook hook)
        {
            DrawingGroup g = null;
            hook.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/hook/hook_head.png");
                bi.DecodePixelWidth = hook.Radius * 4;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.ImageSource = bi;
                img.Rect = new Rect(new Size(hook.Radius * 2, hook.Radius * 2));
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(hook.Position.X, hook.Position.Y));
                tg.Children.Add(new RotateTransform(hook.Rotation, hook.Radius, hook.Radius));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static DrawingGroup CreateLineGeometry(SceneMgr mgr, Color border, double borderThickness, Color fill, Vector positionFrom, Vector positionTo)
        {
            DrawingGroup d = null;
            mgr.Invoke(new Action(() =>
            {
                LineGeometry g = new LineGeometry(positionFrom.ToPoint(), positionTo.ToPoint());
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(new SolidColorBrush(fill), new Pen(new SolidColorBrush(border), borderThickness), g));
            }));

            return d;
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

        public static DrawingGroup CreateConstantColorRectangleGeometry(Square s)
        {
            DrawingGroup d = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                RectangleGeometry geom = new RectangleGeometry(new Rect(s.Size));
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Black, 2), geom));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(s.Position.X, s.Position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        public static DrawingGroup CreateConstantColorEllipseGeometry(OrbitEllipse e)
        {
            DrawingGroup d = null;
            e.SceneMgr.Invoke(new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(new Point(e.RadiusX, e.RadiusY), e.RadiusX, e.RadiusY);
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(new SolidColorBrush(e.Color), null, geom));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(e.Position.X, e.Position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        public static DrawingGroup CreateArcSegments(MiningModuleIntegrityBar arc)
        {
            DrawingGroup d = null;
            arc.SceneMgr.Invoke(new Action(() =>
            {
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(new SolidColorBrush(arc.Color), 2), CreateArc(arc)));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(arc.Position.X, arc.Position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        public static DrawingGroup CreateArcSegments(BaseIntegrityBar arc)
        {
            DrawingGroup d = null;
            arc.SceneMgr.Invoke(new Action(() =>
            {
                d = new DrawingGroup();
                d.Children.Add(new GeometryDrawing(new SolidColorBrush(arc.Color), new Pen(Brushes.Black, 1), CreateArc(arc)));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(arc.Position.X, arc.Position.Y));
                d.Transform = tg;
            }));

            return d;
        }

        private static PathGeometry CreateArc(BaseIntegrityBar e)
        {
            ArcSegment arc = new ArcSegment();

            PathFigure figure = new PathFigure();
            figure.StartPoint = e.StartPoint;

            arc = new ArcSegment();
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = true;
            arc.Size = new Size(e.Width, e.Height);
            arc.Point = e.ComputeEllipsePoint(0);

            e.SetArc(arc);

            figure.Segments.Add(arc);

            PathGeometry geom = new PathGeometry();
            geom.Figures.Add(figure);

            return geom;
        }

        private static PathGeometry CreateArc(MiningModuleIntegrityBar a)
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
            geom.Transform = new TranslateTransform(a.Radius, a.Radius);

            return geom;
        }

        public static DrawingGroup CrateMiningModule(MiningModule m)
        {
            DrawingGroup g = null;
            m.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/mining-module/module.png");
                bi.DecodePixelWidth = m.Radius * 4;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.ImageSource = bi;
                img.Rect = new Rect(new Size(m.Radius * 2, m.Radius * 2));
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(m.Position.X, m.Position.Y));
                tg.Children.Add(new RotateTransform(m.Rotation, m.Radius, m.Radius));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static DrawingGroup CreateIceCube(IceSquare s)
        {
            DrawingGroup g = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/actions/ice-cube.png");
                bi.DecodePixelWidth = (int)s.Size.Width * 2;
                bi.DecodePixelHeight = (int)s.Size.Height * 2;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.ImageSource = bi;
                img.Rect = new Rect(s.Size);
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(s.Position.X, s.Position.Y));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }

        public static DrawingGroup CreateIceShard(IceShard s)
        {
            DrawingGroup g = null;
            s.SceneMgr.Invoke(new Action(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/resources/images/ice-shards/broken_ice_" + s.TextureId + ".png");
                bi.DecodePixelWidth = (int)s.Size.Width * 2;
                bi.DecodePixelHeight = (int)s.Size.Height * 2;
                bi.EndInit();

                g = new DrawingGroup();
                ImageDrawing img = new ImageDrawing();
                img.ImageSource = bi;
                img.Rect = new Rect(s.Size);
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(s.Position.X, s.Position.Y));
                g.Transform = tg;
                g.Children.Add(img);
            }));

            return g;
        }
    }
}
