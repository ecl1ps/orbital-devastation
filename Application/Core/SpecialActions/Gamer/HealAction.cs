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

        public HealAction(IHealingKit healingKit, SceneMgr mgr, Player plr) : base(mgr, plr)
        {
            this.healingKit = healingKit;
            Name = "Repair base";
            Cost = healingKit.Cost;
            ImageSource = "pack://application:,,,/resources/images/icons/heal-icon.png";

            Cooldown = 1; //sekunda
        }

        public override void StartAction()
        {
            if (!IsReady())
                return;

            base.StartAction();
            healingKit.Heal();
            Cost = healingKit.Cost;
        }

        public override bool IsReady()
        {
            int heal = (int)(Owner.Data.MaxBaseIntegrity * SharedDef.HEAL_AMOUNT) + Owner.Data.BonusHeal;

            return !IsOnCooldown() &&
                healingKit.Cost <= Owner.Data.Gold && 
                (Owner.Data.MaxBaseIntegrity - Owner.GetBaseIntegrity()) >= heal;
        }
    }
}
