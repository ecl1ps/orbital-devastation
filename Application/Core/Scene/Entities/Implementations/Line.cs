using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Players;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities
{
    public class Line : SceneObject
    {
        public Vector Start {get; set; }
        public Vector End { get; set; }
        public Color Color { get; set; }
        public int Width { get; set; }
        public override Vector Center
        {
            get
            {
                return Start + ((Start - End) / 2);
            }
        }

        public Line(SceneMgr mgr) : base(mgr)
        {
        }

        public Line(SceneMgr mgr, Vector start, Vector end, Color color, int width)
            : base(mgr)
        {
            SceneMgr = mgr;
            Start = start;
            End = end;
            Width = width;
            Color = color;
            CreateLine();
        }

        public void CreateLine()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
                line.X1 = Start.X;
                line.Y1 = Start.Y;
                line.X2 = End.X;
                line.Y2 = End.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = Width;
                line.Stroke = new SolidColorBrush(Color);
                line.Fill = new SolidColorBrush(Color);

                geometryElement = line;
            }));
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            // always on screen
            return true;
        }

        public override void UpdateGeometric()
        {
            System.Windows.Shapes.Line line = geometryElement as System.Windows.Shapes.Line;
            line.X1 = Start.X;
            line.Y1 = Start.Y;
            line.X2 = End.X;
            line.Y2 = End.Y;
        }
    }
}
