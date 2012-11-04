using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.CollisionShapes;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Collisions.Implementations
{
    public class StickySquareCollisionShapeControl : Control, ICollisionShapeUpdater
    {
        private SquareCollisionShape cs;

        protected override void InitControl(ISceneObject me)
        {
            if (!(me.CollisionShape is SquareCollisionShape))
                throw new Exception("Collision shape must be Square");

            cs = me.CollisionShape as SquareCollisionShape;
        }

        protected override void UpdateControl(float tpf)
        {
            cs.Rotation = (me is IRotable) ? (me as IRotable).Rotation : 0;

            if (typeof(Sphere).IsAssignableFrom(me.GetType()))
            {
                cs.Size = new Size((me as Sphere).Radius * 2, (me as Sphere).Radius * 2);
                //TODO: check
                cs.Position = me.Position;
            }
            else if (typeof(Square).IsAssignableFrom(me.GetType()))
            {
                cs.Size = (me as Square).Size;
                cs.Position = me.Position;
            }
            else if (typeof(Line).IsAssignableFrom(me.GetType()))
            {
                //cs.Size = ;
                //TODO:
                cs.Position = me.Position;
            }
        }
    }
}
