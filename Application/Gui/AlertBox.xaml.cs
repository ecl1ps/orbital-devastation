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
        private double widthLeft;
        private double widthRight;

        public AlertBox()
        {
            InitializeComponent();
            widthLeft = DoorLeft.Width;
            widthRight = DoorRight.Width;
        }

        public void OpenDoors(double val)
        {
            DoorLeft.Width = FastMath.LinearInterpolate(widthLeft, 0, val);
        }

        public void Hide(bool hide)
        {
            if (hide)
                this.Visibility = System.Windows.Visibility.Hidden;
            else
                this.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
