using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xna.Framework;
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
    public partial class OptionsUC : UserControl
    {
        public OptionsUC()
        {
            InitializeComponent();
        }

        private void btnPlayer_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new PlayerSettings());
        }

        private void btnMouse_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new MouseOptions());
        }

        private void btnSound_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new SoundOptions());
        }

        private void btnKeys_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new KeyBindingsOptions());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new EscMenu());
        }

        private void btnGame_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.AddMenu(new GameOptions());
        }
    }
}
