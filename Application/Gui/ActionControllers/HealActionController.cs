using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Utils;

namespace Orbit.Gui.ActionControllers
{
    public class HealActionController : ActionController
    {
        private IHealingKit healingKit;

        public HealActionController(SceneMgr mgr, IHealingKit healingKit) : base(mgr)
        {
            this.healingKit = healingKit;
        }

        public override void ActionClicked(BuyActionUC window)
        {
            healingKit.Heal();
            window.Remove();
        }

        public override void CreateHeaderText(BuyActionUC window)
        {
            window.SetHeaderText("Repair base");
        }

        public override void CreatePriceText(BuyActionUC window)
        {
            window.SetPriceText("Costs " + healingKit.Cost + " credits");
        }

        public override void CreateImageUriString(BuyActionUC window)
        {
            window.SetImageUri("pack://application:,,,/resources/images/icons/heal-icon.png");
        }
    }
}
