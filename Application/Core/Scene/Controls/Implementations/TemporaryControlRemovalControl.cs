using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class TemporaryControlRemovalControl : Control
    {
        public List<IControl> ToRemove { get; set; }
        public float Time { get; set; }

        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);

            ToRemove.ForEach(control => me.RemoveControl(control));
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);

            if (Time <= 0)
            {
                ToRemove.ForEach(control => me.AddControl(control));
                Destroy();
            }

            Time -= tpf;
        }
    }
}
