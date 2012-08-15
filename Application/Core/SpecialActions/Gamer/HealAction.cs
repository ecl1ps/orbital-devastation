using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Utils;
using Orbit.Core.Client;
using Orbit.Core.Players;

namespace Orbit.Core.SpecialActions.Gamer
{
    class HealAction : SpecialAction
    {
        private IHealingKit healingKit;

        public HealAction(IHealingKit healingKit, SceneMgr mgr, Player plr)
            : base(mgr, plr)
        {
            this.healingKit = healingKit;
            Name = "Repair base";
            Cost = healingKit.Cost;
            ImageSource = "pack://application:,,,/resources/images/icons/heal-icon.png";
        }

        public override void StartAction()
        {
            healingKit.Heal();
            Cost = healingKit.Cost;
        }

        public override bool IsReady()
        {
            return healingKit.Cost >= Owner.Data.Gold && (SharedDef.BASE_MAX_INGERITY - Owner.GetBaseIntegrity()) >= SharedDef.HEAL_AMOUNT;
        }
    }
}
