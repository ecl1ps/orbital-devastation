using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Helpers;
using System.Windows;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.CollisionShapes
{
    public class LineCollisionShape : ICollisionShape
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

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
                    (other as SquareCollisionShape).Position, (other as SquareCollisionShape).Rectangle);

            if (other is LineCollisionShape)
                return CollisionHelper.IntersectLineAndLine(Start, End, 
                    (other as LineCollisionShape).Start, (other as LineCollisionShape).End);

            return false;
        }
    }
}
