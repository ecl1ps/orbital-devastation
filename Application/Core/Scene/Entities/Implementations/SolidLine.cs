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
        public Player Owner { get; set; }

        private Line line;


        public SolidLine(SceneMgr mgr, Player owner, Point start, Point end, Color color) : base(mgr)
        {
            SceneMgr = mgr;
            Owner = owner;
            CreateLine(color);
        }

        private void CreateLine(Color color)
        {
            SceneMgr.Invoke(new Action(() =>
            {
                line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = Start.X;
                line.Y1 = Start.Y;
                line.X2 = End.X;
                line.Y2 = End.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 1;
                line.Fill = new SolidColorBrush(color);
            }));
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            //i am always on screen
            return true;
        }

        public override void UpdateGeometric()
        {
            line.X1 = Start.X;
            line.Y1 = Start.Y;
            line.X2 = End.X;
            line.Y2 = End.Y;
        }

        public bool CollideWith(ICollidable other)
        {
            throw new NotImplementedException();
        }

        public void DoCollideWith(ICollidable other)
        {
            throw new NotImplementedException();
        }
    }
}
