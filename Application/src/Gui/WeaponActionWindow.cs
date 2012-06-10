﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Scene;

namespace Orbit.src.Gui
{
    public class WeaponActionWindow : GoldActionWindow
    {

        private IWeapon weapon;
        private Player player;

        public WeaponActionWindow(IWeapon weapon, Player player)
        {
            this.weapon = weapon;
            this.player = player;
        }

        public override BitmapImage initImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/resources/images/icons/upgrade.png");
            image.EndInit();
            return image;
        }

        public override void initText()
        {
            Header.Text = weapon.Name;
            Text.Text = "Costs " + weapon.Cost + " credits";
        }

        public override void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (player.Data.Gold >= weapon.Cost)
            {
                player.Data.Gold -= weapon.Cost;
                addWeapon();
                SceneMgr.GetInstance().ActionBar.removeItem(this);
            }
        }

        private void addWeapon()
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    player.Data.Hook = weapon;
                    break;
            }
        }

        public override object getId()
        {
            return weapon.WeaponType;
        }
    }
}
