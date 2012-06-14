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
using Orbit.Core.Utils;
using Orbit.Core.Scene;

namespace Orbit.Gui
{
    /// <summary>
    /// </summary>
    public abstract partial class GoldActionWindow : UserControl
    {
        public GoldActionWindow()
        {
            InitializeComponent();
        }

        protected void Init()
        {
            ButtonImage.Source = InitImage();
            InitText();
        }

        public abstract BitmapImage InitImage();

        public abstract void InitText();

        public abstract void OnClick(object sender, RoutedEventArgs e);

        public abstract Object GetId();
    }
}
