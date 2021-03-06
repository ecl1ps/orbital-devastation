﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Controls;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class BaseIntegrityBar : SceneObject, IHpBar
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float FullAngle { get; set; }
        public float Percentage { get; set; }
        public Point StartPoint { get; set; }

        private bool colorChanged = false;
        private Color color;
        public Color Color { get { return color; } set { color = value; colorChanged = true; } }
        public override Vector Center { get { return new Vector(Position.X + Width / 2, Position.Y + Height / 2); } }

        private ArcSegment arc;

        public BaseIntegrityBar(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Percentage = 1;
            Category = DrawingCategory.BACKGROUND;
        }

        public Point ComputeEllipsePoint(float angle)
        {
            double x = Width * Math.Cos(angle);
            double y = Height * Math.Sin(angle);

            return new Point(x, y);
        }

        public override bool IsOnScreen(Size screenSize)
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
                (geometryElement.Children[0] as GeometryDrawing).Brush = new SolidColorBrush(Color);

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
