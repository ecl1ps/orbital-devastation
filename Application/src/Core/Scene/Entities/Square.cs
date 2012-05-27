﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities
{
    public abstract class Square : SceneObject, ICollidable
    {
        public Size Size{ get; set; }

        public bool CollideWith(ICollidable other)
        {
            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndSquare((other as Meteor).Position, (other as Meteor).Radius, Position, Size);

            if (other is Square)
                return CollisionHelper.intersectSquareAndSquare(this.Position, this.Size, (other as Square).Position, (other as Square).Size);

            return false;
        }

        public abstract void DoCollideWith(ICollidable other);

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            double halfWidth = Size.Width / 2;
            double halfHeight = Size.Height / 2;

            if(Position.X - halfWidth < 0 || Position.Y - halfHeight < 0)
                return false;

            if ((Position.X + halfWidth > screenSize.Width) || (Position.Y + halfHeight > screenSize.Height))
                return false;

            return true;
        }

    }
}
