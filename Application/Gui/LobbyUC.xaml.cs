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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for LobbyUC.xaml
    /// </summary>
    public partial class LobbyUC : UserControl
    {
        private bool leader;
        private bool ready;
        private bool receivedSettings;

        public LobbyUC(bool asLeader, bool settingsLocked = false)
        {
            leader = asLeader;
            receivedSettings = false;
            InitializeComponent();

            btnReady.IsEnabled = false;

            if (asLeader)
            {
                btnReady.Visibility = Visibility.Hidden;
                ready = true;
            }
            else
            {
                btnStartGame.Visibility = Visibility.Hidden;
                ready = false;
            }
            
            if (asLeader && !settingsLocked)
            {
                List<ComboData> data = new List<ComboData>();
                // pridani produkcnich manageru
                foreach (MatchManagerType t in Enum.GetValues(typeof(MatchManagerType)))
                {
                    MatchManagerInfo i = MatchManagerInfoAccessor.GetInfo(t);
                    if (!i.IsDebug)
                        data.Add(new ComboData { Id = t, Name = i.Text });
                }

#if DEBUG
                // pridani testovacich manageru
                foreach (MatchManagerType t in Enum.GetValues(typeof(MatchManagerType)))
                {
                    MatchManagerInfo i = MatchManagerInfoAccessor.GetInfo(t);
                    if (i.IsDebug)
                        data.Add(new ComboData { Id = t, Name = i.Text });
                }
#endif
                cbType.ItemsSource = data;
                cbType.DisplayMemberPath = "Name";
                cbType.SelectedValuePath = "Id";
                cbType.SelectedValue = MatchManagerType.WINS_THEN_SCORE;

                data = new List<ComboData>();
                // pridani produkcnich map
                foreach (GameLevel l in Enum.GetValues(typeof(GameLevel)))
                {
                    LevelInfo i = LevelInfoAccessor.GetInfo(l);
                    if (!i.IsDebug)
                        data.Add(new ComboData { Id = l, Name = i.Text });
                }
#if DEBUG
                // pridani testovacich map
                foreach (GameLevel l in Enum.GetValues(typeof(GameLevel)))
                {
                    LevelInfo i = LevelInfoAccessor.GetInfo(l);
                    if (i.IsDebug)
                        data.Add(new ComboData { Id = l, Name = i.Text });
                }
#endif

                cbMap.ItemsSource = data;
                cbMap.DisplayMemberPath = "Name";
                cbMap.SelectedValuePath = "Id";
                cbMap.SelectedValue = GameLevel.BASIC_MAP;

#if DEBUG
                // pridani dostupnych botu pro testovani
                data = new List<ComboData>();
                data.Add(new ComboData { Id = BotType.LEVEL1, Name = BotNameAccessor.GetBotName(BotType.LEVEL1) });
                data.Add(new ComboData { Id = BotType.LEVEL2, Name = BotNameAccessor.GetBotName(BotType.LEVEL2) });

                cbBot.ItemsSource = data;
                cbBot.DisplayMemberPath = "Name";
                cbBot.SelectedValuePath = "Id";
                cbBot.SelectedValue = BotType.LEVEL1;
#endif
            }
            else
            {
                btnSettings.Visibility = Visibility.Hidden;
                cbType.Visibility = Visibility.Hidden;
                cbMap.Visibility = Visibility.Hidden;
                tbRounds.Visibility = Visibility.Hidden;
                lblType.Visibility = Visibility.Visible;
                lblMap.Visibility = Visibility.Visible;
                lblRounds.Visibility = Visibility.Visible;

                tbBotCount.Visibility = Visibility.Hidden;
                cbBot.Visibility = Visibility.Hidden;
                lblBot.Visibility = Visibility.Hidden;
                lblBotCount.Visibility = Visibility.Hidden;
            }

#if !DEBUG
                tbBotCount.Visibility = Visibility.Hidden;
                cbBot.Visibility = Visibility.Hidden;
                lblBot.Visibility = Visibility.Hidden;
                lblBotCount.Visibility = Visibility.Hidden;
#endif
        }

        class ComboData
        {
            public object Id { get; set; }
            public string Name { get; set; }
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
#if !DEBUG
            btnStartGame.IsEnabled = false;
#endif
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            ready = true;
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
#if !DEBUG
            if (leader)
                (Application.Current as App).PlayerReady();
#else
            (Application.Current as App).PlayerReady();
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
                if (!ready && receivedSettings)
                    btnReady.IsEnabled = true;
                lblColorNotice.Visibility = Visibility.Hidden;
            }

            int rounds = 1;
            try
            {
                if (tbRounds.Visibility == Visibility.Visible)
                    rounds = int.Parse(tbRounds.Text);
                else
                    rounds = int.Parse((string)lblRounds.Content);
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

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            Grid main = (Application.Current.MainWindow as GameWindow).mainGrid;
            ColorPickerUC cp = LogicalTreeHelper.FindLogicalNode(main, "colorPicker") as ColorPickerUC;
            if (cp == null)
                (Application.Current.MainWindow as GameWindow).mainGrid.Children.Add(new ColorPickerUC());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            btnSettings.IsEnabled = false;
#if !DEBUG
            btnStartGame.IsEnabled = false;
#else
            btnStartGame.IsEnabled = true;
#endif

            TournamentSettings s = new TournamentSettings();
            s.MMType = (MatchManagerType)cbType.SelectedValue;
            s.Level = (GameLevel)cbMap.SelectedValue;
            try
            {
                s.RoundCount = int.Parse(tbRounds.Text);
                if (s.RoundCount < 1)
                    throw new Exception();
            }
            catch (Exception)
            {
                s.RoundCount = 1;
                tbRounds.Text = "1";
                UpdateMatchCount(s.RoundCount);
            }

#if !DEBUG
            s.BotCount = 0;
            s.BotType = SharedDef.DEFAULT_BOT;
#else
            s.BotCount = int.Parse(tbBotCount.Text);
            s.BotType = (BotType)cbBot.SelectedValue;
#endif

            (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
            {
                (Application.Current as App).GetSceneMgr().ProcessNewTournamentSettings(s);
            }));
        }

        internal void UpdateTournamentSettings(TournamentSettings s)
        {
            receivedSettings = true;
            btnReady.IsEnabled = true;
            lblType.Content = MatchManagerInfoAccessor.GetInfo(s.MMType).Text;
            lblMap.Content = LevelInfoAccessor.GetInfo(s.Level).Text;
            lblRounds.Content = s.RoundCount.ToString();
            UpdateMatchCount(s.RoundCount);

            // TODO: mozna se pozdeji pridaji boti i pro normalni hrace - potom se zde musi zobrazit, kdyz prijde zprava
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSettings.IsEnabled = true;
        }

        private void tbRounds_TextChanged(object sender, TextChangedEventArgs e)
        {
            int rounds = 1;
            try
            {
                rounds = int.Parse(tbRounds.Text);
                if (rounds < 1)
                    throw new Exception();
                UpdateMatchCount(rounds);
                btnSettings.IsEnabled = true;
            }
            catch (Exception)
            {
                tbRounds.Text = "";
                lblMatches.Content = "";
            }
        }

        private void UpdateMatchCount(int rounds)
        {
            if (lblMatches == null)
                return;

            int players = spPlayers.Children.Count > 1 ? spPlayers.Children.Count : 2;

            // kombinace
            int matches = GameManager.GetRequiredNumberOfMatches(players, rounds);
            if (matches > 1)
                lblMatches.Content = "(" + matches.ToString() + " matches)";
            else
                lblMatches.Content = "(" + matches.ToString() + " match)";
        }

        private void tbBotCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int bots = 0;
            try
            {
                bots = int.Parse(tbBotCount.Text);
                if (bots < 0 || bots > 2)
                    throw new Exception();
                btnSettings.IsEnabled = true;
            }
            catch (Exception)
            {
                tbBotCount.Text = "0";
            }
        }
    }
}
