using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class TemporaryControlDisableControl : Control
    {
        private List<IControl> controlsForDisabling;
        public float Time { get; set; }

        protected override void InitControl(Entities.ISceneObject me)
        {
            if (controlsForDisabling != null)
                controlsForDisabling.ForEach(control => control.Enabled = false);

            events.AddEvent(1, new Event(Time, EventType.ONE_TIME, new Action(() => { ReenableControls(); })));
        }

        private void ReenableControls()
        {
            controlsForDisabling.ForEach(control => control.Enabled = true);
            Destroy();
        }

        public void SetControlsForDisable(List<IControl> controls)
        {
            controlsForDisabling = controls;
        }
    }
}
