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
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for OptionsUC.xaml
    /// </summary>
    public partial class OptionsUC : UserControl
    {
        public OptionsUC()
        {
            InitializeComponent();
        }

        private void btnSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).StartGame(Gametype.SKIRMISH);
        }

        private void btnHostNewGame_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).StartGame(Gametype.HOSTED_GAME);
        }

        private void btnFindHostedGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
