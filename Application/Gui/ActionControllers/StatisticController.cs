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
        private const float SHOW_TIME = 1;
        private bool limited;
        protected float time = SHOW_TIME;
        protected bool waitingToEnd = false;

        protected SceneMgr mgr;
        protected EndGameStats statsWindow;

        public StatisticController(SceneMgr mgr, EndGameStats statsWindow, bool limited)
        {
            this.mgr = mgr;
            this.statsWindow = statsWindow;
            this.limited = limited;
        }

        public virtual void Update(float tpf)
        {
            if (time <= 0 && HasNext() && !waitingToEnd)
            {
                Next();
                time = SHOW_TIME;            }
            else if (time <= 0 && !waitingToEnd && limited)
            {
                time = 30;
                waitingToEnd = true;
            }
            else if (time <= 0 && waitingToEnd)
                statsWindow.HideStats();

            time -= tpf;

            if (waitingToEnd)
                UpdateTime();
        }

        private void UpdateTime()
        {
            mgr.BeginInvoke(new Action(() => statsWindow.SetTime(time.ToString("00"))));
        }

        protected double ComputePercents() {
            double val = time / SHOW_TIME;
            if (val > 1)
                val = 1;
            if (val < 0)
                val = 0;

            return val;
        }

        protected abstract void Next();

        protected abstract bool HasNext();
    }
}
