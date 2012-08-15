using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows.Controls;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class OrbitEllipse : SceneObject
    {
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public OrbitEllipse(SceneMgr mgr, float radiusX, float radiusY) : base(mgr)
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            //I am always on screen
            return true;
        }

        public override void UpdateGeometric()
        {
            Canvas.SetLeft(geometryElement, Position.X);
            Canvas.SetTop(geometryElement, Position.Y);
        }
    }
}
