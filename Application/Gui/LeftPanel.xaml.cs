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
using Orbit.Gui.InteractivePanel;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for LeftPanel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl, IInteractivePanel
    {
        public LeftPanel()
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
            if (!Panel.Children.Contains(elem))
                Panel.Children.Remove(elem);
        }

        public void ClearAll()
        {
            Panel.Children.Clear();
        }
    }
}
