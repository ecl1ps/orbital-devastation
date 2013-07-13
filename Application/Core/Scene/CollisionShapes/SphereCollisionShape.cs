﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Helpers;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.CollisionShapes
{
    public class SphereCollisionShape : ICollisionShape
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public bool CollideWith(ICollisionShape other)
        {
            if (other is PointCollisionShape)
                return CollisionHelper.IntersectsCircleAndPoint(((PointCollisionShape)other).Center, Center, Radius);

            if (other is SphereCollisionShape)
                return CollisionHelper.IntersectsCircleAndCircle(Center, Radius, 
                    (other as SphereCollisionShape).Center, (other as SphereCollisionShape).Radius);

            if (other is SquareCollisionShape)
                return CollisionHelper.IntersectsCircleAndSquare(Center, Radius, 
                    (other as SquareCollisionShape).Position, (other as SquareCollisionShape).Rectangle);

            if (other is LineCollisionShape)
                return CollisionHelper.IntersectCircleAndLine((other as LineCollisionShape).Start, (other as LineCollisionShape).End, 
                    Center, Radius);

            return false;
        }
    }
}
