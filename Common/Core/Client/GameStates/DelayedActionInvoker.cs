using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client.GameStates
{
    public class DelayedActionInvoker : IGameState
    {
        private float timer;
        private float elapsedTime = 0;
        private Action action;

        public DelayedActionInvoker(float delay, Action action)
        {
            this.timer = delay;
            this.action = action;
        }

        public void Update(float tpf)
        {
            elapsedTime += tpf;
            if (elapsedTime != 0.0f && elapsedTime >= timer)
            {
                action.Invoke();
                elapsedTime = 0.0f;
            }
        }
    }
}
