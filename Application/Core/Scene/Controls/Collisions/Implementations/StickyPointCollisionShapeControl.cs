using System;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Controls.Collisions.Implementations
{
    public class StickyPointCollisionShapeControl : Control, ICollisionShapeUpdater
    {
        private PointCollisionShape cs;

        protected override void InitControl(ISceneObject me)
        {
            if (!(me.CollisionShape is PointCollisionShape))
                throw new Exception("Collision shape must be Vector2");

            cs = me.CollisionShape as PointCollisionShape;
        }

        protected override void UpdateControl(float tpf)
        {
             cs.Center = me.Center;
        }
    }
}
