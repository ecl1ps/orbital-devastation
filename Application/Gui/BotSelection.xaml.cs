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
using System.Windows.Controls.Primitives;
using Orbit.Core;
using Orbit.Core.Players;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for BotSelection.xaml
    /// </summary>
    public partial class BotSelection : UserControl
    {
        public const string TWO_LINES = "\r\n\r\n";

        public BotSelection()
        {
            InitializeComponent();
        }

        private void BotSelectionEvent(object sender, RoutedEventArgs e)
        {
            UntoggleAllButtons();
            ToggleButton selected = e.Source as ToggleButton;
            
            StartGameButton.IsEnabled = true;
            selected.IsChecked = true;

            if (selected.Equals(EasyBotButton))
            {
                SharedDef.DEFAULT_BOT = BotType.LEVEL1;
                TextArea.Text = SharedDef.BOT_LEVEL_1_TEXT + TWO_LINES + "Best highscore: " + GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO1);
            }
            else if (selected.Equals(HookerBotButton))
            {
                SharedDef.DEFAULT_BOT = BotType.LEVEL2;
                TextArea.Text = SharedDef.BOL_LEVEL_2_TEXT + TWO_LINES + "Best highscore: " + GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO2);
            }
            else
                throw new Exception("Unsupported bot type");
        }

        private void UntoggleAllButtons()
        {
            EasyBotButton.IsChecked = false;
            HookerBotButton.IsChecked = false;
            MedicoreBotButton.IsChecked = false;
            HardcoreBotButton.IsChecked = false;
            NightmareBotButton.IsChecked = false;
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).CreateGameGui();
            (Application.Current as App).StartSoloGame();
        }
    }
}