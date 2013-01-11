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
    public abstract class Square : SceneObject
    {
        public Size Size { get; set; }
        public override Vector Center { get { return new Vector(Position.X + Size.Width / 2, Position.Y + Size.Height / 2); } }

        public Square(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        /// <summary>
        /// prepisujte jen pokud je to nezbytne, jinak pouzijte UpdateGeometricState()!
        /// </summary>
        public override void UpdateGeometric()
        {
            (geometryElement.Transform as TransformGroup).Children.Clear();
            (geometryElement.Transform as TransformGroup).Children.Add(new RotateTransform(Rotation, Size.Width / 2, Size.Height / 2));
            (geometryElement.Transform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));
            UpdateGeometricState();
        }

        /// <summary>
        /// tato metoda je volana ve vlaknu GUI -> NEPOSILAT PRES DISPATCHER
        /// </summary>
        protected virtual void UpdateGeometricState()
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            // objekt se odstrani az je jakoby dvakrat mimo obrazovku (dvakrat jeho sirka resp vyska)
            if (Position.X <= (- Size.Width * 2) || Position.Y <= (- Size.Height * 2))
                return false;

            if (Position.X >= screenSize.Width + Size.Width || Position.Y >= screenSize.Height + Size.Height)
                return false;

            return true;
        }
    }
}
