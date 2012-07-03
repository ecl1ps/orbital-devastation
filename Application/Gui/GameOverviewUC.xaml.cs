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
    /// Interaction logic for GameOverviewUC.xaml
    /// </summary>
    public partial class GameOverviewUC : UserControl
    {
        public GameOverviewUC(List<PlayerOverviewData> data)
        {
            InitializeComponent();

            IEnumerable<PlayerOverviewData> sortedData = data.OrderByDescending(p => p.Active).ThenByDescending(p => p.Score).ThenByDescending(p => p.Gold);

            foreach (PlayerOverviewData d in sortedData)
                spPlayers.Children.Add(new PlayerOverview(d));
        }
    }
}
