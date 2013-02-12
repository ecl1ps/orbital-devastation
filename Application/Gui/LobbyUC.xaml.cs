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
        private bool receivedSettings;
        private int round;

        public LobbyUC(bool asLeader, bool settingsLocked = false)
        {
            leader = asLeader;
            receivedSettings = false;
            InitializeComponent();

            btnReady.IsEnabled = false;
            if (!settingsLocked)
                btnStats.IsEnabled = false;

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
                foreach (MatchManagerType t in Enum.GetValues(typeof(MatchManagerType)))
                {
                    MatchManagerInfo i = MatchManagerInfoAccessor.GetInfo(t);
#if DEBUG   // v debugu pridat vsechny managery
                    data.Add(new ComboData { Id = t, Name = i.Text });
#else
                    if (!i.IsDebug)
                        data.Add(new ComboData { Id = t, Name = i.Text });
#endif
                }

                cbType.ItemsSource = data;
                cbType.DisplayMemberPath = "Name";
                cbType.SelectedValuePath = "Id";
                cbType.SelectedValue = MatchManagerType.ONLY_SCORE;

                data = new List<ComboData>();
                foreach (GameLevel l in Enum.GetValues(typeof(GameLevel)))
                {
                    LevelInfo i = LevelInfoAccessor.GetInfo(l);
#if DEBUG   // v debugu pridat vsechny mapy
                    data.Add(new ComboData { Id = l, Name = i.Text });
#else
                    if (!i.IsDebug)
                        data.Add(new ComboData { Id = l, Name = i.Text });
#endif
                }

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

        [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
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
            btnReady.Content = ready ? Strings.ui_ready_not : Strings.ui_ready;
            App.Instance.PlayerReady(ready);
        }

        public void AllReady(bool ready = true)
        {
            if (leader)
                btnStartGame.IsEnabled = ready;
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
                if (receivedSettings)
                    btnReady.IsEnabled = true;
                lblColorNotice.Visibility = Visibility.Hidden;
            }

            int rounds = 1;
            try
            {
                if (tbRounds.Visibility == Visibility.Visible)
                    rounds = int.Parse(tbRounds.Text, CultureInfo.InvariantCulture);
                else
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
                s.RoundCount = int.Parse(tbRounds.Text, CultureInfo.InvariantCulture);
                if (s.RoundCount < 1)
                    throw new Exception();
            }
            catch (Exception)
            {
                s.RoundCount = 1;
                tbRounds.Text = 1.ToString(Strings.Culture);
                UpdateMatchCount(s.RoundCount);
            }

#if !DEBUG
            s.BotCount = 0;
            s.BotType = SharedDef.DEFAULT_BOT;
#else
            try
            {
            	s.BotCount = int.Parse(tbBotCount.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                s.BotCount = 0;
            }
            
            s.BotType = (BotType)cbBot.SelectedValue;
#endif

            App.Instance.GetSceneMgr().Enqueue(new Action(() =>
            {
                App.Instance.GetSceneMgr().ProcessNewTournamentSettings(s);
            }));
        }

        internal void UpdateTournamentSettings(TournamentSettings s)
        {
            receivedSettings = true;
            btnReady.IsEnabled = true;
            lblType.Content = MatchManagerInfoAccessor.GetInfo(s.MMType).Text;
            lblMap.Content = LevelInfoAccessor.GetInfo(s.Level).Text;
            lblRounds.Content = s.RoundCount.ToString(Strings.Culture);
            round = s.PlayedMatches;
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
                rounds = int.Parse(tbRounds.Text, CultureInfo.InvariantCulture);
                if (rounds < 1)
                    throw new Exception();
                UpdateMatchCount(rounds);
                btnSettings.IsEnabled = true;
            }
            catch (Exception)
            {
                tbRounds.Text = string.Empty;
                lblMatches.Content = string.Empty;
            }
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

        private void tbBotCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int bots = 0;
            try
            {
                bots = int.Parse(tbBotCount.Text, CultureInfo.InvariantCulture);
                if (bots < 0 || bots > 2)
                    throw new Exception();
                btnSettings.IsEnabled = true;
            }
            catch (Exception)
            {
                if (!String.IsNullOrWhiteSpace(tbBotCount.Text))
                    tbBotCount.Text = 0.ToString(Strings.Culture);
            }
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            StatisticsTabbedPanel stats = GuiObjectFactory.CreateStatisticsTabbedPanel(App.Instance.GetSceneMgr());
            App.Instance.AddMenu(stats);
        }
    }
}
