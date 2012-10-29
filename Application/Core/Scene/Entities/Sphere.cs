using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Entities
{
    public abstract class Sphere : SceneObject, IMovable, ICollidable
    {
        public Color Color { get; set; }
        private Vector direction;
        public Vector Direction { get; set; }
        public int Radius { get; set; }
        public bool HasPositionInCenter { get; set; }
        public Vector Center
        {
            get
            {
                if (!HasPositionInCenter) // image ma Position v levem hornim rohu
                    return new Vector(Position.X + Radius, Position.Y + Radius);
                else
                    return Position; // EllipseGeometry ma Position ve stredu
            }
        }

        public Sphere(SceneMgr mgr) : base(mgr)
        {
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            // 5 je tolerance, ktera zabranuje nepresnostem na rozhrani
            if (Center.X <= -(Radius * 3 + 5) || Center.Y <= -(Radius + 5))
                return false;

            if (Center.X >= screenSize.Width + Radius * 3 + 5 || Center.Y >= screenSize.Height + Radius + 5)
                return false;

            return true;
        }

        public sealed override void UpdateGeometric()
        {
            Canvas.SetLeft(geometryElement, Position.X);
            Canvas.SetTop(geometryElement, Position.Y);
            if (geometryElement is Image)
                (geometryElement as Image).Width = Radius * 2;
            UpdateGeometricState();
        }

        /// <summary>
        /// tato metoda je volana ve vlaknu GUI -> NEPOSILAT PRES DISPATCHER
        /// </summary>
        protected virtual void UpdateGeometricState() 
        {
        }

        public virtual bool CollideWith(ICollidable other)
        {
            if (other is SpherePoint)
                return CollisionHelper.IntersectsCircleAndPoint(((SpherePoint)other).Center, Center, Radius);

            if (other is Sphere)
                return CollisionHelper.IntersectsCircleAndCircle(Center, Radius, (other as Sphere).Center, (other as Sphere).Radius);

            if (other is Square)
                return CollisionHelper.IntersectsCircleAndSquare(Center, Radius, (other as Square).Position, (other as Square).Size);

            if (other is SolidLine)
                return CollisionHelper.IntersectCircleAndLine((other as SolidLine).Start, (other as SolidLine).End, Center.ToPoint(), Radius);
            
            return false;
        }

        public abstract void DoCollideWith(ICollidable other, float tpf);
    }
}
