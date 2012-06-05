using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Orbit.src.Core.utils;
using Orbit.Core.Scene;

namespace Orbit.src.Gui
{
    public class HealActionWindow : GoldActionWindow
    {
        private IHealingKit healingKit;

        public HealActionWindow(IHealingKit healingKit)
        {
            this.healingKit = healingKit;
        }

        public override BitmapImage initImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("heal-icon.png", UriKind.Relative);
            image.EndInit();
            return image;
        }

        public override void initText()
        {
            Header.Text = "Repair base";
            Text.Text = "Repair your base at cost of " + healingKit.Cost + " credits";
        }

        public override void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            healingKit.heal();
            SceneMgr.GetInstance().ActionBar.removeItem(this);
        }
    }
}
