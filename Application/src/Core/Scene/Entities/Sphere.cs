using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities
{

    public class Sphere : SceneObject, IMovable, ICollidable, IRotable
    {
        public Color Color { get; set; }
        public Vector Direction { get; set; }
        public int Radius { get; set; }
        public bool IsHeadingRight { get; set; }
        public int Rotation { get; set; }

        public bool CollideWith(ICollidable other)
        {
            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndCircle(Position, Radius, (other as Sphere).Position, (other as Sphere).Radius);

            if (other is Base)
                return CollisionHelper.intersectsCircleAndSquare(Position, Radius, (other as Base).Position, (other as Base).Size);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (!(other is Sphere))
                DoRemoveMe();
        }

        public override void UpdateGeometric()
        {
            GeometryElement.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate 
            {
                Canvas.SetLeft(GeometryElement, Position.X - Radius);
                Canvas.SetTop(GeometryElement, Position.Y - Radius);
                (GeometryElement as Image).Width = Radius * 2;
                (GeometryElement as Image).RenderTransform = new RotateTransform(Rotation);
            }));

        }

        public override bool IsOnScreen(Size screenSize)
        {
            if (Position.X <= -Radius || Position.Y <= -Radius)
                return false;

            if (Position.X >= screenSize.Width + Radius || Position.Y >= screenSize.Height + Radius)
                return false;

            return true;
        }

        public override void OnRemove()
        {
            SceneMgr.GetInstance().AttachToScene(SceneObjectFactory.CreateNewSphereOnEdge(this));
        }
    }

}
