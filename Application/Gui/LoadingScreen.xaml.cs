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
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : UserControl
    {
        private Dictionary<Player, PlayerLoadingUC> map;

        public LoadingScreen()
        {
            InitializeComponent();
            map = new Dictionary<Player,PlayerLoadingUC>();
        }

        public void PreparePlayers(List<Player> players)
        {
            if (players == null)
                return;

             Dispatcher.BeginInvoke(new Action(() =>
             {
                foreach (Player p in players) 
                {
                    if (map.ContainsKey(p))
                        continue;
               
                     PlayerLoadingUC uc = new PlayerLoadingUC();
                     uc.LoadPlayer(p);
                     uc.Margin = new Thickness(5, 0, 5, 0);

                     PlayerPanel.Children.Add(uc);
                     map.Add(p, uc);
                }
            }));
        }

        public void UpdatePlayer(Player p, float progress)
        {
            if(map.ContainsKey(p))
                map[p].SetPercentage(progress);
        }
    }
}
