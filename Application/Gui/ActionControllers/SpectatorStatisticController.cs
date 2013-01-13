using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;
using Orbit.Core;

namespace Orbit.Gui.ActionControllers
{
    class SpectatorStatisticController : StatisticController
    {
        private int step = 0;
        private SpectatorStatsUC window;

        private String gameTime = "";
        private String deadTime = "";
        private String actionsUsed = "";
        private String favAction = "";
        private String damage = "";

        public SpectatorStatisticController(SceneMgr mgr, EndGameStats stats, bool limited, SpectatorStatsUC window) : base(mgr, stats, limited)
        {
            this.window = window;
        }

        protected override void Next()
        {
            step++;
        }

        protected override bool HasNext()
        {
            return step <= 4;
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            switch (step)
            {
                case 0:
                    LoadGameTime();
                    break;
                case 1:
                    LoadDeadTime();
                    break;
                case 2:
                    LoadDamage();
                    break;
                case 3:
                    LoadActionsUsed();
                    break;
                case 4:
                    LoadFavAction();
                    break;
            }

            SetData();
        }

        protected void SetData()
        {
            mgr.BeginInvoke(new Action(() => window.SetData(gameTime, deadTime, actionsUsed, favAction, damage)));
        }

        protected void LoadFavAction()
        {
            if (mgr.LastGameStats.Actions.Count == 0)
            {
                favAction = "No data";
                time = 0;
                return;
            }

            ISpecialAction first = mgr.LastGameStats.Actions.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
            favAction = first.Name;
            time = 0;
        }

        protected void LoadActionsUsed()
        {
            if (mgr.LastGameStats.Actions.Count == 0)
            {
                time = 0;
                actionsUsed = "0";
                return;
            }

            actionsUsed = FastMath.LinearInterpolate(mgr.LastGameStats.Actions.Count, 0, ComputePercents()).ToString("###");
        }

        protected void LoadDamage()
        {
            if (mgr.LastGameStats.DamageTaken == 0)
            {
                time = 0;
                damage = "0";
                return;
            }
            damage = FastMath.LinearInterpolate(mgr.LastGameStats.DamageTaken, 0, ComputePercents()).ToString(".#");
        }

        protected void LoadDeadTime()
        {
            if (mgr.LastGameStats.DeadTime == 0)
            {
                time = 0;
                deadTime = "0";
                return;
            }
            deadTime = FastMath.LinearInterpolate(mgr.LastGameStats.DeadTime, 0, ComputePercents()).ToString(".#");
        }

        protected void LoadGameTime()
        {
            if (mgr.LastGameStats.Time == 0)
            {
                time = 0;
                damage = "0";
                return;
            }

            gameTime = FastMath.LinearInterpolate(mgr.LastGameStats.Time, 0, ComputePercents()).ToString(".#");
        }
    }
}
