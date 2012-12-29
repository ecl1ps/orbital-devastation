using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Health;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class RespawningObjectControl : Control
    {
        private bool counting = false;
        private float respawningTime = 0;
        private float visibilityTime = 0;
        private float initRespawningTime = 0;

        private IHpControl hpControl;

        protected override void InitControl(ISceneObject me)
        {
            hpControl = me.GetControlOfType<IHpControl>();
        }

        protected override void UpdateControl(float tpf)
        {
            if (counting)
            {
                respawningTime -= tpf;
                if (respawningTime <= 0)
                {
                    counting = false;
                    me.ToggleControls(true);
                    if (hpControl != null)
                        hpControl.RefillHp();

                    me.Visible = true;
                }
                else
                {
                    BlinkObject(tpf);
                }
            }
        }

        private void BlinkObject(float tpf)
        {
            if (visibilityTime <= 0)
            {
                visibilityTime = 0.1f + (0.9f * respawningTime / initRespawningTime);
                me.Visible = !me.Visible;
            }
            else
            {
                visibilityTime -= tpf;
            }
        }

        public void Die(float respawningTime)
        {
            if (counting)
                return;

            this.respawningTime = respawningTime;
            this.initRespawningTime = respawningTime;
            counting = true;

            me.ToggleControls(false, this);
        }
    }
}
