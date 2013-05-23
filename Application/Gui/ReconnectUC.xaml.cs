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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for InfoUC.xaml
    /// </summary>
    public partial class ReconnectUC : UserControl
    {
        private string serverAddress;

        public ReconnectUC(string serverAddress)
        {
            InitializeComponent();
            this.serverAddress = serverAddress;
        }

        private void btnReconnect_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.StartTournamentGame(serverAddress, 0);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowStartScreen();
        }
    }
}
