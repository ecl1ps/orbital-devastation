using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class PercentageArc : SceneObject, IHpBar, ISpheric
    {
        public int Radius { get; set; }
        public float Percentage { get; set; }
        public Point CenterOfArc { get; set; }

        private bool colorChanged = false;
        private Color color;
        public Color Color { get { return color; } set { color = value; colorChanged = true; } }
        public override Vector Center { get { return new Vector(Position.X + Radius, Position.Y + Radius); } }

        private ArcSegment arc;

        public PercentageArc(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Percentage = 1;
            Category = DrawingCategory.PLAYER_OBJECTS;
            visible = true;
        }

        public Point ComputePointOnCircle(double angle)
        {
            Point p = new Point();
            p.X = Radius * Math.Cos(angle) + CenterOfArc.X;
            p.Y = Radius * Math.Sin(angle) + CenterOfArc.Y - 1;

            return p;
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            if (Visible)
                geometryElement.Opacity = 1;
            else
                geometryElement.Opacity = 0;
                

            if (arc.IsLargeArc && Percentage < 0.5)
                arc.IsLargeArc = false;
            else if (!arc.IsLargeArc && Percentage > 0.5)
                arc.IsLargeArc = true;

            if (colorChanged)
                (geometryElement.Children[0] as GeometryDrawing).Pen = new Pen(new SolidColorBrush(Color), 2);

            arc.Point = ComputePointOnCircle((Math.PI * 2) * Percentage);

            (geometryElement.Transform as TransformGroup).Children.Clear();
            (geometryElement.Transform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));
        }

        public void SetArc(ArcSegment arc)
        {
            this.arc = arc;
        }
    }
}
