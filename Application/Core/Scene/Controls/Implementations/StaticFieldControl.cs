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
    class StaticFieldControl : Control, ICollisionReactionControl
    {
        public float LifeTime { get; set; }
        public float Force { get; set; }

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            if (LifeTime <= 0)
                me.DoRemoveMe();

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

        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (other is IMovable)
                collide(other as IMovable, tpf);
        }
    }
}
