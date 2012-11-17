using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Health;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Health.Implementations
{
    class HpBarControl : Control
    {
        public IHpBar Bar { get; set;}
        private IHpControl obj;

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
                {
                    me.SceneMgr.Invoke(new Action(() =>
                    {
                        Bar.Visible = value;
                    }));
                }
            }
        }

        public HpBarControl(IHpBar bar)
        {
            this.Bar = bar;
        }

        protected override void InitControl(Entities.ISceneObject me)
        {
            this.obj = me.GetControlOfType<IHpControl>();
            if (this.obj == null)
                throw new Exception("HpBarControl must be attached to object containing HpControl");
        }

        protected override void UpdateControl(float tpf)
        {
            float p = obj.Hp / (float) obj.MaxHp;

            if (p > 1)
                p = 1;
            else if (p < 0)
                p = 0;

            Bar.Percentage = p;
        }
    }
}
