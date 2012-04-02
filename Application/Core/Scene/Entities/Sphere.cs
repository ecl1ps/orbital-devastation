using System;
using System.Windows.Media;
using System.Windows;

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
            (path.Data as EllipseGeometry).Center = GetPosition().ToPoint();
            (path.Data as EllipseGeometry).RadiusX = Radius;
            (path.Data as EllipseGeometry).RadiusY = Radius;
        }

        public override bool IsOnScreen()
        {
            return true;//throw new Exception("Not implemented");
        }
    }

}
