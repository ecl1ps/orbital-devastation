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
            if ((Application.Current.MainWindow as GameWindow).GameRunning)
            {
                btnMainMenu.Content = "Exit Match";
            }
            if (LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "mainUC") != null)
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
            (Application.Current.MainWindow as GameWindow).mainGrid.Children.Remove(this);

            if ((Application.Current.MainWindow as GameWindow).GameRunning)
            {
                (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
                {
                    (Application.Current as App).GetSceneMgr().PlayerQuitGame();
                }));
            }
            else
                (Application.Current as App).ShowStartScreen();

            (Application.Current as App).ShutdownServerIfExists();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as GameWindow).ShowOptions(this);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Panel).Children.Remove(this);
            if ((Application.Current.MainWindow as GameWindow).GameRunning)
                StaticMouse.Enable(true);
        }
    }
}
