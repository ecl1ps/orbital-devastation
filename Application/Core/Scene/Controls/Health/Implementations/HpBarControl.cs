using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Health;
using Orbit.Core.Scene.Entities;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Health.Implementations
{
    public class HpBarControl : Control
    {
        public Arc HpBar { get; set; }
        private IHpControl control; 

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                if (me != null)
                    HpBar.Visible = value;
            }
        }

        public HpBarControl(Arc bar)
        {
            HpBar = bar;
        }

        protected override void InitControl(Entities.ISceneObject me)
        {
            control = me.GetControlOfType<IHpControl>();
            if (control == null)
                throw new Exception("HpBarControl must be attached to object containing IHpControl");
        }

        protected override void UpdateControl(float tpf)
        {
            float p = control.Hp / (float) control.MaxHp;

            if (p > 1)
                p = 1;
            else if (p < 0)
                p = 0;

            HpBar.EndingAngle = MathHelper.TwoPi * p;

            if (me.Dead)
                HpBar.DoRemoveMe();
        }
    }
}
