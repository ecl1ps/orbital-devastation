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
using System.IO;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core;
using System.Globalization;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for GameOptions.xaml
    /// </summary>
    public partial class GameOptions : UserControl
    {
        public GameOptions()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.WindowInstance.ShowOptionsMenu();
        }

        private void cbLocalizations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo ci = ((sender as ComboBox).SelectedItem as CultureInfo);
            App.SetLocalization(ci);
            GameProperties.Props.SetAndSave(PropertyKey.GAME_LANGUAGE, ci.Name);
        }
    }
}
