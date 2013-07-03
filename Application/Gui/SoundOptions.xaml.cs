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
using System.Windows.Controls.Primitives;
using Orbit.Core.Client;
using Orbit.Core.Client.Sounds;
using Orbit.Core;
using System.Globalization;

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

            float soundVolume = float.Parse(GameProperties.Props.Get(PropertyKey.SOUNDS_VOLUME), CultureInfo.InvariantCulture);
            SoundSlider.Value = soundVolume * 100;
            SoundValue.Text = (soundVolume * 100).ToString("0.##", Strings.Culture);

            float musicVolume = float.Parse(GameProperties.Props.Get(PropertyKey.MUSIC_VOLUME), CultureInfo.InvariantCulture);
            BackgroundSlider.Value = musicVolume * 100;
            BackgroundValue.Text = (musicVolume * 100).ToString("0.##", Strings.Culture);
        }

        private void SoundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundValue.Text = e.NewValue.ToString("0.##", Strings.Culture);
            float volume = (float)(e.NewValue / 100);
            SoundManager.Instance.SetSoundVolume(volume);

            GameProperties.Props.SetAndSave(PropertyKey.SOUNDS_VOLUME, volume.ToString(Strings.Culture));
        }

        private void BackgroundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BackgroundValue.Text = e.NewValue.ToString("0.##", Strings.Culture);
            float volume = (float) (e.NewValue / 100);
            SoundManager.Instance.SetMusicVolume(volume);

            GameProperties.Props.SetAndSave(PropertyKey.MUSIC_VOLUME, volume.ToString(Strings.Culture));
        }

        private void EnableSounds(bool enabled)
        {
            SoundManager.Instance.Enabled = enabled;

            BackgroundSlider.IsEnabled = enabled;
            SoundSlider.IsEnabled = enabled;

            soundEnabled.IsChecked = enabled;

            GameProperties.Props.SetAndSave(PropertyKey.MUSIC_ENABLED, enabled.ToString(Strings.Culture));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.WindowInstance.ShowOptionsMenu();
        }

        private void soundEnabled_Checked(object sender, RoutedEventArgs e)
        {
            EnableSounds(true);
        }

        private void soundEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableSounds(false);
        }
    }
}
