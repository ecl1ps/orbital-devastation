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
using System.Windows.Controls.Primitives;
using Orbit.Core.Client;
using Orbit.Core.Client.Sounds;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for SoundOptions.xaml
    /// </summary>
    public partial class SoundOptions : UserControl
    {
        public SoundOptions()
        {
            InitializeComponent();
        }

        private void SoundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundValue.Content = e.NewValue;
            float volume = (float)(e.NewValue / 100);
            SoundManager.Instance.SoundsByType(SoundType.EFFECTS).ForEach(sound => sound.Volume = volume);
        }

        private void BackgroundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BackgroundValue.Content = e.NewValue;
            float volume = (float) (e.NewValue / 100);
            SoundManager.Instance.SoundsByType(SoundType.MUSIC).ForEach(sound => sound.Volume = volume);
        }

        private void EnabledChanged(object sender, RoutedEventArgs e)
        {
            bool enabled = !SoundManager.Instance.Enabled;
            SoundManager.Instance.Enabled = enabled;
            BackgroundSlider.IsEnabled = enabled;
            SoundSlider.IsEnabled = enabled;

            if (enabled)
                SoundButton.Content = "Disable sound";
            else
                SoundButton.Content = "Enable sound";

        }
    }
}
