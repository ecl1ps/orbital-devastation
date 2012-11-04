using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Helpers;
using System.Windows;

namespace Orbit.Core.Scene.CollisionShapes
{
    public class LineCollisionShape : ICollisionShape
    {
        public Vector Start { get; set; }
        public Vector End { get; set; }

        public bool CollideWith(ICollisionShape other)
        {
            if (other is PointCollisionShape)
                return CollisionHelper.IntersectsPointAndLine(Start, End, 
                    (other as PointCollisionShape).Center);

            if (other is SphereCollisionShape)
                return CollisionHelper.IntersectCircleAndLine(Start, End, 
                    (other as SphereCollisionShape).Center, (other as SphereCollisionShape).Radius);

            if (other is SquareCollisionShape)
                return CollisionHelper.IntersectLineAndSquare(Start, End, 
                    (other as SquareCollisionShape).Position, (other as SquareCollisionShape).Size);

            if (other is LineCollisionShape)
                return CollisionHelper.IntersectLineAndLine(Start, End, 
                    (other as LineCollisionShape).Start, (other as LineCollisionShape).End);

            return false;
        }
    }
}
