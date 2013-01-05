using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene.Controls.Collisions;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class StaticFieldControl : Control, ISpheric
    {
        public float LifeTime { get; set; }
        public float Force { get; set; }
        public int Radius { get; set; }

        private SphereCollisionShape shape;

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            shape = new SphereCollisionShape();
            shape.Radius = 200;
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            if (LifeTime <= 0)
                Destroy();

            LifeTime -= tpf;
            shape.Center = me.Center;

            foreach (ISceneObject obj in me.SceneMgr.GetSceneObjects())
            {
                if (obj is IMovable && shape.CollideWith(obj.CollisionShape))
                    Collide(obj as IMovable, tpf);
            }
        }

        private void Collide(IMovable obj, float tpf)
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
