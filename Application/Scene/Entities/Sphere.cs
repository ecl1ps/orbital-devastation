using System;
using System.Drawing;
using System.Windows;

namespace Orbit.Scene.Entities
{

    public class Sphere : SceneObject, IMovable, ICollidable
    {
        private Color Color { get; set; }
        private Vector direction;
        private uint Radius { get; set; }

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

        public override void Render()
        {
            throw new Exception("Not implemented");
        }

        public override bool IsOnScreen()
        {
            throw new Exception("Not implemented");
        }
    }

}
