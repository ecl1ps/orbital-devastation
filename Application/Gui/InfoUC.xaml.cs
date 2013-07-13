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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for InfoUC.xaml
    /// </summary>
    public partial class InfoUC : UserControl
    {
        public InfoUC(string text)
        {
            InitializeComponent();
            tbInfo.Text = text;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ClearMenus();
        }
    }
}
