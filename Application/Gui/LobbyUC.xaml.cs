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
    /// Interaction logic for LobbyUC.xaml
    /// </summary>
    public partial class LobbyUC : UserControl
    {
        private bool leader;

        public LobbyUC(bool asLeader)
        {
            leader = asLeader;
            InitializeComponent();
            if (asLeader)
            {
                btnReady.Visibility = Visibility.Hidden;
            }
            else
            {
                btnStartGame.Visibility = Visibility.Hidden;
            }
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (Application.Current as App).SendChatMessage(tbMessage.Text);
                lvChat.Items.Add(tbMessage.Text);
                tbMessage.Text = "";
            }
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).StartTournamentGame();
            btnStartGame.IsEnabled = false;
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            btnReady.IsEnabled = false;
            (Application.Current as App).PlayerReady();
        }

        public void AllReady()
        {
            if (leader)
                btnStartGame.IsEnabled = true;
        }

        private void lobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (leader)
                (Application.Current as App).PlayerReady();
        }
    }
}
