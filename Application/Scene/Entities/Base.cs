using System;
using Orbit.Player;
using System.Drawing;

namespace Orbit.Scene.Entities
{
    public class Base : SceneObject, ICollidable
    {
        private PlayerPosition BasePosition { get; set; }
        private Color Color { get; set; }
        private int Integrity { get; set; }

        public bool CollideWith(ICollidable other)
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

        public void DoCollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }
    }

}
