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
        public ISound Sound { get; set; }
        public String FileName { get; set; }
        public SoundType SoundType { get; set; }

        public FileSound(String soundName, String fileName)
        {
            this.soundName = soundName;
            this.FileName = fileName;
            SoundType = SoundType.EFFECTS;
        }

        public FileSound(String soundName, String fileName, SoundType type)
        {
            this.soundName = soundName;
            this.FileName = fileName;
            SoundType = type;
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
