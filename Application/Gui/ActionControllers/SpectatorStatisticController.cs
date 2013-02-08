using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;
using Orbit.Core;
using Orbit.Core.Players;

namespace Orbit.Gui.ActionControllers
{
    class SpectatorStatisticController : StatisticController
    {
        private int step = 0;
        private SpectatorStatsUC window;

        private String gameTime = string.Empty;
        private String deadTime = string.Empty;
        private String actionsUsed = string.Empty;
        private String favAction = string.Empty;
        private String damage = string.Empty;

        public SpectatorStatisticController(SceneMgr mgr, Player owner, EndGameStats stats, bool limited, SpectatorStatsUC window) 
            : base(mgr, owner, stats, limited)
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
            if (owner.Statistics.Actions.Count == 0)
            {
                favAction = "No data";
                time = 0;
                return;
            }

            ISpecialAction first = owner.Statistics.Actions.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
            favAction = first.Name;
            time = 0;
        }

        protected void LoadActionsUsed()
        {
            if (owner.Statistics.Actions.Count == 0)
            {
                time = 0;
                actionsUsed = 0.ToString(Strings.Culture);
                return;
            }

            actionsUsed = FastMath.LinearInterpolate(owner.Statistics.Actions.Count, 0, ComputePercents()).ToString("###", Strings.Culture);
        }

        protected void LoadDamage()
        {
            if (owner.Statistics.DamageTaken == 0)
            {
                time = 0;
                damage = 0.ToString(Strings.Culture);
                return;
            }
            damage = FastMath.LinearInterpolate(owner.Statistics.DamageTaken, 0, ComputePercents()).ToString(".#", Strings.Culture);
        }

        protected void LoadDeadTime()
        {
            if (owner.Statistics.DeadTime == 0)
            {
                time = 0;
                deadTime = 0.ToString(Strings.Culture);
                return;
            }
            deadTime = FastMath.LinearInterpolate(owner.Statistics.DeadTime, 0, ComputePercents()).ToString(".#", Strings.Culture);
        }

        protected void LoadGameTime()
        {
            if (owner.Statistics.Time == 0)
            {
                time = 0;
                damage = 0.ToString(Strings.Culture);
                return;
            }

            gameTime = FastMath.LinearInterpolate(owner.Statistics.Time, 0, ComputePercents()).ToString(".#", Strings.Culture);
        }
    }
}
