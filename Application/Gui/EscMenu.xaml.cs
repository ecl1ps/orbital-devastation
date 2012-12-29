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
using Orbit.Core.Client;
using Orbit.Core.Client.GameStates;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for EscMenu.xaml
    /// </summary>
    public partial class EscMenu : UserControl
    {
        public EscMenu()
        {
            InitializeComponent();
            if (App.WindowInstance.GameRunning)
            {
                btnMainMenu.Content = "Exit Match";
            }

            if (LogicalTreeHelper.FindLogicalNode(App.WindowInstance.mainGrid, "mainUC") != null)
            {
                btnExit.Margin = btnMainMenu.Margin;
                btnMainMenu.Visibility = Visibility.Hidden;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ClearMenus();

            DependencyObject lobby = LogicalTreeHelper.FindLogicalNode(App.WindowInstance.mainGrid, "lobbyWindow");
            if (App.WindowInstance.GameRunning || lobby != null)
            {
                App.Instance.GetSceneMgr().Enqueue(new Action(() =>
                {
                    App.Instance.GetSceneMgr().PlayerQuitGame();
                }));
            }
            else
                App.Instance.ShowStartScreen();

            App.Instance.ShutdownServerIfExists();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            App.WindowInstance.ShowOptionsMenu();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ClearMenus();
            if (App.WindowInstance.GameRunning)
                StaticMouse.Enable(true);
        }
    }
}
