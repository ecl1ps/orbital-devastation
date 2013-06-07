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
using System.Windows.Controls.Primitives;
using Orbit.Core;
using Orbit.Core.Players;
using Orbit.Core.Server.Match;
using Orbit.Core.Server.Level;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for BotSelection.xaml
    /// </summary>
    public partial class BotSelection : UserControl
    {
        private BotType type;

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
                type = BotType.LEVEL1;
                TextArea.Text = String.Concat(Strings.bot_text_1, Environment.NewLine, Environment.NewLine, String.Format(Strings.Culture, Strings.ui_best_highscore, GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO1)));
            }
            else if (selected.Equals(HookerBotButton))
            {
                type = BotType.LEVEL2;
                TextArea.Text = String.Concat(Strings.bot_text_2, Environment.NewLine, Environment.NewLine, String.Format(Strings.Culture, Strings.ui_best_highscore, GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO2)));
            }
            else if (selected.Equals(MedicoreBotButton))
            {
                type = BotType.LEVEL3;
                TextArea.Text = String.Concat(Strings.bot_text_3, Environment.NewLine, Environment.NewLine, String.Format(Strings.Culture, Strings.ui_best_highscore, GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO3)));
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
            App.Instance.CreateLoadingGui();

            TournamentSettings s = new TournamentSettings();
            s.MMType = MatchManagerType.SKIRMISH;
            s.Level = GameLevel.BASIC_MAP;
            s.RoundCount = 1;
            s.BotCount = 1;
            s.BotType = type;

            App.Instance.StartSoloGame(s);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ClearMenus();
        }
    }
}
