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

        private double minPosition;
        private double maxPosition;
        private double maxRotation;

        public AlertBox()
        {
            InitializeComponent();
            sizeLeft = 190;
            sizeRight = 190;

            minPosition = -60;
            maxPosition = 5;
            maxRotation = 180;

            Slip(1);
        }

        public void OpenDoors(double val)
        {
            if (val > 1)
                val = 1;

            DoorLeft.Margin = new Thickness(-FastMath.LinearInterpolate(sizeLeft, 0, val), 0, 0, 0);
            DoorRight.Margin = new Thickness(0, 0, -FastMath.LinearInterpolate(sizeRight, 0, val), 0);
            RotateSprockets(val);
        }

        public void Hide(bool hide)
        {
            if (hide)
                this.Visibility = System.Windows.Visibility.Hidden;
            else
                this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Slip(double val)
        {
            if (val > 1)
                val = 1;

            Canvas.SetTop(Panel, FastMath.LinearInterpolate(maxPosition, minPosition, val));
            Canvas.SetTop(TextBlock, FastMath.LinearInterpolate(maxPosition, minPosition, val) + 5);
            RotateSprockets(val);
        }

        private void RotateSprockets(double val)
        {
            SprocketLeft.RenderTransform = new RotateTransform(FastMath.LinearInterpolate(0, maxRotation, val), 15, 15);
            SprocketRight.RenderTransform = new RotateTransform(FastMath.LinearInterpolate(maxRotation, 0, val), 15, 15);
        }

        public void SetText(String text)
        {
            TextBlock.Text = text;
        }
    }
}
