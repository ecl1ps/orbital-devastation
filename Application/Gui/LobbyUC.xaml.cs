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
using Orbit.Core.Server.Match;
using Orbit.Core.Server;
using Orbit.Core;
using Orbit.Core.Server.Level;
using Orbit.Core.Players;
using Orbit.Core.AI;
using Orbit.Core.Helpers;
using System.Globalization;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for LobbyUC.xaml
    /// </summary>
    public partial class LobbyUC : UserControl
    {
        private bool leader;
        private bool ready;
        private int round;

        public LobbyUC(bool asLeader, bool firstMatch = true)
        {
            InitializeComponent();

            if (firstMatch)
                btnStats.IsEnabled = false;

            SetIsLeader(asLeader);
            
#if !DEBUG
            lblBots.Visibility = Visibility.Hidden;
            lblBotLevel.Visibility = Visibility.Hidden;
            lblBot.Visibility = Visibility.Hidden;
            lblBotCount.Visibility = Visibility.Hidden;
#endif
        }

        public void SetIsLeader(bool asLeader)
        {
            if (asLeader == leader)
                return;

            leader = asLeader;

            if (asLeader)
            {
                btnReady.Visibility = Visibility.Hidden;
                ready = true;
            }
            else
            {
                btnStartGame.Visibility = Visibility.Hidden;
                btnReadyCheck.Visibility = Visibility.Hidden;
                ready = false;
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
            if (String.IsNullOrWhiteSpace(tbMessage.Text))
                return;

            App.Instance.SendChatMessage(tbMessage.Text);
            lvChat.Items.Add(App.Instance.PlayerName + ": " + tbMessage.Text);
            lvChat.ScrollIntoView(lvChat.Items[lvChat.Items.Count - 1]);
            tbMessage.Text = string.Empty;
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.StartTournamentGame();
#if !DEBUG
            btnStartGame.IsEnabled = false;
#endif
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            ready = !ready;
            SetReadyState(ready);
        }

        public void SetReadyState(bool rdy)
        {
            ready = rdy;
            btnReady.Content = rdy ? Strings.ui_ready_not : Strings.ui_ready;
            App.Instance.PlayerReady(ready);
        }

        public void AllReady(bool ready = true)
        {
            if (leader)
            {
#if !DEBUG
                btnStartGame.IsEnabled = ready;
#endif
                btnReadyCheck.IsEnabled = !ready;
                if (ready)
                    App.Instance.FocusWindow();
            }
        }

        private void lobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            ready = true;
            btnReady.Content = Strings.ui_ready_not;
            App.Instance.PlayerReady(true);
#else
            if (leader)
                App.Instance.PlayerReady(true);
#endif
            tbMessage.Focus();
        }

        public void UpdateShownPlayers(List<LobbyPlayerData> updatedPlayers)
        {
            spPlayers.Children.Clear();

            IEnumerable<LobbyPlayerData> sortedData = updatedPlayers.OrderByDescending(p => p.Won).
                ThenByDescending(p => p.Score).ThenByDescending(p => p.Leader).ThenBy(p => p.Name);
            foreach (LobbyPlayerData data in sortedData)
                AddPlayer(data);

            if (PlayersHaveSameColor(updatedPlayers))
            {
                if (!leader)
                    btnReady.IsEnabled = false;

                lblColorNotice.Visibility = Visibility.Visible;
            }
            else
            {
                btnReady.IsEnabled = true;
                lblColorNotice.Visibility = Visibility.Hidden;
            }

            int rounds = 1;
            try
            {
                rounds = int.Parse((string)lblRounds.Content, CultureInfo.InvariantCulture);
                if (rounds < 1)
                    throw new Exception();
            }
            catch (Exception)
            {
                rounds = 1;
            }
            UpdateMatchCount(rounds);
        }

        private bool PlayersHaveSameColor(List<LobbyPlayerData> updatedPlayers)
        {
            foreach (LobbyPlayerData p in updatedPlayers)
                if (updatedPlayers.Exists(pl => pl.Id != p.Id && p.Color == pl.Color))
                    return true;
            return false;
        }

        private void AddPlayer(LobbyPlayerData data)
        {
            // TODO: ted se jich vejde 6
            if (spPlayers.Children.Count < 6)
            {
                LobbyPlayer p = new LobbyPlayer(data);
                if (leader)
                    p.EnableKickButton();
                spPlayers.Children.Add(p);
            }
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.GetSceneMgr().Enqueue(new Action(() =>
            {
                App.Instance.GetSceneMgr().PlayerQuitGame();
            }));
            App.Instance.ShutdownServerIfExists();
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            Grid main = App.WindowInstance.mainGrid;
            ColorPickerUC cp = LogicalTreeHelper.FindLogicalNode(main, "colorPicker") as ColorPickerUC;
            if (cp == null)
                App.Instance.AddMenu(new ColorPickerUC());
        }

        public void UpdateTournamentSettings(TournamentSettings s)
        {
            btnReady.IsEnabled = true;
            lblName.Content = s.Name;
            lblType.Content = MatchManagerInfoAccessor.GetInfo(s.MMType).Text;
            lblMap.Content = LevelInfoAccessor.GetInfo(s.Level).Text;
            lblRounds.Content = s.RoundCount.ToString(Strings.Culture);
            round = s.PlayedMatches;
            UpdateMatchCount(s.RoundCount);

#if DEBUG
            lblBotLevel.Content = BotNameAccessor.GetBotName(s.BotType);
            lblBots.Content = s.BotCount.ToString(Strings.Culture);
#endif
            // TODO: mozna se pozdeji pridaji boti i pro normalni hrace - potom se zde musi zobrazit, kdyz prijde zprava
        }

        private void UpdateMatchCount(int rounds)
        {
            if (lblMatches == null)
                return;

            int players = spPlayers.Children.Count > 1 ? spPlayers.Children.Count : 2;

            // kombinace
            int matches = GameManager.GetRequiredNumberOfMatches(players, rounds);
            lblMatches.Content = String.Format(Strings.Culture, Strings.ui_tournament_match_count, matches);
            lblMatchNumber.Text = String.Format(Strings.Culture, Strings.ui_tournament_current_match, (round + 1), matches);
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            StatisticsTabbedPanel stats = GuiObjectFactory.CreateStatisticsTabbedPanel(App.Instance.GetSceneMgr());
            App.Instance.AddMenu(stats);
        }

        private void btnReadyCheck_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.GetSceneMgr().Enqueue(new Action(() => App.Instance.GetSceneMgr().RequestReadyCheck()));
        }

        public void ReadyCheckRequested()
        {
            ReadyCheckUC rc = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "readyCheckDialog") as ReadyCheckUC;
            if (rc == null)
                App.Instance.AddMenu(new ReadyCheckUC(this));
        }
    }
}
