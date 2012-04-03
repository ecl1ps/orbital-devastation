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
        public int Radius { get; set; }
        public bool IsHeadingRight { get; set; }

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
            if (other.GetType() == typeof(Sphere))
                return CollisionHelper.intersectsCircleAndCircle(GetPosition(), Radius, (other as Sphere).GetPosition(), (other as Sphere).Radius);

            if (other.GetType() == typeof(Base))
                return CollisionHelper.intersectsCircleAndSquare(GetPosition(), Radius, (other as Base).Position, (other as Base).Size);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (other.GetType() == typeof(Sphere))
                ;
            else
                DoRemoveMe();
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
            if (position.X <= -Radius || position.Y <= -Radius)
                return false;

            if (position.X >= screenSize.Width + Radius || position.Y >= screenSize.Height + Radius)
                return false;

            return true;
        }

        public override void OnRemove()
        {
            SceneMgr.GetInstance().AttachToScene(SceneObjectFactory.CreateNewSphereOnEdge(this));
        }
    }

}
