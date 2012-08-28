using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Orbit.Core.Players;

namespace Orbit.Core.SpecialActions.Gamer
{
    class WeaponUpgrade : SpecialAction
    {
        private IWeapon currentWeapon;

        public WeaponUpgrade(IWeapon weapon, SceneMgr mgr, Player plr) : base(mgr, plr)
        {
            LoadWeapon(weapon);
            ImageSource = "pack://application:,,,/resources/images/icons/upgrade.png";
        }

        private void LoadWeapon(IWeapon weapon)
        {
            this.currentWeapon = weapon;

            if (weapon == null || weapon.Next() == null)
            {
                Name = "Unavailable";
                Cost = 0;
            }
            else
            {
                Name = weapon.Next().Name;
                Cost = weapon.Next().Cost;
            }
        }

        public override void StartAction()
        {
            if (IsReady())
            {
                base.StartAction();
                UpgradeWeapon();
            }

        }

        private void UpgradeWeapon()
        {
            switch (currentWeapon.DeviceType)
            {
                case DeviceType.CANNON:
                    Owner.Canoon = currentWeapon.Next();
                    break;
                case DeviceType.HOOK:
                    Owner.Hook = currentWeapon.Next();
                    break;
                case DeviceType.MINE:
                    Owner.Mine = currentWeapon.Next();
                    break;

                default:
                    throw new Exception("unknown weapon type " + currentWeapon.DeviceType.ToString());
            }

            LoadWeapon(currentWeapon.Next());
        }

        public override bool IsReady()
        {
            return currentWeapon.Next() != null && currentWeapon.Next().Cost <= Owner.Data.Gold;
        }
    }
}
