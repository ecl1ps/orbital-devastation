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
            (Application.Current as App).AddMenu(new PlayerSettings());
        }

        private void btnMouse_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).AddMenu(new MouseOptions());
        }

        private void btnSound_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).AddMenu(new SoundOptions());
        }

        private void btnKeys_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).AddMenu(new KeyBindingsOptions());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).AddMenu(new EscMenu());
        }
    }
}
