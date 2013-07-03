using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Controls.Collisions.Implementations
{
    public class StickySphereCollisionShapeControl : Control, ICollisionShapeUpdater
    {
        private SphereCollisionShape cs;

        protected override void InitControl(ISceneObject me)
        {
            if (!(me.CollisionShape is SphereCollisionShape))
                throw new Exception("Collision shape must be Sphere");

            cs = me.CollisionShape as SphereCollisionShape;
        }

        protected override void UpdateControl(float tpf)
        {
            cs.Center = me.Center;
            if (typeof(Sphere).IsAssignableFrom(me.GetType()))
                cs.Radius = (me as Sphere).Radius;
            else if (typeof(Square).IsAssignableFrom(me.GetType()))
                cs.Radius = (int)Math.Sqrt(Math.Pow((double)(me as Square).Rectangle.Width, 2) + Math.Pow((double)(me as Square).Rectangle.Height, 2)) / 2;
            else if (typeof(Line).IsAssignableFrom(me.GetType()))
                cs.Radius = (int)((me as Line).Start - (me as Line).End).Length();
        }
    }
}
