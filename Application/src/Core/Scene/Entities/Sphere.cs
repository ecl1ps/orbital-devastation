using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Orbit.src.Core.Scene.Entities;

namespace Orbit.Core.Scene.Entities
{
    public abstract class Sphere : SceneObject, IMovable, ICollidable
    {
        public Color Color { get; set; }
        public Vector Direction { get; set; } 
        public int Radius { get; set; }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            if (Position.X <= -Radius || Position.Y <= -Radius)
                return false;

            if (Position.X >= screenSize.Width + Radius || Position.Y >= screenSize.Height + Radius)
                return false;

            return true;
        }

        public override void UpdateGeometric()
        {
            geometryElement.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate
            {
                Canvas.SetLeft(geometryElement, Position.X - Radius);
                Canvas.SetTop(geometryElement, Position.Y - Radius);
                if (geometryElement is Image)
                    (geometryElement as Image).Width = Radius * 2;
                UpdateGeometricState();
            }));
        }

        protected abstract void UpdateGeometricState();

        public virtual bool CollideWith(ICollidable other)
        {
            if (other is SpherePoint)
                return CollisionHelper.intersectsCircleAndPoint(((SpherePoint)other).Position, Position, Radius);

            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndCircle(Position, Radius, (other as Sphere).Position, (other as Sphere).Radius);

            if (other is Base)
                return CollisionHelper.intersectsCircleAndSquare(Position, Radius, (other as Base).Position, (other as Base).Size);

            return false;
        }

        public abstract void DoCollideWith(ICollidable other);
    }
}
