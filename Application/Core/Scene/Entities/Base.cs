using System;
using Orbit.Core.Player;
using System.Windows.Media;
using System.Windows;

namespace Orbit.Core.Scene.Entities
{
    public class Base : SceneObject, ICollidable
    {
        public PlayerPosition BasePosition { get; set; }
        public Color Color { get; set; }
        public int Integrity { get; set; }
        public Vector Position { get; set; }
        public Size Size { get; set; }

        public override bool IsOnScreen(Size screenSize)
        {
            return Integrity > 0;
        }

        public bool CollideWith(ICollidable other)
        {
            if (other.GetType() == typeof(Sphere))
                return CollisionHelper.intersectsCircleAndSquare((other as Sphere).GetPosition(), (other as Sphere).Radius, Position, Size);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            Integrity -= 10;
        }

        public override void UpdateGeometric()
        {
            
        }
    }

}
