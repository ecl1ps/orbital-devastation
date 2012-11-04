using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Helpers;
using System.Windows;

namespace Orbit.Core.Scene.CollisionShapes
{
    public class SquareCollisionShape : ICollisionShape
    {
        public Vector Position { get; set; }
        public Size Size { get; set; }
        public float Rotation { get; set; }
        public Vector Center
        {
            get
            {
                return new Vector(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
            }
        }

        public bool CollideWith(ICollisionShape other)
        {
            if (other is PointCollisionShape)
                return CollisionHelper.IntersectsPointAndSquare(((PointCollisionShape)other).Center, Position, Size);

            if (other is SphereCollisionShape)
                return CollisionHelper.IntersectsCircleAndSquare((other as SphereCollisionShape).Center, (other as SphereCollisionShape).Radius, 
                    Position, Size);

            if (other is SquareCollisionShape)
                return CollisionHelper.IntersectsSquareAndSquare(GetVertices(), (other as SquareCollisionShape).GetVertices());

            if (other is LineCollisionShape)
                return CollisionHelper.IntersectLineAndSquare((other as LineCollisionShape).Start, (other as LineCollisionShape).End, 
                    Position, Size);

            return false;        
        }

        public Vector[] GetVertices()
        {
            Vector[] vertices = new Vector[4];
            vertices[0] = Position;
            vertices[1] = new Vector(Position.X + Size.Width, Position.Y);
            vertices[2] = new Vector(Position.X + Size.Width, Position.Y + Size.Height);
            vertices[3] = new Vector(Position.X, Position.Y + Size.Height);

            if (Rotation == 0)
                return vertices;

            for (int i = 0; i < vertices.Length; ++i)
                vertices[i] = vertices[i].Rotate(Rotation, Center, false);

            return vertices;
        }
    }
}
