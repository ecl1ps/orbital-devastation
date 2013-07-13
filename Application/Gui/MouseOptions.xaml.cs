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
using Orbit.Core.Client;
using System.Windows.Controls.Primitives;
using Orbit.Core;
using System.Collections;
using System.Globalization;
using Orbit.Core.Client.GameStates;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for MouseOptions.xaml
    /// </summary>
    public partial class MouseOptions : UserControl
    {
        public MouseOptions()
        {
            InitializeComponent();
            SetValues();
        }

        private void SetValues()
        {
            ChangeMouseButton(Boolean.Parse(GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_ENABLED)));
            double sensitivity = Double.Parse(GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_SENSITIVITY), CultureInfo.InvariantCulture);
            SensitivitySlider.Value = 50 - ((1 - sensitivity) * 100);
            SensitivityLabel.Text = sensitivity.ToString("0.00", CultureInfo.InvariantCulture);

            SelectSelectedIcon();
        }

        private void SelectSelectedIcon()
        {
            String url = GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_CURSOR);

            if (url.Equals("pack://application:,,,/resources/images/mouse/targeting_icon3.png"))
                MouseIcon3.IsChecked = true;
            else if (url.Equals("pack://application:,,,/resources/images/mouse/targeting_icon.png"))
                MouseIcon2.IsChecked = true;
            else
                MouseIcon1.IsChecked = true;
        }

        private void SensitivityChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = 1 - ((50 - e.NewValue) / 100);
            SensitivityLabel.Text = val.ToString("0.00", CultureInfo.InvariantCulture);
            StaticMouse.SENSITIVITY = (float) val;

            GameProperties.Props.SetAndSave(PropertyKey.STATIC_MOUSE_SENSITIVITY, val.ToString(Strings.Culture));
        }

        private void Allow(bool allow)
        {
            StaticMouse.ALLOWED = allow;
            ChangeMouseButton(allow);

            GameProperties.Props.SetAndSave(PropertyKey.STATIC_MOUSE_ENABLED, allow);
        }

        private void ChangeMouseButton(bool enabled)
        {
            UntoggleButtons(null);
            if (enabled)
                SelectSelectedIcon();

            MouseIcon1.IsEnabled = enabled;
            MouseIcon2.IsEnabled = enabled;
            MouseIcon3.IsEnabled = enabled;

            SensitivitySlider.IsEnabled = enabled;

            mouseEnabled.IsChecked = enabled;
        }

        private void UntoggleButtons(ToggleButton except)
        {
            if (MouseIcon1 != except)
                MouseIcon1.IsChecked = false;
            if (MouseIcon2 != except)
                MouseIcon2.IsChecked = false;
            if (MouseIcon3 != except)
                MouseIcon3.IsChecked = false;
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        private void MouseIcon1_Click(object sender, RoutedEventArgs e)
        {
            ProccesCursorIconChange(e, new Uri("pack://application:,,,/resources/images/mouse/targeting_icon2.png"));
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        private void MouseIcon2_Click(object sender, RoutedEventArgs e)
        {
            ProccesCursorIconChange(e, new Uri("pack://application:,,,/resources/images/mouse/targeting_icon.png"));
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        private void MouseIcon3_Click(object sender, RoutedEventArgs e)
        {
            ProccesCursorIconChange(e, new Uri("pack://application:,,,/resources/images/mouse/targeting_icon3.png"));
        }

        private void ProccesCursorIconChange(RoutedEventArgs e, Uri uri)
        {
            UntoggleButtons(e.Source as ToggleButton);
            mouseEnabled.IsChecked = true;

            if (StaticMouse.Instance != null)
                StaticMouse.Instance.InitCursorImage(uri);

            GameProperties.Props.SetAndSave(PropertyKey.STATIC_MOUSE_CURSOR, uri.ToString());
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.WindowInstance.ShowOptionsMenu();
        }

        private void mouseEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            Allow(false);
        }

        private void mouseEnabled_Checked(object sender, RoutedEventArgs e)
        {
            Allow(true);
        }
    }
}
