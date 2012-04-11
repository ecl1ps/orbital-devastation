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
using System.Windows.Media.Imaging;
using System.Windows.Controls;

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
            s.SetDirection(headingRight ? new Vector(1, 0) : new Vector(-1, 0));

            s.Radius = randomGenerator.Next(SharedDef.MIN_SPHERE_RADIUS, SharedDef.MAX_SPHERE_RADIUS);
            s.SetPosition(new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius))));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));
            s.SetRotation(SceneMgr.GetInstance().GetRandomGenerator().Next(360));

            LinearMovementControl lmc = new LinearMovementControl();
            lmc.Speed = randomGenerator.Next(SharedDef.MIN_SPHERE_SPEED * 10, SharedDef.MAX_SPHERE_SPEED * 10) / 10.0f;
            s.AddControl(lmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = randomGenerator.Next(SharedDef.MIN_SPHERE_ROTATION_SPEED, SharedDef.MAX_SPHERE_ROTATION_SPEED) / 10.0f;
            s.AddControl(lrc);

            s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));

            return s;
        }

        public static Base CreateBase(PlayerPosition pos, Color col)
        {
            Base baze = new Base();
            baze.Setid(IdMgr.GetNewId());
            baze.BasePosition = pos;
            baze.Color = col;
            baze.Integrity = SharedDef.BASE_MAX_INGERITY;

            baze.SetPosition(new Vector(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * ((pos == PlayerPosition.LEFT) ? 0.1 : 0.6),
                                       SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.85));
            baze.Size = new Size(SceneMgr.GetInstance().ViewPortSizeOriginal.Width * 0.3, 
                                 SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.15);

            baze.SetGeometry(SceneGeometryFactory.CreateLinearGradientRectangleGeometry(baze));

            return baze;
        }

        public static Sphere CreateNewSphereOnEdge(Sphere oldSphere)
        {
            Sphere s = CreateNewRandomSphere(oldSphere.IsHeadingRight);

            Rect actionArea = SceneMgr.GetInstance().GetActionArea();

            s.SetPosition(new Vector(s.IsHeadingRight ? (int)(-s.Radius) : (int)(actionArea.Width + s.Radius),
                SceneMgr.GetInstance().GetRandomGenerator().Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius))));

            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Canvas.SetLeft(s.GetGeometry(), s.GetPosition().X - s.Radius);
                Canvas.SetTop(s.GetGeometry(), s.GetPosition().Y - s.Radius);
            }));

            return s;
        }

        public static SingularityMine CreateSingularityMine(Point point, PlayerData plrdata)
        {
            Random randomGenerator = SceneMgr.GetInstance().GetRandomGenerator();
            SingularityMine mine = new SingularityMine();
            mine.Setid(IdMgr.GetNewId());
            mine.SetPosition(point.ToVector());
            //mine.Radius = randomGenerator.Next(1, SharedDef.MAX_SPHERE_RADIUS);

            SingularityControl sc = new SingularityControl();
            sc.Speed = plrdata.GetMineGrowthSpeed();
            sc.Strength = plrdata.GetMineStrength();
            mine.AddControl(sc);

            mine.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(mine));

            return mine;
        }
    }
}
