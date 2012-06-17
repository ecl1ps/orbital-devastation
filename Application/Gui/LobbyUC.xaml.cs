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
        private List<int> players;

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
            players = new List<int>();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbMessage.Text == "")
                    return;
                (Application.Current as App).SendChatMessage(tbMessage.Text);
                lvChat.Items.Add((Application.Current as App).GetPlayerName() + ": " +tbMessage.Text);
                lvChat.ScrollIntoView(lvChat.Items[lvChat.Items.Count - 1]);
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
            // nejdriv se odstrani hraci, kteri uz nejsou, ale jsou zobrazeni
            for (int i = 0; i < players.Count; ++i)
            {
                if (!updatedPlayers.Exists(p => p.Id == players[i]))
                    RemovePlayer(players[i]);
            }

            // pak se pridaji hraci, kteri jeste nejsou zobrazeni
            foreach (LobbyPlayerData data in updatedPlayers)
            {
                if (!players.Exists(id => id == data.Id))
                    AddPlayer(data);
            }

        }

        private void AddPlayer(LobbyPlayerData data)
        {
            if (spPlayers1.Children.Count < 5)
            {
                spPlayers1.Children.Add(new LobbyPlayer(data));
                players.Add(data.Id);
            }
            else if (spPlayers2.Children.Count < 5)
            {
                spPlayers2.Children.Add(new LobbyPlayer(data));
                players.Add(data.Id);
            }
        }

        private void RemovePlayer(int id)
        {
            bool removed = false;
            for (int i = 0; i < spPlayers1.Children.Count; ++i)
                if ((spPlayers1.Children[i] as LobbyPlayer).PlayerId == id)
                {
                    spPlayers1.Children.RemoveAt(i);
                    removed = true;
                    players.Remove(id);
                }

            if (removed)
                return;

            foreach (UIElement plr in spPlayers2.Children)
                if ((plr as LobbyPlayer).PlayerId == id)
                {
                    spPlayers2.Children.Remove(plr);
                    players.Remove(id);
                }
        }
    }
}
