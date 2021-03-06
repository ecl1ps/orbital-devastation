﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Entities.Implementations
{
    /// <summary>
    /// unused
    /// </summary>
    public class VectorLine : SceneObject, IMovable
    {
        public Color Color { get; set; }
        public override Vector Center { get { return new Vector(Position.X, Position.Y); } }

        public VectorLine(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            /*(geometryElement as System.Windows.Shapes.Line).X1 = Position.X;
            (geometryElement as System.Windows.Shapes.Line).Y1 = Position.Y;
            (geometryElement as System.Windows.Shapes.Line).X2 = Position.X + Direction.X;
            (geometryElement as System.Windows.Shapes.Line).Y2 = Position.Y + Direction.Y;*/
        }

    }
}
