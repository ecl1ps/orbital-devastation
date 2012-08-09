using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class SimpleLine : SolidLine
    {
        public SimpleLine(SceneMgr mgr) : base(mgr)
        {
            //default constructor
        }

        public SimpleLine(SceneMgr mgr, Point start, Point end, Color color, int width)
            : base(mgr, start, end, color, width)
        {
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            //i dont collide
        }
    }
}
