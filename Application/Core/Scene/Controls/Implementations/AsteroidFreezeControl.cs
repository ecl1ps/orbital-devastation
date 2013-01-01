using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client.Shaders;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class AsteroidFreezeControl : TemporaryControlRemovalControl
    {
        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);
            me.SceneMgr.BeginInvoke(new Action(() =>
            {
                AlphaChannelEffect effect = new AlphaChannelEffect();
                effect.Alpha = 0.5f;
                me.GetGeometry().Effect = effect;
            }));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            me.SceneMgr.BeginInvoke(new Action(() =>
            {
                AlphaChannelEffect effect = new AlphaChannelEffect();
                effect.Alpha = 1.0f;
                me.GetGeometry().Effect = null;
            }));
        }
    }
}
