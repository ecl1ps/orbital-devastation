using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class OrbitEllipse : SceneObject
    {
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }
        public override Vector Center { get { return new Vector(Position.X + RadiusX, Position.Y + RadiusY); } }

        public OrbitEllipse(SceneMgr mgr, long id, float radiusX, float radiusY)
            : base(mgr, id)
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
            Category = DrawingCategory.PROJECTILES;
        }

        public override bool IsOnScreen(Size screenSize)
        {
            //I am always on screen
            return true;
        }

        public override void UpdateGeometric()
        {
            (geometryElement.Transform as TransformGroup).Children.Clear();
            (geometryElement.Transform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));
        }
    }
}
