using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Reflection;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ShrinkingControl : Control
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private float overTime;
        private float currentTime;
        private int startingRadius;
        private int finalRadius;
        private ISpheric sphere;
        private bool shouldAdjustPos;

        public ShrinkingControl(int finalRadius, float overTime)
        {
            this.overTime = overTime;
            this.currentTime = overTime;
            this.finalRadius = finalRadius;
        }

        protected override void InitControl(ISceneObject obj)
        {
            if (!(obj is ISpheric))
            {
                Logger.Error("ShrinkingControl can be attached only to object with Radius property!");
                return;
            }

            sphere = obj as ISpheric;
            startingRadius = sphere.Radius;

            PropertyInfo hasPositionInCenter = sphere.GetType().GetProperty("HasPositionInCenter");
            if (hasPositionInCenter == null)
                shouldAdjustPos = true;
            else
                shouldAdjustPos = !(bool)hasPositionInCenter.GetValue(sphere, null);
                
        }

        protected override void UpdateControl(float tpf)
        {
            if (currentTime <= 0)
                return;

            currentTime -= tpf;

            int newRadius = (int)FastMath.LinearInterpolate(finalRadius, startingRadius, currentTime / overTime);

            if (shouldAdjustPos)
            {
                me.Position = new Vector(me.Position.X + sphere.Radius - newRadius, me.Position.Y + sphere.Radius - newRadius);
                PositionCloneControl pcc = me.GetControlOfType<PositionCloneControl>();
                if (pcc != null)
                    pcc.Offset = new Vector(pcc.Offset.X - (sphere.Radius - newRadius), pcc.Offset.Y - (sphere.Radius - newRadius));
            }

            sphere.Radius = newRadius;
        }
    }
}
