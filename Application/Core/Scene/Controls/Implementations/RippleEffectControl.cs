using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaderEffectLibrary;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class RippleEffectControl : Control
    {
        public float Speed { get; set; }
        private RippleEffect effect;

        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);
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
                me.GetGeometry().Effect = effect;
            }));

        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
            me.SceneMgr.BeginInvoke(new Action(() => { effect.Phase -= Speed * tpf; }));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            //TODO odebrat efekt z sceneObjectu
        }
    }
}
