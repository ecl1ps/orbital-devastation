using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Helpers;
using System.Windows;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.CollisionShapes
{
    public class PointCollisionShape : ICollisionShape
    {
        public Vector2 Center { get; set; }

        public bool CollideWith(ICollisionShape other)
        {
            if (other is PointCollisionShape)
                return CollisionHelper.IntersectsPointAndPoint(Center, ((PointCollisionShape)other).Center);

            if (other is SquareCollisionShape)
                return CollisionHelper.IntersectsPointAndSquare(Center, ((SquareCollisionShape)other).Position, ((SquareCollisionShape)other).Rectangle);

            if (other is SphereCollisionShape)
                return CollisionHelper.IntersectsCircleAndPoint(Center, ((SphereCollisionShape)other).Center, ((SphereCollisionShape)other).Radius);

            if (other is LineCollisionShape)
                return CollisionHelper.IntersectsPointAndLine((other as LineCollisionShape).Start, (other as LineCollisionShape).End, Center);

            return false;
        }
    }
}
