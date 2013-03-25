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
    /// Interaction logic for ReadyCheckUC.xaml
    /// </summary>
    public partial class ReadyCheckUC : UserControl
    {
        private LobbyUC lobbyUC;

        public ReadyCheckUC(LobbyUC lobbyUC)
        {
            this.lobbyUC = lobbyUC;
            InitializeComponent();
            App.Instance.FocusWindow();
            // TODO: zvuk
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            lobbyUC.SetReadyState(true);
            App.Instance.ClearMenus();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ClearMenus();
        }
    }
}
