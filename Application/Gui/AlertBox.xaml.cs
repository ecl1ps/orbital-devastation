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
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for AlertBox.xaml
    /// </summary>
    public partial class AlertBox : UserControl
    {
        private double sizeLeft;
        private double sizeRight;

        public AlertBox()
        {
            InitializeComponent();
            sizeLeft = 70;
            sizeRight = 70;
            Visibility = System.Windows.Visibility.Hidden;
        }

        public void OpenDoors(double val)
        {
            DoorLeft.Margin = new Thickness(-FastMath.LinearInterpolate(sizeLeft, 0, val), 0, 0, 0);
            DoorRight.Margin = new Thickness(0, 0, -FastMath.LinearInterpolate(sizeRight, 0, val), 0);
        }

        public void Hide(bool hide)
        {
            if (hide)
                this.Visibility = System.Windows.Visibility.Hidden;
            else
                this.Visibility = System.Windows.Visibility.Visible;
        }

        public void SetText(String text)
        {
            TextBlock.Text = text;
        }
    }
}
