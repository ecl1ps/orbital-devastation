using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using Orbit.Core.Scene.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Orbit.Core.Scene
{
    static class SceneObjectFactory
    {
        public static Sphere CreateNewRandomSphere(bool headingRight)
        {
            Rect actionArea = SceneMgr.GetInstance().GetOrbitArea();
            Random randomGenerator = SceneMgr.GetInstance().GetRandomGenerator();
            Sphere s = new Sphere();
            s.Id = IdMgr.GetNewId();
            s.IsHeadingRight = headingRight;
            s.Direction = headingRight ? new Vector(1, 0) : new Vector(-1, 0);

            s.Radius = randomGenerator.Next(SharedDef.MIN_SPHERE_RADIUS, SharedDef.MAX_SPHERE_RADIUS);
            s.Position = new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius)));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));
            s.TextureId = SceneMgr.GetInstance().GetRandomGenerator().Next(1, 18);
            s.Rotation = SceneMgr.GetInstance().GetRandomGenerator().Next(360);

            CreateSphereControls(s);

            s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));

            return s;
        }

        private static Sphere CreateSphereControls(Sphere s)
        {
            /*LinearMovementControl lmc = new LinearMovementControl();
            lmc.InitialSpeed = SceneMgr.GetInstance().GetRandomGenerator().Next(SharedDef.MIN_SPHERE_SPEED * 10, SharedDef.MAX_SPHERE_SPEED * 10) / 10.0f;
            s.AddControl(lmc);*/

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = SceneMgr.GetInstance().GetRandomGenerator().Next(SharedDef.MIN_SPHERE_SPEED * 10, SharedDef.MAX_SPHERE_SPEED * 10) / 10.0f;
            s.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = SceneMgr.GetInstance().GetRandomGenerator().Next(SharedDef.MIN_SPHERE_ROTATION_SPEED, SharedDef.MAX_SPHERE_ROTATION_SPEED) / 10.0f;
            s.AddControl(lrc);

            return s;
        }

        public static Base CreateBase(PlayerData data)
        {
            Base baze = new Base();
            baze.Id = IdMgr.GetNewId();
            baze.BasePosition = data.PlayerPosition;
            baze.Color = data.PlayerColor;
            baze.Integrity = SharedDef.BASE_MAX_INGERITY;
            baze.Position = new Vector(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * ((data.PlayerPosition == PlayerPosition.LEFT) ? 0.1 : 0.6),
                                       SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.85);
            baze.Size = new Size(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * 0.3, 
                                 SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.15);

            baze.SetGeometry(SceneGeometryFactory.CreateLinearGradientRectangleGeometry(baze));

            return baze;
        }

        public static Sphere CreateNewSphereOnEdge(Sphere oldSphere)
        {
            Sphere s = CreateNewRandomSphere(oldSphere.IsHeadingRight);

            Rect actionArea = SceneMgr.GetInstance().GetOrbitArea();

            s.Position = new Vector(s.IsHeadingRight ? (int)(-s.Radius) : (int)(actionArea.Width + s.Radius),
                SceneMgr.GetInstance().GetRandomGenerator().Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius)));

            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Canvas.SetLeft(s.GetGeometry(), s.Position.X - s.Radius);
                Canvas.SetTop(s.GetGeometry(), s.Position.Y - s.Radius);
            }));

            return s;
        }

        public static SingularityMine CreateSingularityMine(Point point, PlayerData plrdata)
        {
            Random randomGenerator = SceneMgr.GetInstance().GetRandomGenerator();
            SingularityMine mine = new SingularityMine();
            mine.Id = IdMgr.GetNewId();
            mine.Position = point.ToVector();
            //mine.Radius = randomGenerator.Next(1, SharedDef.MAX_SPHERE_RADIUS);

            SingularityControl sc = new SingularityControl();
            sc.Speed = plrdata.MineGrowthSpeed;
            sc.Strength = plrdata.MineStrength;
            mine.AddControl(sc);

            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));

            return mine;
        }
    }
}
