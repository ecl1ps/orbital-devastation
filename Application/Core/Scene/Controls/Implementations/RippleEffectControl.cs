using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaderEffectLibrary;
using Orbit.Core.Scene.Entities.HeavyWeight;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class RippleEffectControl : Control
    {
        public float Speed { get; set; }
        private RippleEffect effect;

        private new HeavyWeightSceneObject me;

        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);
            if (me is HeavyWeightSceneObject)
                this.me = me as HeavyWeightSceneObject;
            else
                throw new Exception("Effect controls must be attached to heavyWeight objects");
            AddEffect();
        }

        private void AddEffect()
        {
            me.SceneMgr.BeginInvoke(new Action(() =>
            {
                effect = new RippleEffect();
                effect.Amplitude = 0.1;
                effect.Frequency = 180;
                effect.Phase = 0;
                me.HeavyWeightGeometry.Effect = effect;
            }));

        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            me.SceneMgr.BeginInvoke(new Action(() => { effect.Phase -= Speed * tpf; }));
        }
    }
}
