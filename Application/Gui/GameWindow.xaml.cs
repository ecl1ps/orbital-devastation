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
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    UIElement uc;
                    if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "playerSettings") as UIElement) != null)
                    {
                        mainGrid.Children.Remove(uc);
                        mainGrid.Children.Add(new OptionsMenu());
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "optionsMenu") as UIElement) != null)
                    {
                        mainGrid.Children.Remove(uc);
                        mainGrid.Children.Add(new EscMenu());
                    } 
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "escMenu") as UIElement) != null)
                    {
                        mainGrid.Children.Remove(uc);
                    } else
                        mainGrid.Children.Add(new EscMenu());
                    break;
            }
        }

    }
}
