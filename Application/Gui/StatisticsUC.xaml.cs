using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orbit.Core;
using Orbit.Core.AI;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for StatisticsUC.xaml
    /// </summary>
    public partial class StatisticsUC : UserControl
    {
        public StatisticsUC()
        {
            InitializeComponent();
            lblHSText1.Content = String.Format(Strings.Culture, Strings.ui_statistics_solo_vs_bot, Strings.bot_name_1);
            lblHSText2.Content = String.Format(Strings.Culture, Strings.ui_statistics_solo_vs_bot, Strings.bot_name_2);
            lblHSText3.Content = String.Format(Strings.Culture, Strings.ui_statistics_solo_vs_bot, Strings.bot_name_3);
            lblHSText4.Content = String.Format(Strings.Culture, Strings.ui_statistics_solo_vs_bot, Strings.bot_name_4);
            lblHSText5.Content = String.Format(Strings.Culture, Strings.ui_statistics_solo_vs_bot, Strings.bot_name_5);

            lblHS1.Content = GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO1);
            lblHS2.Content = GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO2);
            lblHS3.Content = GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO3);
            lblHS4.Content = GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO4);
            lblHS5.Content = GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_SOLO5);
            lblHS6.Content = GameProperties.Props.Get(PropertyKey.PLAYER_HIGHSCORE_QUICK_GAME);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowStartScreen();
        }
    }
}
