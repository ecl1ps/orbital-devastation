using System;
using Orbit.Core.Player;
using System.Drawing;

namespace Orbit.Core.Scene.Entities
{
    public class Base : SceneObject, ICollidable
    {
        public PlayerPosition BasePosition { get; set; }
        public Color Color { get; set; }
        public int Integrity { get; set; }

        public override void Render()
        {
            throw new Exception("Not implemented");
        }

        public override bool IsOnScreen()
        {
            return Integrity > 0;
        }

        public bool CollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }

        public void DoCollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }
    }

}
