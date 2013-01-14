using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Players;

namespace Orbit.Gui.ActionControllers
{
    class InstaSpectatorStatisticsController : SpectatorStatisticController
    {
        public InstaSpectatorStatisticsController(SceneMgr mgr, Player owner, EndGameStats stats, bool limited, SpectatorStatsUC window)
            : base(mgr, owner, stats, limited, window)
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
