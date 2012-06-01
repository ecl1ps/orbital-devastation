using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities
{
    // Unused
    public abstract class SpherePoint : Sphere
    {
        public override bool CollideWith(ICollidable other) 
        {
            if (other is SpherePoint)
                return CollisionHelper.intersectPointAndPoint(Center, ((SpherePoint)other).Center);

            if (other is Square)
                return CollisionHelper.intersectSquareAndPoint(Center, ((Square)other).Position, ((Square)other).Size);

            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndPoint(Center, ((Sphere)other).Center, ((Sphere)other).Radius);

            return false;
        }
    }

}
