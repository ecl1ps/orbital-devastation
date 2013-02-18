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
using System.Windows.Media.Effects;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ScoreboardUC.xaml
    /// </summary>
    public partial class ScoreboardUC : UserControl
    {
        public ScoreboardUC(LobbyPlayerData winnerData, List<LobbyPlayerData> data)
        {
            InitializeComponent();

            lblWinner.Content = winnerData.Name + " is winner!";

            IEnumerable<LobbyPlayerData> sortedData = data.OrderByDescending(p => p.Score).ThenByDescending(p => p.Won);

            LobbyPlayer element;
            int i = 0;
            foreach (LobbyPlayerData d in sortedData)
            {
                element = new LobbyPlayer(d, true);
                spResults.Children.Add(element);
                PrepareWinnerColors(element, i);
                i++;
            }
        }

        private void PrepareWinnerColors(LobbyPlayer elem, int order)
        {
            Color c;
            if (order == 0)
                c = Colors.Gold;
            else if (order == 1)
                c = Colors.Silver;
            else if (order == 2)
                c = Color.FromRgb(140, 120, 83);
            else
                c = Colors.LightGray;

            elem.border.Background = new SolidColorBrush(c);
            if (order < 3)
            {
                DropShadowEffect effect = new DropShadowEffect();
                effect.Color = c;
                effect.BlurRadius = 10;
                effect.Direction = 0;
                
            //    elem.border.Effect = effect;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowStartScreen();
            App.Instance.GameEnded();
        }
    }
}
