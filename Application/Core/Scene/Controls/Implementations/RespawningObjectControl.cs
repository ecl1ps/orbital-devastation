﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Controls.Health;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class RespawningObjectControl : Control
    {
        private bool counting = false;
        private float respawningTime = 0;
        private float visibilityTime = 0;

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
                    toggleControls(true);
                    if (hpControl != null)
                        hpControl.RefillHp();

                    me.SceneMgr.Invoke(new Action(() =>
                    {
                        me.Visible = true;
                    }));
                }
                else
                {
                    blinkObject(tpf);
                }
            }
        }

        private void blinkObject(float tpf)
        {
            if (visibilityTime <= 0)
            {
                visibilityTime = 1;
                me.SceneMgr.Invoke(new Action(() => {
                    me.Visible = !me.Visible;
                }));
            }
            else
            {
                visibilityTime -= tpf;
            }
        }

        public void die(float respawningTime)
        {
            if (counting)
                return;

            this.respawningTime = respawningTime;
            counting = true;

            toggleControls(false);
        }

        private void toggleControls(bool enable)
        {
            foreach (Control control in me.GetControlsCopy()) 
            {
                if(control != this)
                    control.Enabled = enable;
            }
        }




    }
}
