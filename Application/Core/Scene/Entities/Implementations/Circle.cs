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

    public class Circle : SceneObject
    {
        public Color Color { get; set; }
        public int Radius { get; set; }

        public Circle(SceneMgr mgr) : base(mgr)
        {

        }

        public override void UpdateGeometric()
        {
            geometryElement.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                Canvas.SetLeft(geometryElement, Position.X);
                Canvas.SetTop(geometryElement, Position.Y);
            }));
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }
    }
}
