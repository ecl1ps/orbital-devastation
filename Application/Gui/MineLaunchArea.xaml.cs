using Orbit.Core;
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
    /// Interaction logic for MineLaunchArea.xaml
    /// </summary>
    public partial class MineLaunchArea : UserControl
    {
        public MineLaunchArea()
        {
            InitializeComponent();
        }

        public void SetPosition(Point position)
        {
            Canvas.SetLeft(this, position.X + 9);
            SetPercentage(0);
        }

        public void LoadColor(Color c)
        {
            Main.Background = new SolidColorBrush(Color.FromArgb((byte)100, c.R, c.G, c.B));
            LoadingLineLeft.Background = new SolidColorBrush(c);
            LoadingLineRight.Background = new SolidColorBrush(c);
        }

        public void SetPercentage(float value)
        {
            double move = FastMath.LinearInterpolate(0, 8, value);

            Canvas.SetRight(LoadingLineRight, move);
            Canvas.SetLeft(LoadingLineLeft, move);
        }
    }
}
