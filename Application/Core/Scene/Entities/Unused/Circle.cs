using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
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
    public class Circle : SceneObject
    {
        public Color Color { get; set; }
        public int Radius { get; set; }

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
