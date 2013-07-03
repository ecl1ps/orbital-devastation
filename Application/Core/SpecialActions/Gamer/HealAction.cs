using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Utils;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Microsoft.Xna.Framework;

namespace Orbit.Core.SpecialActions.Gamer
{
    class HealAction : SpecialAction, IPlayerAction
    {
        private IHealingKit healingKit;

        public HealAction()
            : base(null, null)
        {
            Name = "Repair base";
            ImageSource = "pack://application:,,,/resources/images/icons/heal-icon.png";
            BackgroundColor = new Color(0.85f, 1, 0.8f);
            Cooldown = 1; //sekunda
        }

        public HealAction(IHealingKit healingKit, SceneMgr mgr, Player plr) : base(mgr, plr)
        {
            this.healingKit = healingKit;
            Name = "Repair base";
            Cost = healingKit.Cost;
            ImageSource = "pack://application:,,,/resources/images/icons/heal-icon.png";
            BackgroundColor = new Color(0.85f, 1, 0.8f);
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
