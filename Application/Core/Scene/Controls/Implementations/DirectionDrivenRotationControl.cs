using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class DirectionDrivenRotationControl : Control
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is IRotable) || !(me is IMovable))
                throw new ArgumentException("DirectionDrivenRotationControl must by attached to an IRotable and an IMovable object");
        }

        protected override void UpdateControl(float tpf)
        {
            (me as IRotable).Rotation = (float)FastMath.DegToRad(Vector.AngleBetween(SharedDef.DEFAULT_VECTOR, (me as IMovable).Direction));

            //Logger.Debug("Rotation: " + (me as IRotable).Rotation + " vec(" + (me as IMovable).Direction.ToString() + ")");
        }
    }
}
