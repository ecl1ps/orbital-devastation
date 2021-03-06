using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using Lidgren.Network;
using System.Collections.Generic;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities.Implementations
{
    /// <summary>
    /// unused
    /// </summary>
    public class Circle : SceneObject, ISpheric
    {
        public Color Color { get; set; }
        public int Radius { get; set; }
        public override Vector Center { get { return new Vector(Position.X + Radius, Position.Y + Radius); } }

        public Circle(SceneMgr mgr, long id)
            : base(mgr, id)
        {

        }

        public override void UpdateGeometric()
        {
            (geometryElement.Transform as TransformGroup).Children.Clear();
            (geometryElement.Transform as TransformGroup).Children.Add(new RotateTransform(Rotation));
            (geometryElement.Transform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }
    }
}
