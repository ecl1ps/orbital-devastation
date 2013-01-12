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
using Orbit.Core.Client;
using Orbit.Core;
using Orbit.Core.Players;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for EndGameStats.xaml
    /// </summary>
    public partial class EndGameStats : UserControl
    {
        private SceneMgr mgr;

        public EndGameStats(SceneMgr mgr)
        {
            this.mgr = mgr;
            InitializeComponent();
        }

        public void setStats(UIElement elem)
        {
            Root.Children.Add(elem);
            Canvas.SetLeft(elem, 91);
            Canvas.SetTop(elem, 60);
        }

        public void SetTime(String time)
        {
            Time.Text = time;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HideStats();
        }

        public void HideStats()
        {
            GameEnd endType = mgr.GetLastGameEnd();
            if (endType != GameEnd.TOURNAMENT_FINISHED)
                mgr.CloseGameWindowAndCleanup();
            else
                mgr.TournamentFinished(mgr.GetWinner());
        }
    }
}
