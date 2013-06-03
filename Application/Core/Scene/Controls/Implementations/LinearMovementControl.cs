using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LinearMovementControl : Control, IMovementControl
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IMovable meMovable;

        public float Speed { get; set; }
        public float RealSpeed { get { return Speed; } }
        public Vector RealDirection { get { return meMovable.Direction; } }

        protected override void InitControl(ISceneObject obj)
        {
            if (!(obj is IMovable))
            {
                Logger.Error("LinearMovementControl cannot be attached to non-movable object!");
                return;
            }

            meMovable = obj as IMovable;
        }

        protected override void UpdateControl(float tpf)
        {
            if (meMovable == null)
                return;

            me.Position += (meMovable.Direction * Speed * tpf);
        }
    }
}
