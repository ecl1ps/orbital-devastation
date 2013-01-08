using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class TemporaryControlDisableControl : Control
    {
        public List<IControl> ControlsForDisabling { get; set; }
        public float Time { get; set; }

        protected override void InitControl(Entities.ISceneObject me)
        {
            ControlsForDisabling.ForEach(control => control.Enabled = false);

            events.AddEvent(1, new Event(Time, EventType.ONE_TIME, new Action(() => { ReenableControls(); })));
        }

        private void ReenableControls()
        {
            ControlsForDisabling.ForEach(control => control.Enabled = true);
            Destroy();
        }
    }
}
