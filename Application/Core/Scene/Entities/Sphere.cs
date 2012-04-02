using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities
{

    public class Sphere : SceneObject, IMovable, ICollidable
    {
        public Color Color { get; set; }
        private Vector direction;
        public uint Radius { get; set; }

        public Vector GetDirection()
        {
            return direction;
        }

        public void SetDirection(Vector dir)
        {
            direction = dir;
        }

        public bool CollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }

        public void DoCollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }

        public override void UpdateGeometric()
        {
            path.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate 
            {
                (path.Data as EllipseGeometry).Center = position.ToPoint();
                (path.Data as EllipseGeometry).RadiusX = Radius;
                (path.Data as EllipseGeometry).RadiusY = Radius;
            }));
        }

        public override bool IsOnScreen(Size screenSize)
        {
            if (position.X <= Radius || position.Y <= Radius)
                return false;

            if (position.X >= screenSize.Width + Radius || position.Y >= screenSize.Height + Radius)
                return false;

            return true;
        }
    }

}
