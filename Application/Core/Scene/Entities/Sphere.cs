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
    public abstract class Sphere : SceneObject, ISpheric
    {
        public Color Color { get; set; }
        public int Radius { get; set; }
        public override Vector Center { get { return new Vector(Position.X + Radius, Position.Y + Radius); } }

        public Sphere(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            // objekt se odstrani az je jakoby dvakrat mimo obrazovku (dvakrat jeho sirka)
            if (Position.X <= (-Radius * 4) || Position.Y <= (-Radius * 4))
                return false;

            if (Position.X >= screenSize.Width + Radius * 2 || Position.Y >= screenSize.Height + Radius * 2)
                return false;

            return true;
        }

        /// <summary>
        /// prepisujte jen pokud je to nezbytne, jinak pouzijte UpdateGeometricState()!
        /// </summary>
        public override void UpdateGeometric()
        {
            (geometryElement.Transform as TransformGroup).Children.Clear();
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
