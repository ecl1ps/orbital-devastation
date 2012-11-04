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

namespace Orbit.Core.Scene.Entities
{
    public abstract class Square : SceneObject
    {
        public Size Size { get; set; }
        public override Vector Center
        {
            get
            {
                return new Vector(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
            }
        }
        
        public Square(SceneMgr mgr) : base(mgr)
        {
        }

        public sealed override void UpdateGeometric()
        {
            Canvas.SetLeft(geometryElement, Position.X);
            Canvas.SetTop(geometryElement, Position.Y);
            UpdateGeometricState();
        }

        /// <summary>
        /// tato metoda je volana ve vlaknu GUI -> NEPOSILAT PRES DISPATCHER
        /// </summary>
        protected virtual void UpdateGeometricState()
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            if (Position.X + 2 * Size.Width + 5 < 0 || Position.Y + Size.Height + 5 < 0)
                return false;

            if ((Position.X - Size.Width - 5 > screenSize.Width) || (Position.Y + 5 > screenSize.Height))
                return false;

            return true;
        }
    }
}
