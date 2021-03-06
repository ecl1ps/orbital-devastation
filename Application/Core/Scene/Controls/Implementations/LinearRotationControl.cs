using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LinearRotationControl : Control
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRotable meRotable;

        public float RotationSpeed { get; set; }

        protected override void InitControl(ISceneObject obj)
        {
            if (!(obj is IRotable))
            {
                Logger.Error("LinearRotationControl cannot be attached to non-rotable object!");
                return;
            }

            meRotable = obj as IRotable;
        }

        protected override void UpdateControl(float tpf)
        {
            if (meRotable == null)
                return;

            meRotable.Rotation += RotationSpeed * tpf * 100;
            /*SceneMgr.GetInstance().ShowStatusText(2, "ROT: " + meRotable.Rotation);
            SceneMgr.GetInstance().ShowStatusText(3, "ROT SP: " + RotationSpeed);
            SceneMgr.GetInstance().ShowStatusText(4, "ROT INC: " + (RotationSpeed * tpf * 100));*/
        }
    }
}
