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
using System.IO;
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for OptionsMenu.xaml
    /// </summary>
    public partial class OptionsMenu : UserControl
    {
        public OptionsMenu()
        {
            InitializeComponent();
        }

        private void btnPlayer_Click(object sender, RoutedEventArgs e)
        {
            ShowAnotherWindow(new PlayerSettings());
        }

        private void btnMouse_Click(object sender, RoutedEventArgs e)
        {
            ShowAnotherWindow(new MouseOptions());
        }

        private void btnSound_Click(object sender, RoutedEventArgs e)
        {
            ShowAnotherWindow(new SoundOptions());
        }

        private void ShowAnotherWindow(UserControl window)
        {
            (Parent as Panel).Children.Remove(this);
            (Application.Current.MainWindow as GameWindow).mainGrid.Children.Add(window);
        }

        private void btnKeys_Click(object sender, RoutedEventArgs e)
        {
            ShowAnotherWindow(new KeyBindingsOptions());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Panel).Children.Remove(this);
            (Application.Current.MainWindow as GameWindow).mainGrid.Children.Add(new EscMenu());
        }
    }
}
