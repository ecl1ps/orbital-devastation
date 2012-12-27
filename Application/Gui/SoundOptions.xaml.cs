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
using Orbit.Core;

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
            LoadValues();
        }

        private void LoadValues()
        {
            EnableSounds(SoundManager.Instance.Enabled);

            float soundVolume = float.Parse(GameProperties.Props.Get(PropertyKey.SOUNDS_VOLUME));
            SoundSlider.Value = soundVolume * 100;
            SoundValue.Text = (soundVolume * 100).ToString("0.##");

            float musicVolume = float.Parse(GameProperties.Props.Get(PropertyKey.MUSIC_VOLUME));
            BackgroundSlider.Value = musicVolume * 100;
            BackgroundValue.Text = (musicVolume * 100).ToString("0.##");

        }

        private void SoundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundValue.Text = e.NewValue.ToString("0.##");
            float volume = (float)(e.NewValue / 100);
            SoundManager.Instance.SetSoundVolume(volume);

            GameProperties.Props.SetAndSave(PropertyKey.SOUNDS_VOLUME, volume.ToString());
        }

        private void BackgroundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BackgroundValue.Text = e.NewValue.ToString("0.##");
            float volume = (float) (e.NewValue / 100);
            SoundManager.Instance.SetMusicVolume(volume);

            GameProperties.Props.SetAndSave(PropertyKey.MUSIC_VOLUME, volume.ToString());
        }

        private void EnabledChanged(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.Enabled = !SoundManager.Instance.Enabled;
            EnableSounds(SoundManager.Instance.Enabled);
        }

        private void EnableSounds(bool enabled)
        {
            BackgroundSlider.IsEnabled = enabled;
            SoundSlider.IsEnabled = enabled;

            if (enabled)
                SoundButton.Content = "Disable sound";
            else
                SoundButton.Content = "Enable sound";

            GameProperties.Props.SetAndSave(PropertyKey.MUSIC_ENABLED, enabled.ToString());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as GameWindow).ShowOptions(this);
        }
    }
}
