using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class TemporaryControlRemovalControl : Control
    {
        public List<IControl> ToRemove { get; set; }
        public float Time { get; set; }

        protected override void InitControl(Entities.ISceneObject me)
        {
            ToRemove.ForEach(control => me.RemoveControl(control));

            events.AddEvent(1, new Event(Time, EventType.ONE_TIME, new Action(() => { ReenableControls(); })));
        }

        private void ReenableControls()
        {
            ToRemove.ForEach(control => me.AddControl(control));
            Destroy();
        }
    }
}
