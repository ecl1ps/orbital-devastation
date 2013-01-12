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
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for OptionsUC.xaml
    /// </summary>
    public partial class MainUC : UserControl
    {
        public MainUC()
        {
            InitializeComponent();
#if !DEBUG
            spMenu.Children.Remove(btnConnectToLocalhost);
            spMenu.Children.Remove(btnPlayAsSpectator);
#endif
        }

        private void btnSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowBotSelectionGui();
        }

        private void btnQuickGame_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.CreateGameGui();
            App.Instance.StartHostedGame();
        }

        private void btnHostTournament_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.StartTournamentLobby();
        }

        private void btnFindHostedGame_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.LookForGame();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRepeatGame_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.RepeatGame();
        }

        private void btnConnectToLocalhost_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.CreateGameGui();
            App.Instance.ConnectToGame("127.0.0.1");
        }

        private void btnShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowStatisticsGui();
        }

        private void btnShowOptions_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new OptionsMenu());
        }

        private void btnPlayAsSpectator_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.StartTournamentLobby();

            App.Instance.GetSceneMgr().Enqueue(new Action(() =>
            {
                App.Instance.GetSceneMgr().GetCurrentPlayer().Data.LobbyReady = true;
                App.Instance.GetSceneMgr().SendPlayerReadyMessage();
                TournamentSettings s = new TournamentSettings(true);
                s.MMType = Orbit.Core.Server.Match.MatchManagerType.TEST_LEADER_SPECTATOR;
                App.Instance.GetSceneMgr().ProcessNewTournamentSettings(s);
                App.Instance.GetSceneMgr().SendStartGameRequest();
            }));
        }
    }
}
