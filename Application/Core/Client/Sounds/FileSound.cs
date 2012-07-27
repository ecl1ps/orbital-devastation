using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrKlang;
using System.IO;
using System.Windows;

namespace Orbit.Core.Client.Sounds
{
    public enum SoundType
    {
        MUSIC,
        EFFECTS
    }

    public class FileSound
    {
        private const String APP_PATH = "pack://application:,,,/resources/sounds/";

        private String soundName;
        public String SoundName { get { return soundName; } }
        private float volume = 1;
        public float Volume { get { return volume; } set { UpdateVolume(value); } }
        private ISound sound;
        public ISound Sound { get { return sound; } set { UpdateSound(value); } }
        public String FileName { get; set; }
        public SoundType SoundType { get; set; }

        public FileSound(String soundName, String fileName)
        {
            this.soundName = soundName;
            this.FileName = fileName;
        }

        private void UpdateVolume(float value)
        {
            volume = value;
            if (Sound != null)
                Sound.Volume = value;
        }

        private void UpdateSound(ISound value)
        {
            sound = value;
            if (Sound != null)
                Sound.Volume = Volume;
        }

        public void LoadMusic(ISoundEngine engine)
        {
            Stream stream = Application.GetResourceStream(GetFileUri()).Stream;
            
            int lenght = (int)stream.Length;
            byte[] buffer = new byte[lenght];
          
            try
            {
                int count;
                int sum = 0;

                while ((count = stream.Read(buffer, sum, lenght - sum)) > 0)
                    sum += count;

            }
            finally
            {
                stream.Close();
            }

            engine.AddSoundSourceFromMemory(buffer, SoundName);
        }

        public Uri GetFileUri() {
            return new Uri(APP_PATH + FileName);
        }

    }
}
