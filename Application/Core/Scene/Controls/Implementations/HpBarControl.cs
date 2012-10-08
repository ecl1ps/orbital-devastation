using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class HpBarControl : Control
    {
        private PercentageArc bar;
        private MiningModule module;

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
                        bar.Visible = value;
                    }));
                }
            }
        }

        public HpBarControl(PercentageArc bar)
        {
            this.bar = bar;
        }

        public override void InitControl(Entities.ISceneObject me)
        {
            this.module = me as MiningModule;
        }

        public override void UpdateControl(float tpf)
        {
            float p = module.Hp / SharedDef.SPECTATOR_MAX_HP;

            if (p > 1)
                p = 1;
            else if (p < 0)
                p = 0;

            bar.Percentage = p;
        }
    }
}
