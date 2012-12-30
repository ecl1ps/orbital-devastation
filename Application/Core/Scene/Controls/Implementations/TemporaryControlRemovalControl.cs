using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class TemporaryControlRemovalControl : Control
    {
        public IControl ToRemove { get; set; }
        public float Time { get; set; }

        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);
            me.RemoveControl(ToRemove);
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);

            if (Time <= 0)
            {
                me.AddControl(ToRemove);
                Destroy();
            }

            Time -= tpf;
        }
    }
}
