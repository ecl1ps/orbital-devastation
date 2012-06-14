using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Orbit.Core.Utils;
using Orbit.Core.Scene;
using Lidgren.Network;
using System.Windows;

namespace Orbit.Gui
{
    public class HealActionWindow : GoldActionWindow
    {
        private IHealingKit healingKit;

        public HealActionWindow(IHealingKit healingKit)
        {
            this.healingKit = healingKit;
            Init();
        }

        public override BitmapImage InitImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/resources/images/icons/heal-icon.png");
            image.EndInit();
            return image;
        }

        public override void InitText()
        {
            Header.Text = "Repair base";
            Text.Text = "Costs " + healingKit.Cost + " credits";
        }

        public override void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            healingKit.Heal();
            (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
            {
                (Application.Current as App).GetSceneMgr().ActionBar.RemoveItem(this);
            }));
        }

        public override object GetId()
        {
            //allways same
            return 1;
        }
    }
}
