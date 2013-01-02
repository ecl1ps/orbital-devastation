using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class PercentageEllipse : SceneObject, IHpBar
    {
        public float A { get; set; }
        public float B { get; set; }
        public float FullAngle { get; set; }
        public float Percentage { get; set; }
        public Point StartPoint { get; set; }

        private bool colorChanged = false;
        private Color color;
        public Color Color { get { return color; } set { color = value; colorChanged = true; } }

        private ArcSegment arc;

        public PercentageEllipse(SceneMgr mgr, long id)
            : base(mgr, id)
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

            if (arc.IsLargeArc && angle < -(Math.PI * 0.5))
                arc.IsLargeArc = false;
            else if (!arc.IsLargeArc && angle > -(Math.PI * 0.5))
                arc.IsLargeArc = true;

            if (colorChanged)
                (geometryElement.Children[0] as GeometryDrawing).Pen = new Pen(new SolidColorBrush(Color), 2);

            arc.Point = ComputeEllipsePoint(angle);

            (geometryElement.Transform as TransformGroup).Children.Clear();
            (geometryElement.Transform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));
        }

        public void SetArc(ArcSegment arc)
        {
            this.arc = arc;
        }
    }
}
