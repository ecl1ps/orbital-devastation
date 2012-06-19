﻿using System;
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

        public override void ActionClicked(BuyActionWindow window)
        {
            if (player.Data.Gold >= weapon.Cost)
            {
                player.AddGoldAndShow(-weapon.Cost);
                AddWeapon();
                window.Remove();
            }
        }

        private void AddWeapon()
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    player.Hook = weapon;
                    break;
            }
        }

        public override void CreateHeaderText(BuyActionWindow window)
        {
            window.SetHeaderText(weapon.Name);
        }

        public override void CreatePriceText(BuyActionWindow window)
        {
            window.SetPriceText("Costs " + weapon.Cost + " credits");
        }

        public override void CreateImageUriString(BuyActionWindow window)
        {
            window.SetImageUri("pack://application:,,,/resources/images/icons/upgrade.png");
        }
    }
}