using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class SolidLine : SceneObject, IProjectile, ICollidable
    {
        public Point Start {get; set; }
        public Point End { get; set; }
        public int Width { get; set; }
        public Player Owner { get; set; }

        public SolidLine(SceneMgr mgr, Player owner, Point start, Point end, Color color, Brush brush, int width) : base(mgr)
        {
            SceneMgr = mgr;
            Owner = owner;
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
            SceneMgr.Invoke(new Action(() =>
            {
                Line line = geometryElement as Line;
                line.X1 = Start.X;
                line.Y1 = Start.Y;
                line.X2 = End.X;
                line.Y2 = End.Y;
            }));
        }

        public bool CollideWith(ICollidable other)
        {
            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
        }
    }
}
