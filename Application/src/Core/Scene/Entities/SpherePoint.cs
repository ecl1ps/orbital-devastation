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
    public abstract class SpherePoint : Sphere
    {
        public override bool CollideWith(ICollidable other) 
        {
            if (other is SpherePoint)
                return CollisionHelper.intersectPointAndPoint(Position, ((SpherePoint) other).Position);

            if (other is Square)
                return CollisionHelper.intersectSquareAndPoint(Position, ((Square) other).Position, ((Square) other).Size);

            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndPoint(Position, ((Sphere)other).Position, ((Sphere)other).Radius);

            return false;
        }
    }

}
