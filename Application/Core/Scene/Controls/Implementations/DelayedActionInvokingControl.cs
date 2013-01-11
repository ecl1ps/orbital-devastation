using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class DelayedActionInvokingControl : Control
    {
        private float timer;
        private float elapsedTime = 0;
        private bool remove;
        private Action action;

        public DelayedActionInvokingControl(float delay, bool removeAfterInvoke, Action action)
        {
            this.timer = delay;
            this.remove = removeAfterInvoke;
            this.action = action;
        }

        protected override void UpdateControl(float tpf)
        {
            elapsedTime += tpf;
            if (elapsedTime != 0.0f && elapsedTime >= timer)
            {
                action.Invoke();
                if (remove)
                    me.RemoveControl(this);
                elapsedTime = 0.0f;
            }
        }
    }
}
