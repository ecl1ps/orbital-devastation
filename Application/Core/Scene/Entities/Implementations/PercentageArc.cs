using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Orbit.Core.Client;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class PercentageArc : SceneObject
    {
        public int Radius { get; set; }
        public float Percentage { get; set; }
        public Color Color { get; set; }
        public Point Center { get; set; }

        private ArcSegment arc;

        public PercentageArc(SceneMgr mgr)
            : base(mgr)
        {
            Percentage = 1;
        }

        public Point computePointOnCircle(double angle)
        {
            Point p = new Point();
            p.X = Radius * Math.Cos(angle) + Center.X;
            p.Y = Radius * Math.Sin(angle) + Center.Y - 1;

            return p;
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            if (arc.IsLargeArc && Percentage < 0.5)
                arc.IsLargeArc = false;
            else if (!arc.IsLargeArc && Percentage > 0.5)
                arc.IsLargeArc = true;

            arc.Point = computePointOnCircle((Math.PI * 2) * Percentage);
            Canvas.SetLeft(geometryElement, Position.X);
            Canvas.SetTop(geometryElement, Position.Y);

        }

        public void SetArc(ArcSegment arc)
        {
            this.arc = arc;
        }
    }
}
