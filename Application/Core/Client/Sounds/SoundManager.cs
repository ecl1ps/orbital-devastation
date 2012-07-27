using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client.Sounds;
using System.Timers;
using IrrKlang;

namespace Orbit.Core.Client
{
    public class SoundTime
    {
        public float Time { get; set; }
        public ISound Sound { get; set; }
    }

    public class SoundManager
    {
        public ISoundEngine SoundEngine { get; set; }
        private bool enabled;
        public bool Enabled { get { return enabled; } set {disable(value); } }
        private List<FileSound> sounds;
        
 
        public SoundManager()
        {
            SoundEngine = new ISoundEngine();
            sounds = new List<FileSound>();
            enabled = true;

            PrepareDefaultSounds();
        }

        private void PrepareDefaultSounds()
        {
            sounds.Add(new FileSound(SharedDef.MUSIC_BACKGROUND_ACTION, "background/action.wav"));
            sounds.Add(new FileSound(SharedDef.MUSIC_BACKGROUND_CALM, "background/stanza-poem.ogg"));
            sounds.Add(new FileSound(SharedDef.MUSIC_EXPLOSION, "weapons/mine-explosion.wav"));
            sounds.Add(new FileSound(SharedDef.MUSIC_SHOOT, "weapons/shot.wav"));
            sounds.Add(new FileSound(SharedDef.MUSIC_DAMAGE_TO_BASE, "misc/crash.wav"));

            sounds.ForEach(sound => sound.LoadMusic(SoundEngine));
        }

        private void disable(bool value) {
            enabled = value;
            
            if (!Enabled)
                StopAllSounds();
        }

        public FileSound SoundByName(String soundName)
        {
            FileSound finded = sounds.Find(sound => sound.SoundName.Equals(soundName));
            if (finded == null)
                throw new Exception("Sound with name " + soundName + " does not exists!");

            return finded;
        }

        public FileSound StartPlayingInfinite(String soundName)
        {
            return StartPlayingInfinite(SoundByName(soundName));
        }

        public FileSound StartPlayingInfinite(FileSound sound)
        {
            if (!Enabled)
                return null;

            sound.Sound = SoundEngine.Play2D(sound.SoundName, true);
            return sound;
        }

        public FileSound StartPlayingOnce(String soundName)
        {
            return StartPlayingOnce(SoundByName(soundName));
        }

        public FileSound StartPlayingOnce(FileSound sound)
        {
            if (!Enabled)
                return null;

            sound.Sound = SoundEngine.Play2D(sound.SoundName);
            return sound;
        }

        public FileSound StartPlayingMoreThenOnce(FileSound sound, long howLong)
        {
            if (!Enabled)
                return null;

            sound.Sound = SoundEngine.Play2D(sound.SoundName, true);
            Timer timer = new Timer(howLong);
            timer.Elapsed += new ElapsedEventHandler((sender, e) => StopSound(sound));
            timer.Start();
            return sound;
        }

        public void StopSound(String soundName)
        {
            StopSound(SoundByName(soundName));
        }

        public void StopSound(FileSound sound)
        {
            if(sound.Sound != null)
                sound.Sound.Stop();
        }

        public void StopAllSounds()
        {
            SoundEngine.StopAllSounds();
        }

        private static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SoundManager();
                
                return instance;
            }
        }

    }
}
