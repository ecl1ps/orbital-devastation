using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class PercentageEllipse : SceneObject, IHpBar
    {
        public float A {get; set;}
        public float B { get; set; }
        public float FullAngle { get; set; }
        public float Percentage { get; set; }

        private bool colorChanged = false;
        private Color color;
        public Color Color { get { return color; } set { color = value; colorChanged = true; } }

        private ArcSegment arc;

        public PercentageEllipse(SceneMgr mgr) : base(mgr)
        {
            Percentage = 1;
        }

        public Point ComputeEllipsePoint(float angle)
        {
            double x = A * Math.Cos(angle);
            double y = B * Math.Sin(angle);

            return new Point(x, y);
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            float angle = - (FullAngle - (FullAngle * Percentage));

            if (arc.IsLargeArc && angle < Math.PI)
                arc.IsLargeArc = false;
            else if (!arc.IsLargeArc && angle > Math.PI)
                arc.IsLargeArc = true;

            if (colorChanged)
                (geometryElement as Path).Stroke = new SolidColorBrush(Color);

            arc.Point = ComputeEllipsePoint(angle);
            Canvas.SetLeft(geometryElement, Position.X);
            Canvas.SetTop(geometryElement, Position.Y);
        }

        public void SetArc(ArcSegment arc)
        {
            this.arc = arc;
        }

    }
}
