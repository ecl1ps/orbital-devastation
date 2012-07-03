using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core;
using System.Windows.Media;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SpherePoint : Sphere
    {
        public SpherePoint(SceneMgr mgr) : base(mgr)
        {
        }

        public override bool CollideWith(ICollidable other) 
        {
            if (other is SpherePoint)
                return CollisionHelper.IntersectsPointAndPoint(Center, ((SpherePoint)other).Center);

            if (other is Square)
                return CollisionHelper.IntersectsPointAndSquare(Center, ((Square)other).Position, ((Square)other).Size);

            if (other is Sphere)
                return CollisionHelper.IntersectsCircleAndPoint(Center, ((Sphere)other).Center, ((Sphere)other).Radius);

            if (other is SolidLine)
                return CollisionHelper.IntersectsPointAndLine((other as SolidLine).Start, (other as SolidLine).End, Center.ToPoint());

            return false;
        }
    }

}
