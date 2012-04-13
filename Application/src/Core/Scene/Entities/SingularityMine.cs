using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities
{

    public class SingularityMine : SceneObject, ICollidable
    {
        public int Radius { get; set; }
        public bool IsVisible { get; set; }

        public SingularityMine() : base()
        {
            IsVisible = false;
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public bool CollideWith(ICollidable other)
        {
            if (!IsVisible)
                return false;

            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndCircle(Position, Radius, (other as Sphere).Position, (other as Sphere).Radius);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (other is IMovable)
            {
                SingularityControl c = GetControlOfType(typeof(SingularityControl)) as SingularityControl;
                if (c != null)
                    c.CollidedWith(other as IMovable);
            }
        }

        public override void UpdateGeometric()
        {
            if (!IsVisible)
                return;

            GeometryElement.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                ((GeometryElement as Path).Data as EllipseGeometry).RadiusX = Radius;
                ((GeometryElement as Path).Data as EllipseGeometry).RadiusY = Radius;
                (GeometryElement as Path).Stroke = Brushes.Black;
            }));
        }
    }

}
