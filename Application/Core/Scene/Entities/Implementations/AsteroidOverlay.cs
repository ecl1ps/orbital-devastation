using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Gui.Visuals;
using System.Windows.Media;


namespace Orbit.Core.Scene.Entities.Implementations
{
    public class AsteroidOverlay : Sphere, ITransparent
    {
        public float Opacity { get; set; }

        public AsteroidOverlay(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.ASTEROIDS;
            Opacity = 0; // fully transparent
        }

        protected override void UpdateGeometricState()
        {
            (geometryElement.Children[0] as ImageDrawing).Rect = new Rect(0, 0, Radius * 2, Radius * 2);
            geometryElement.Opacity = Opacity;
        }
    }

}
