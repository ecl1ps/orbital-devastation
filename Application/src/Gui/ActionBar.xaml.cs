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
using System.Windows.Threading;
using Orbit.Core.Scene;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ActionBar.xaml
    /// </summary>
    public partial class ActionBar : UserControl
    {
        public ActionBar()
        {
            InitializeComponent();
        }

        public void AddItem(UIElement elem)
        {
            if (!Panel.Children.Contains(elem))
                Panel.Children.Add(elem);
        }

        public void RemoveItem(UIElement elem)
        {
            Panel.Children.Remove(elem);
        }
    }
}
