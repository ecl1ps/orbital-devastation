using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class GrowingRadiusControl : Control
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private float left = 0;
        public float Time { get; set; }
        public float Radius { get; set; }

        private ISpheric sphere;

        public GrowingRadiusControl(float time, float radius)
        {
            Time = time;
            Radius = radius;
        }

        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);

            if (!(me is ISpheric))
            {
                Logger.Error("ShrinkingControl can be attached only to object with Radius property!");
                return;
            }

            sphere = me as ISpheric;
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);

            if (left >= Time)
            {
                left = Time;
                Grow();
                Destroy();
                return;
            }

            Grow();
            Time -= tpf;
        }

        private void Grow()
        {
            sphere.Radius = (int) FastMath.LinearInterpolate(0, Radius, left / Time);
        }
    }
}
