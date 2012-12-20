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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for PlayerSettings.xaml
    /// </summary>
    public partial class PlayerSettings : UserControl
    {
        public PlayerSettings()
        {
            InitializeComponent();
            tbPlayerName.Text = (Application.Current as App).PlayerName;
        }

        private void btnSaveName_Click(object sender, RoutedEventArgs e)
        {
            if (tbPlayerName.Text.Length < 2)
            {
                tbPlayerName.Text = string.Empty;
                return;
            }

            (Application.Current as App).PlayerName = tbPlayerName.Text;
            (Application.Current as App).PlayerHashId = Player.GenerateNewHashId(tbPlayerName.Text);
            GameProperties.Props.Set(PropertyKey.PLAYER_NAME, tbPlayerName.Text);
            GameProperties.Props.SetAndSave(PropertyKey.PLAYER_HASH_ID, (Application.Current as App).PlayerHashId);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as GameWindow).ShowOptions(this);
        }
    }
}
