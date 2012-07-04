using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows.Shapes;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SolidLine : SceneObject, ICollidable
    {
        public Point Start {get; set; }
        public Point End { get; set; }
        public int Width { get; set; }

        public SolidLine(SceneMgr mgr, Point start, Point end, Color color, Brush brush, int width) : base(mgr)
        {
            SceneMgr = mgr;
            Start = start;
            End = end;
            Width = width;
            CreateLine(color, brush);
        }

        private void CreateLine(Color color, Brush brush)
        {
            SceneMgr.Invoke(new Action(() =>
            {
                Line line = new Line();
                line.Stroke = brush;
                line.X1 = Start.X;
                line.Y1 = Start.Y;
                line.X2 = End.X;
                line.Y2 = End.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = Width;
                line.Fill = new SolidColorBrush(color);

                geometryElement = line;
            }));
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            //i am always on screen
            return true;
        }

        public override void UpdateGeometric()
        {
            Line line = geometryElement as Line;
            line.X1 = Start.X;
            line.Y1 = Start.Y;
            line.X2 = End.X;
            line.Y2 = End.Y;
        }

        public bool CollideWith(ICollidable other)
        {
            if (other is SpherePoint)
                return CollisionHelper.IntersectsPointAndLine(Start, End, (other as SpherePoint).Center.ToPoint());
            if (other is Sphere)
                return CollisionHelper.IntersectCircleAndLine(Start, End, (other as Sphere).Center.ToPoint(), (other as Sphere).Radius);
            if(other is Square)
                return CollisionHelper.IntersectLineAndSquare(Start, End, (other as Square).Position.ToPoint(), (other as Square).Size);
            if (other is SolidLine)
                return CollisionHelper.IntersectLineAndLine(Start, End, (other as SolidLine).Start, (other as SolidLine).End);

            return false;
        }

        public abstract void DoCollideWith(ICollidable other, float tpf);
    }
}
