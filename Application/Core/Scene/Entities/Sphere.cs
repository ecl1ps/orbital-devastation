using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;

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
            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndCircle(GetPosition(), Radius, (other as Sphere).GetPosition(), (other as Sphere).Radius);

            if (other is Base)
                return CollisionHelper.intersectsCircleAndSquare(GetPosition(), Radius, (other as Base).GetPosition(), (other as Base).Size);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (!(other is Sphere))
                DoRemoveMe();
        }

        public override void UpdateGeometric()
        {
            geometryElement.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate 
            {
                Canvas.SetLeft(geometryElement, position.X - Radius);
                Canvas.SetTop(geometryElement, position.Y - Radius);
                (geometryElement as Image).Width = Radius * 2;
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
