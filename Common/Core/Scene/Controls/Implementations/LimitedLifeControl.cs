using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LimitedLifeControl : Control
    {
        public LimitedLifeControl(float timeToDie)
        {
            events.AddEvent(1, new Event(timeToDie, EventType.ONE_TIME, new Action(() => { me.DoRemoveMe(); })));
        }
    }
}
