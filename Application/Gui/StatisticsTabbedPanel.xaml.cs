﻿using System;
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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for StatisticsTabbedPanel.xaml
    /// </summary>
    public partial class StatisticsTabbedPanel : UserControl
    {
        public StatisticsTabbedPanel()
        {
            InitializeComponent();
        }

        public void AddItem(UIElement elem, String name)
        {
            TabItem item = new TabItem();
            item.Header = name;
            item.Content = elem;

            Panel.Items.Add(item);
        }
    }
}
