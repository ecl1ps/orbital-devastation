using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Entities;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class StaticFieldControl : Control
    {
        public float LifeTime { get; set; }
        public float Force { get; set; }
        public int Radius { get; set; }

        private SphereCollisionShape collisionShape;

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            collisionShape = new SphereCollisionShape();
            collisionShape.Radius = Radius;
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            if (LifeTime <= 0)
                Destroy();

            collisionShape.Center = me.Center;

            foreach (ISceneObject obj in me.SceneMgr.GetSceneObjects())
            {
                if (obj is IMovable && collisionShape.CollideWith(obj.CollisionShape))
                    collide(obj as IMovable, tpf);
            }

            LifeTime -= tpf;
        }

        private void collide(IMovable obj, float tpf)
        {
            IMovementControl control = obj.GetControlOfType<IMovementControl>();
            if(control == null)
                return;

            Vector v = obj.Center - me.Center;
            v = v.NormalizeV();
            v = v * (Force * tpf);

            Vector dir = obj.Direction * control.Speed;
            v = v + dir;

            control.Speed = (float) v.Length;
            obj.Direction = v.NormalizeV();
        }
    }
}
