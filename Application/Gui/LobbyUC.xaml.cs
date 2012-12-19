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
                SendMessage();
            }
        }

        private void SendMessage()
        {
            if (tbMessage.Text == "")
                return;
            (Application.Current as App).SendChatMessage(tbMessage.Text);
            lvChat.Items.Add((Application.Current as App).PlayerName + ": " + tbMessage.Text);
            lvChat.ScrollIntoView(lvChat.Items[lvChat.Items.Count - 1]);
            tbMessage.Text = "";
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

        public void AllReady(bool ready = true)
        {
            if (leader)
                btnStartGame.IsEnabled = ready;
        }

        private void lobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (leader)
                (Application.Current as App).PlayerReady();
            tbMessage.Focus();
        }

        public void UpdateShownPlayers(List<LobbyPlayerData> updatedPlayers)
        {
            spPlayers.Children.Clear();

            // pridat leadera jako prvniho
            LobbyPlayerData d = updatedPlayers.Find(p => p.Leader);
            if (d != null)
                AddPlayer(d);

            // pak zbytek hracu
            foreach (LobbyPlayerData data in updatedPlayers)
                if (!data.Leader)
                    AddPlayer(data);
        }

        private void AddPlayer(LobbyPlayerData data)
        {
            // TODO: ted se jich vejde alespon 6
            if (spPlayers.Children.Count < 6)
                spPlayers.Children.Add(new LobbyPlayer(data));
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
            {
                (Application.Current as App).GetSceneMgr().PlayerQuitGame();
            }));
            (Application.Current as App).ShutdownServerIfExists();
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}
