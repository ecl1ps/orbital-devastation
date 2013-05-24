using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Client;
using Orbit.Core.Players;

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
        protected Player owner;

        public StatisticController(SceneMgr mgr, Player owner, EndGameStats statsWindow, bool limited)
        {
            this.mgr = mgr;
            this.owner = owner;
            this.statsWindow = statsWindow;
            this.limited = limited;
        }

        public virtual void Update(float tpf)
        {
            if (SharedDef.SKIP_STATISTICS)
            {
                statsWindow.HideStats();
                return;
            }

            if (time <= 0 && HasNext() && !waitingToEnd)
            {
                Next();
                time = SHOW_TIME;            
            }
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
            mgr.BeginInvoke(new Action(() => statsWindow.SetTime(time.ToString("00", Strings.Culture))));
        }

        protected virtual double ComputePercents() {
            double val = time / SHOW_TIME;
            if (val > 1)
                val = 1;
            if (val < 0)
                val = 0;

            return val;
        }

        protected String FormatTime(float value)
        {
            int min = (int)(value / 60);
            int sec = (int)value % 60;

            return min + ":" + sec;
        }

        protected abstract void Next();

        protected abstract bool HasNext();
    }
}
