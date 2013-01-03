using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaderEffectLibrary;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class PinchEffectControl : Control
    {
        public float Speed { get; set; }
        public float Radius { get; set; }
        public Vector Position { get; set; }

        private float progress = 0;
        private PinchEffect effect;

        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);

            me.SceneMgr.Invoke(new Action(() =>
            {
                effect = new PinchEffect();
                effect.Radius = Radius;
                effect.Amount = 0;
                effect.CenterX = Position.X / SharedDef.VIEW_PORT_SIZE.Width;
                effect.CenterY = Position.Y / SharedDef.VIEW_PORT_SIZE.Height;

                me.SceneMgr.GetCanvas().Effect = effect;
            }));

            
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);

            if (progress > Speed)
                Destroy();

            progress += tpf;
            AnimateEffect();

        }

        private void AnimateEffect()
        {
            me.SceneMgr.Invoke(new Action(() =>
            {
                effect.Amount = FastMath.LinearInterpolate(0, 15, progress / Speed);
            }));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            me.SceneMgr.Invoke(new Action(() =>
            {
                me.SceneMgr.GetCanvas().Effect = null;
            }));
        }
    }
}
