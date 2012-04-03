using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Player;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.Scene
{
    static class SceneObjectFactory
    {
        public static Sphere CreateNewRandomSphere(bool headingRight)
        {
            Rect actionArea = SceneMgr.GetInstance().GetActionArea();
            Random randomGenerator = SceneMgr.GetInstance().GetRandomGenerator();
            Sphere s = new Sphere();
            s.Setid(IdMgr.GetNewId());
            s.IsHeadingRight = headingRight;
            s.SetDirection(headingRight ? new Vector(1, 1) : new Vector(-1, 0));

            s.Radius = randomGenerator.Next(SharedDef.MIN_SPHERE_RADIUS, SharedDef.MAX_SPHERE_RADIUS);
            s.SetPosition(new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius))));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));


            LinearMovementControl lc = new LinearMovementControl();
            lc.Speed = randomGenerator.Next(SharedDef.MIN_SPHERE_SPEED * 10, SharedDef.MAX_SPHERE_SPEED * 10) / 10.0f;
            s.AddControl(lc);

            SceneMgr.GetInstance().GetCanvasDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(s.GetPosition().ToPoint(), s.Radius, s.Radius);
                Path path = new Path();
                path.Data = geom;
                path.Fill = new RadialGradientBrush(s.Color, Color.FromArgb(0x80, s.Color.R, s.Color.G, s.Color.B));
                path.Stroke = Brushes.Black;
                s.SetGeometry(path);
            }));

            return s;
        }

        public static Base CreateBase(PlayerPosition pos, Color col)
        {
            Base baze = new Base();
            baze.Setid(IdMgr.GetNewId());
            baze.BasePosition = pos;
            baze.Color = col;
            baze.Integrity = SharedDef.BASE_MAX_INGERITY;

            baze.Position = new Vector(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * ((pos == PlayerPosition.LEFT) ? 0.1 : 0.6),
                                       SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.85);
            baze.Size = new Size(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * 0.3, 
                                 SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.15);

            Rect rec = new Rect(baze.Position.X, baze.Position.Y, baze.Size.Width, baze.Size.Height);

            SceneMgr.GetInstance().GetCanvasDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                RectangleGeometry geom = new RectangleGeometry(rec);
                Path path = new Path();
                path.Data = geom;
                path.Fill = new LinearGradientBrush(baze.Color, Colors.Black, 90.0);
                path.Stroke = Brushes.Black;
                baze.SetGeometry(path);
            }));

            return baze;
        }

        public static Sphere CreateNewSphereOnEdge(Sphere oldSphere)
        {
            Sphere s = CreateNewRandomSphere(oldSphere.IsHeadingRight);

            Rect actionArea = SceneMgr.GetInstance().GetActionArea();

            s.SetPosition(new Vector(s.IsHeadingRight ? (int)(-s.Radius) : (int)(actionArea.Width + s.Radius),
                SceneMgr.GetInstance().GetRandomGenerator().Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius))));

            return s;
        }
    }
}
