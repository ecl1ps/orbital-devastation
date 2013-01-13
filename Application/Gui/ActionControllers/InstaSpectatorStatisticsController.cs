using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    class InstaSpectatorStatisticsController : SpectatorStatisticController
    {
        public InstaSpectatorStatisticsController(SceneMgr mgr, EndGameStats stats, bool limited, SpectatorStatsUC window)
            : base(mgr, stats, limited, window)
        {
            LoadActionsUsed();
            LoadDamage();
            LoadDeadTime();
            LoadFavAction();
            LoadGameTime();

            SetData();
        }

        protected override double ComputePercents()
        {
            return 0;
        }

    }
}
