using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    public class WeaponActionController : ActionController
    {
        private IWeapon weapon;
        private Player player;

        public WeaponActionController(SceneMgr mgr, IWeapon weapon, Player player) : base(mgr)
        {
            this.weapon = weapon;
            this.player = player;
        }

        public override void ActionClicked(BuyActionUC window)
        {
            if (weapon == null)
                return;

            if (player.Data.Gold >= weapon.Cost)
            {
                player.AddGoldAndShow(-weapon.Cost);
                AddWeapon();
                window.AttachNewController(new WeaponActionController(sceneMgr, weapon.Next(), player));
            }
        }

        private void AddWeapon()
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    player.Hook = weapon;
                    break;
                case WeaponType.MINE:
                    player.Mine = weapon;
                    break;
                case WeaponType.CANNON:
                    player.Canoon = weapon;
                    break;
            }
        }

        public override void CreateHeaderText(BuyActionUC window)
        {
            if (weapon != null)
                window.SetHeaderText(weapon.Name);
            else
                window.SetHeaderText("No upgrades");
        }

        public override void CreatePriceText(BuyActionUC window)
        {
            if (weapon != null)
                window.SetPriceText("Costs " + weapon.Cost + " credits");
            else
            {
                window.SetPriceText("No cost");
                window.Deactivate();
            }
        }

        public override void CreateImageUriString(BuyActionUC window)
        {
            window.SetImageUri("pack://application:,,,/resources/images/icons/upgrade.png");
        }
    }
}
