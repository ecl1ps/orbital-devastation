using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class AlphaChangingControl : Control
    {
        private float pause = 0f;

        private float elapsedTime;

        public float Time { get; set; }
        public float MinAlpha { get; set; }
        public float MaxAlpha { get; set; }

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            elapsedTime = 0;
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            if (pause < 0)
            {
                SetObjectAlpha(FastMath.LinearInterpolate(MaxAlpha, MinAlpha, elapsedTime / Time));
                pause = 0.5f;
            }
            pause -= tpf;
            elapsedTime += tpf;
            if (elapsedTime > Time)
                elapsedTime = Time;
        }

        private void SetObjectAlpha(double alpha)
        {
            me.SceneMgr.Invoke(new Action(() => me.GetGeometry().Opacity = alpha));
        }
    }
}
