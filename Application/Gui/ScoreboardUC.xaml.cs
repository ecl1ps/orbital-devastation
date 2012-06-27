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
    /// Interaction logic for ScoreboardUC.xaml
    /// </summary>
    public partial class ScoreboardUC : UserControl
    {
        private LobbyPlayerData winnerData;
        private List<LobbyPlayerData> data;

        public ScoreboardUC(LobbyPlayerData winnerData, List<LobbyPlayerData> data)
        {
            InitializeComponent();
            this.winnerData = winnerData;
            this.data = data;

            lblWinner.Content = winnerData.Name + " is winner!";

            IEnumerable<LobbyPlayerData> sortedData = data.OrderByDescending(p => p.Won).ThenByDescending(p => p.Score);

            foreach (LobbyPlayerData d in sortedData)
                spResults.Children.Add(new LobbyPlayer(d));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowStartScreen();
            (Application.Current as App).GameEnded();
        }
    }
}
