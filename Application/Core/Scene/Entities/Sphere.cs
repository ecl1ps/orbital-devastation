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
    public abstract class Sphere : SceneObject
    {
        public Color Color { get; set; }
        public int Radius { get; set; }
        public bool HasPositionInCenter { get; set; }
        public override Vector Center
        {
            get
            {
                if (!HasPositionInCenter) // image ma Position v levem hornim rohu
                    return new Vector(Position.X + Radius, Position.Y + Radius);
                else
                    return Position; // EllipseGeometry ma Position ve stredu
            }
        }

        public Sphere(SceneMgr mgr, long id)
            : base(mgr, id)
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
            (geometryElement.Transform as TransformGroup).Children.Clear();

            if (HasPositionInCenter)
                (geometryElement.Transform as TransformGroup).Children.Add(new RotateTransform(Rotation));
            else
                (geometryElement.Transform as TransformGroup).Children.Add(new RotateTransform(Rotation, Radius, Radius));

            (geometryElement.Transform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));

            UpdateGeometricState();
        }

        /// <summary>
        /// tato metoda je volana ve vlaknu GUI -> NEPOSILAT PRES DISPATCHER
        /// </summary>
        protected virtual void UpdateGeometricState() 
        {
        }
    }
}
