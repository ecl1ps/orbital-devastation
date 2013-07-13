using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Helpers;
using System.Windows;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.CollisionShapes
{
    public class SquareCollisionShape : ICollisionShape
    {
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; set; }
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + Rectangle.Width / 2, Position.Y + Rectangle.Height / 2);
            }
        }

        public bool CollideWith(ICollisionShape other)
        {
            if (other is PointCollisionShape)
                return CollisionHelper.IntersectsPointAndSquare(((PointCollisionShape)other).Center, Position, Rectangle);

            if (other is SphereCollisionShape)
                return CollisionHelper.IntersectsCircleAndSquare((other as SphereCollisionShape).Center, (other as SphereCollisionShape).Radius, 
                    Position, Rectangle);

            if (other is SquareCollisionShape)
                return CollisionHelper.IntersectsSquareAndSquare(GetVertices(), (other as SquareCollisionShape).GetVertices());

            if (other is LineCollisionShape)
                return CollisionHelper.IntersectLineAndSquare((other as LineCollisionShape).Start, (other as LineCollisionShape).End, 
                    Position, Rectangle);

            return false;        
        }

        public Vector2[] GetVertices()
        {
            Vector2[] vertices = new Vector2[4];
            vertices[0] = Position;
            vertices[1] = new Vector2(Position.X + Rectangle.Width, Position.Y);
            vertices[2] = new Vector2(Position.X + Rectangle.Width, Position.Y + Rectangle.Height);
            vertices[3] = new Vector2(Position.X, Position.Y + Rectangle.Height);

            if (Rotation == 0)
                return vertices;

            for (int i = 0; i < vertices.Length; ++i)
                vertices[i] = vertices[i].Rotate(Rotation, Center, false);

            return vertices;
        }
    }
}
