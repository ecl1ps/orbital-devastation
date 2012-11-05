﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class HpBarControl : Control
    {
        public PercentageArc Bar { get; set;}
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
                        Bar.Visible = value;
                    }));
                }
            }
        }

        public HpBarControl(PercentageArc bar)
        {
            this.Bar = bar;
        }

        protected override void InitControl(Entities.ISceneObject me)
        {
            this.module = me as MiningModule;
        }

        protected override void UpdateControl(float tpf)
        {
            float p = module.Hp / SharedDef.SPECTATOR_MAX_HP;

            if (p > 1)
                p = 1;
            else if (p < 0)
                p = 0;

            Bar.Percentage = p;
        }
    }
}