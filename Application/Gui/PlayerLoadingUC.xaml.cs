using Orbit.Core.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for PlayerLoadingUC.xaml
    /// </summary>
    public partial class PlayerLoadingUC : UserControl
    {
        private Border[] arr = new Border[20];

        public PlayerLoadingUC()
        {
            InitializeComponent();
            arr[0] = b0;
            arr[1] = b1;
            arr[2] = b2;
            arr[3] = b3;
            arr[4] = b4;
            arr[5] = b5;
            arr[6] = b6;
            arr[7] = b7;
            arr[8] = b8;
            arr[9] = b9;
            arr[10] = b10;
            arr[11] = b11;
            arr[12] = b12;
            arr[13] = b13;
            arr[14] = b14;
            arr[15] = b15;
            arr[16] = b16;
            arr[17] = b17;
            arr[18] = b18;
            arr[19] = b19;
        }

        public void SetPercentage(float perc)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int top = (int)(perc * 100 / 5) - 1;
                for (int i = 0; i < top; i++)
                    arr[i].Background = new SolidColorBrush(Color.FromRgb(151, 255, 0));
            }));
        }

        public void LoadPlayer(Player p)
        {
            Player.Text = p.Data.Name;
        }
    }
}
