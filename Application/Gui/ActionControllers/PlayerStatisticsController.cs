using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.SpecialActions;
using Orbit.Core.Players;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    class PlayerStatisticsController : StatisticController
    {
        private int step = 0;
        private PlayerStatsUC window;

        private float loadTime = 0;
        private int multipleStatsStep = 0;

        private String mine = "";
        private String bullet = "";
        private String hook = "";
        private String damage = "";
        private String heal = "";
        private String gold = "";
        private String actions = "";
        private String powerups = "";
        private String favAction = "";
        private String favPowerup = "";

        public PlayerStatisticsController(SceneMgr mgr, EndGameStats stats, bool limited, PlayerStatsUC window)
            : base(mgr, stats, limited)
        {
            this.window = window;
        }

        protected override bool HasNext()
        {
            return step < 10;
        }

        protected override void Next()
        {
            step++;
            multipleStatsStep = 0;
            loadTime = 0;
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            switch (step)
            {
                case 0:
                    LoadMine(tpf);
                    break;
                case 1:
                    LoadBullet(tpf);
                    break;
                case 2:
                    LoadHook(tpf);
                    break;
                case 3:
                    LoadDamage();
                    break;
                case 4:
                    LoadHeal();
                    break;
                case 5:
                    LoadGold();
                    break;
                case 6:
                    LoadActions();
                    break;
                case 7:
                    LoadPowerups();
                    break;
                case 8:
                    LoadFavAction();
                    break;
                case 9:
                    LoadFavPowerup();
                    break;
            }

            SetData();
        }

        protected void SetData()
        {
            mgr.BeginInvoke(new Action(() =>
            {
                window.setData(mine, bullet, hook, damage, heal, gold, actions, powerups, favAction, favPowerup);
            }));
        }

        protected void LoadFavPowerup()
        {
            if (mgr.LastGameStats.Stats.Count == 0)
            {
                favPowerup = "No data";
                time = 0;
                return;
            }

            Stat first = mgr.LastGameStats.Stats.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
            favPowerup = first.text;
            time = 0;
        }

        protected void LoadFavAction()
        {
            if(mgr.LastGameStats.Actions.Count == 0) 
            {
                favAction = "No data";
                time = 0;
                return;
            }

            ISpecialAction first = mgr.LastGameStats.Actions.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
            favAction = first.Name;
            time = 0;
        }

        protected void LoadPowerups()
        {
            if (mgr.LastGameStats.Stats.Count == 0)
            {
                time = 0;
                powerups = "0";
                return;
            }
            powerups = FastMath.LinearInterpolate(mgr.LastGameStats.Stats.Count, 0, ComputePercents()).ToString("###");
        }

        protected void LoadActions()
        {
            if (mgr.LastGameStats.Actions.Count == 0)
            {
                time = 0;
                actions = "0";
                return;
            }

            actions = FastMath.LinearInterpolate(mgr.LastGameStats.Actions.Count, 0, ComputePercents()).ToString("###");
        }

        protected void LoadGold()
        {
            if (mgr.LastGameStats.GoldEarned == 0)
            {
                time = 0;
                gold = "0";
                return;
            }
            gold = FastMath.LinearInterpolate(mgr.LastGameStats.GoldEarned, 0, ComputePercents()).ToString(".#");
        }

        protected void LoadHeal()
        {
            if (mgr.LastGameStats.Healed == 0)
            {
                time = 0;
                heal = "0";
                return;
            }
            heal = FastMath.LinearInterpolate(mgr.LastGameStats.Healed, 0, ComputePercents()).ToString(".#");
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

        protected void LoadHook(float tpf)
        {
                if (tpf == 0)
                {
                    hook += mgr.LastGameStats.HookFired;
                    hook += " / " + mgr.LastGameStats.HookHit;
                    double val = mgr.LastGameStats.HookHit / mgr.LastGameStats.HookFired;
                    if (Double.IsNaN(val))
                        val = 0;
                    hook += " / " + val.ToString("#0.##%");
                }
               else if (loadTime <= 0)
                {
                    loadTime = 2 / 3;
                    multipleStatsStep++;
                    if (multipleStatsStep == 1)
                        hook += mgr.LastGameStats.HookFired;
                    else if (multipleStatsStep == 2)
                        hook += " / " + mgr.LastGameStats.HookHit;
                    else if (multipleStatsStep == 3) {
                        double val = mgr.LastGameStats.HookHit / mgr.LastGameStats.HookFired;
                        if (Double.IsNaN(val))
                            val = 0;
                        hook += " / " + val.ToString("#0.##%");
                    }
                }

            loadTime -= tpf;
            
        }

        protected void LoadBullet(float tpf)
        {
                if (tpf == 0)
                {
                    bullet += mgr.LastGameStats.BulletFired;
                    bullet += " / " + mgr.LastGameStats.BulletHit;
                    double val = mgr.LastGameStats.BulletHit / mgr.LastGameStats.BulletFired;
                    if (Double.IsNaN(val))
                        val = 0;
                    bullet += " / " + val.ToString("#0.##%");
                }
               else if (loadTime <= 0)
                {
                    loadTime = 2 / 3;
                    multipleStatsStep++;
                    if (multipleStatsStep == 1)
                        bullet += mgr.LastGameStats.BulletFired;
                    else if (multipleStatsStep == 2)
                        bullet += " / " + mgr.LastGameStats.BulletHit;
                    else if (multipleStatsStep == 3) {
                        double val = mgr.LastGameStats.BulletHit / mgr.LastGameStats.BulletFired;
                        if (Double.IsNaN(val))
                            val = 0;
                        bullet += " / " + val.ToString("#0.##%");
                    }
                }

            loadTime -= tpf;
        }

        protected void LoadMine(float tpf)
        {
            if (tpf == 0)
            {
                mine += mgr.LastGameStats.MineFired;
                mine += " / " + mgr.LastGameStats.MineHit;
                double val = mgr.LastGameStats.MineHit / mgr.LastGameStats.MineFired;
                if (Double.IsNaN(val))
                    val = 0;
                mine += " / " + val.ToString("#0.##%");
            }
            else if (loadTime <= 0)
            {
                loadTime = 2 / 3;
                multipleStatsStep++;
                if (multipleStatsStep == 1)
                    mine += mgr.LastGameStats.MineFired;
                else if (multipleStatsStep == 2)
                    mine += " / " + mgr.LastGameStats.MineHit;
                else if (multipleStatsStep == 3) {
                    double val = mgr.LastGameStats.MineHit / mgr.LastGameStats.MineFired;
                    if (Double.IsNaN(val))
                        val = 0;
                    mine += " / " + val.ToString("#0.##%");
                }
            }

            loadTime -= tpf;
        }
    }
}
