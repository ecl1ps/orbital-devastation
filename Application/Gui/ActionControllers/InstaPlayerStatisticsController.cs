﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    class InstaPlayerStatisticsController : PlayerStatisticsController
    {
        public InstaPlayerStatisticsController(SceneMgr mgr, EndGameStats stats, bool limited, PlayerStatsUC window)
            : base(mgr, stats, limited, window)
        {
            LoadHook(0);
            LoadMine(0);
            LoadBullet(0);
            LoadDamage();
            LoadGold();
            LoadHeal();
            LoadActions();
            LoadPowerups();
            LoadFavAction();
            LoadFavPowerup();

            SetData();
        }

        protected override double ComputePercents()
        {
            return 0;
        }
    }
}
