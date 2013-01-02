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
        public override Vector Center
        {
            get
            {
                return Start + ((Start - End) / 2);
            }
        }

        public Line(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public Line(SceneMgr mgr, long id, Vector start, Vector end, Color color, int width)
            : base(mgr, id)
        {
            SceneMgr = mgr;
            Start = start;
            End = end;

            geometryElement = SceneGeometryFactory.CreateLineGeometry(SceneMgr, color, width, color, Start, End);
        }

        public override bool IsOnScreen(Size screenSize)
        {
            // always on screen
            return true;
        }

        public override void UpdateGeometric()
        {
            LineGeometry line = (geometryElement.Children[0] as GeometryDrawing).Geometry as LineGeometry;
            line.StartPoint = Start.ToPoint();
            line.EndPoint = End.ToPoint();
        }
    }
}
