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

namespace Orbit.Core.Scene.Entities
{
    public abstract class Square : SceneObject, ICollidable
    {
        public Size Size { get; set; }
        public Vector Center
        {
            get
            {
                return new Vector(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
            }
        }
        
        public Square(SceneMgr mgr) : base(mgr)
        {
        }

        public bool CollideWith(ICollidable other)
        {
            if (other is SpherePoint)
                return CollisionHelper.IntersectsPointAndSquare(((SpherePoint)other).Center, Position, Size);

            if (other is Sphere)
                return CollisionHelper.IntersectsCircleAndSquare((other as Sphere).Center, (other as Sphere).Radius, Position, Size);

            if (other is Square)
                return CollisionHelper.IntersectsSquareAndSquare(GetVertices(), (other as Square).GetVertices());

            return false;
        }

        public abstract void DoCollideWith(ICollidable other);

        public override bool IsOnScreen(Size screenSize)
        {
            if (Position.X + 2 * Size.Width + 5 < 0 || Position.Y + Size.Height + 5 < 0)
                return false;

            if ((Position.X - Size.Width - 5 > screenSize.Width) || (Position.Y + 5 > screenSize.Height))
                return false;

            return true;
        }

        public Vector[] GetVertices()
        {
            Vector[] vertices = new Vector[4];
            vertices[0] = Position;
            vertices[1] = new Vector(Position.X + Size.Width, Position.Y);
            vertices[2] = new Vector(Position.X + Size.Width, Position.Y + Size.Height);
            vertices[3] = new Vector(Position.X, Position.Y + Size.Height);

            if (!(this is IRotable) || (this as IRotable).Rotation == 0)
                return vertices;

            for (int i = 0; i < vertices.Length; ++i)
                vertices[i] = vertices[i].Rotate((this as IRotable).Rotation, Center, false);

            return vertices;
        }
    }
}
