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
            tbPlayerName.Text = App.Instance.PlayerName;
            colorPickerField.Background = new SolidColorBrush(Player.GetChosenColor());
        }

        private void btnSaveName_Click(object sender, RoutedEventArgs e)
        {
            SavePlayerName();
        }

        private void SavePlayerName()
        {
            if (tbPlayerName.Text.Length < 2)
            {
                tbPlayerName.Text = string.Empty;
                return;
            }

            App.Instance.PlayerName = tbPlayerName.Text;
            App.Instance.PlayerHashId = Player.GenerateNewHashId(tbPlayerName.Text);
            GameProperties.Props.Set(PropertyKey.PLAYER_NAME, tbPlayerName.Text);
#if DEBUG
            GameProperties.Props.Set(PropertyKey.AVAILABLE_COLORS, ((int)PlayerColorSet.END - 1).ToString());
            PlayerColorManager.RefreshPlayerColors();
#endif
            GameProperties.Props.SetAndSave(PropertyKey.PLAYER_HASH_ID, App.Instance.PlayerHashId);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.WindowInstance.ShowOptionsMenu();
        }

        private void colorPicker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Instance.AddMenu(new ColorPickerUC(true));
        }

        private void playerSettings_LostFocus(object sender, RoutedEventArgs e)
        {
            SavePlayerName();
        }
    }
}
