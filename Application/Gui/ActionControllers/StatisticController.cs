using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    public abstract class StatisticController : IGameState
    {
        protected float time = 2;
        protected bool waitingToEnd = false;

        protected SceneMgr mgr;
        protected PlayerStatsUC window;

        public StatisticController(SceneMgr mgr, PlayerStatsUC window)
        {
            this.mgr = mgr;
            this.window = window;
        }

        public virtual void Update(float tpf)
        {
            if (time <= 0 && HasNext() && !waitingToEnd)
            {
                Next();
                time = 2;
            }
            else if (time <= 0 && !waitingToEnd)
                time = 60;
            else if (time <= 0 && waitingToEnd)
                mgr.CloseGameWindowAndCleanup();

            time -= tpf;
        }

        protected abstract void Next();

        protected abstract bool HasNext();
    }
}
