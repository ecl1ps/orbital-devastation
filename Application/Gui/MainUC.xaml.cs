﻿using System;
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
#endif
        }

        private void btnSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowBotSelectionGui();
        }

        private void btnQuickGame_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).CreateGameGui();
            (Application.Current as App).StartHostedGame();
        }

        private void btnHostTournament_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).StartTournamentLobby();
        }

        private void btnFindHostedGame_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).LookForGame();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRepeatGame_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).RepeatGame();
        }

        private void btnConnectToLocalhost_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).CreateGameGui();
            (Application.Current as App).ConnectToGame("127.0.0.1");
        }

        private void btnShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowStatisticsGui();
        }

        private void btnShowOptions_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).AddMenu(new OptionsMenu());
        }
    }
}
