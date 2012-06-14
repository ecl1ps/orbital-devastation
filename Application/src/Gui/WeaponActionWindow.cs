using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Scene;
using System.Windows;

namespace Orbit.Gui
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

        public override BitmapImage InitImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/resources/images/icons/upgrade.png");
            image.EndInit();
            return image;
        }

        public override void InitText()
        {
            Header.Text = weapon.Name;
            Text.Text = "Costs " + weapon.Cost + " credits";
        }

        public override void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (player.Data.Gold >= weapon.Cost)
            {
                player.Data.Gold -= weapon.Cost;
                AddWeapon();
                (Application.Current as App).GetSceneMgr().ActionBar.RemoveItem(this);
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

        public override object GetId()
        {
            return weapon.WeaponType;
        }
    }
}
