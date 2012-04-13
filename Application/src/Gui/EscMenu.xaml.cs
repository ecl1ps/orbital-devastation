﻿using System;
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
    /// Interaction logic for EscMenu.xaml
    /// </summary>
    public partial class EscMenu : UserControl
    {
        public EscMenu()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as GameWindow).mainGrid.Children.Remove(this);
            (Application.Current as App).ShowStartScreen();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}