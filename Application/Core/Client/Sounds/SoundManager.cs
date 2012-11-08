﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client.Sounds;
using System.Timers;
using IrrKlang;
using Lidgren.Network;

namespace Orbit.Core.Client
{
    public enum PlayType
    {
        ONCE,
        INFINITY,
        FIXED_TIME
    }

    public class SoundTime
    {
        public float Time { get; set; }
        public ISound Sound { get; set; }
    }

    public class SoundManager
    {

        private ISoundEngine soundEngine;
        private ISoundEngine musicEngine;
        
        private bool enabled;
        public bool Enabled { get { return enabled; } set {disable(value); } }

        private List<FileSound> music;
        private List<FileSound> sounds;
 
        public SoundManager()
        {
            soundEngine = new ISoundEngine();
            musicEngine = new ISoundEngine();

            sounds = new List<FileSound>();
            music = new List<FileSound>();
            enabled = true;

            PrepareDefaultSounds();
            LoadSettings();
        }

        private void LoadSettings()
        {
            bool loaded = bool.Parse(GameProperties.Props.Get(PropertyKey.MUSIC_ENABLED));
            enabled = loaded;

            float soundValue = float.Parse(GameProperties.Props.Get(PropertyKey.SOUNDS_VOLUME));
            setSoundVolume(soundValue);

            float musicValue = float.Parse(GameProperties.Props.Get(PropertyKey.MUSIC_VOLUME));
            setMusicVolume(musicValue);
        }

        private void PrepareDefaultSounds()
        {
            sounds.Add(new FileSound(SharedDef.MUSIC_BACKGROUND_ACTION, "background/action.ogg", SoundType.MUSIC));
            sounds.Add(new FileSound(SharedDef.MUSIC_BACKGROUND_CALM, "background/stanza-poem.ogg", SoundType.MUSIC));
            sounds.Add(new FileSound(SharedDef.MUSIC_EXPLOSION, "weapons/mine-explosion.ogg"));
            sounds.Add(new FileSound(SharedDef.MUSIC_SHOOT, "weapons/shot.ogg"));
            sounds.Add(new FileSound(SharedDef.MUSIC_DAMAGE_TO_BASE, "misc/crash.ogg"));

            sounds.ForEach(sound => {
                if (sound.SoundType == SoundType.MUSIC)
                    sound.LoadMusic(musicEngine);
                else
                    sound.LoadMusic(soundEngine);
            });
        }

        private void disable(bool value) {
            enabled = value;

            if (!value)
            {
                soundEngine.StopAllSounds();
                musicEngine.StopAllSounds();
            }
            else
            {
                setMusicVolume(float.Parse(GameProperties.Props.Get(PropertyKey.MUSIC_VOLUME)));
                music.ForEach(sound => StartPlayingInfinite(sound));
            }
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
            if (!music.Contains(sound))
                music.Add(sound);

            if (!Enabled)
                setMusicVolume(0);

            sound.Sound = musicEngine.Play2D(sound.SoundName, true);
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
            
            sound.Sound = soundEngine.Play2D(sound.SoundName);
            return sound;
        }

        public FileSound StartPlayingForSpecifiedTime(String soundName, long howLong)
        {
            return StartPlayingForSpecifiedTime(SoundByName(soundName), howLong);
        }

        public FileSound StartPlayingForSpecifiedTime(FileSound sound, long howLong)
        {
            if (!Enabled)
                return null;

            sound.Sound = soundEngine.Play2D(sound.SoundName, true);
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
            if (sound.Sound != null)
            {
                sound.Sound.Stop();
                if (music.Contains(sound))
                    music.Remove(sound);
            }
        }

        public void StopAllSounds()
        {
            soundEngine.StopAllSounds();
            musicEngine.StopAllSounds();
            music.Clear();
        }

        public void setMusicVolume(float volume)
        {
            if (volume < 0 || volume > 1)
                throw new Exception("Volume must be value between 0 and 1");

            musicEngine.SoundVolume = volume;
        }

        public void setSoundVolume(float volume)
        {
            if (volume < 0 || volume > 1)
                throw new Exception("Volume must be value between 0 and 1");

            soundEngine.SoundVolume = volume;
        }

        public void BroadcastSoundMessage(SceneMgr mgr, PlayType playType, String sound)
        {
            if (playType == PlayType.FIXED_TIME)
                throw new Exception("U must specify time.");

            NetOutgoingMessage message = mgr.CreateNetMessage();
            message = PrepareSoundMessage(message, playType, sound);

            mgr.SendMessage(message);
        }

        public void BroadcastSoundMessage(SceneMgr mgr, PlayType playType, String sound, float time)
        {
            NetOutgoingMessage message = mgr.CreateNetMessage();
            message = PrepareSoundMessage(message, playType, sound);
            message.Write(time);

            mgr.SendMessage(message);
        }

        public void ReadSoundMessage(NetIncomingMessage message)
        {
            PlayType type = (PlayType) message.ReadInt32();
            String sound = message.ReadString();

            switch (type)
            {
                case PlayType.ONCE:
                    StartPlayingOnce(sound);
                    break;

                case PlayType.INFINITY:
                    StartPlayingInfinite(sound);
                    break;

                case PlayType.FIXED_TIME:
                    long time = message.ReadInt64();
                    StartPlayingForSpecifiedTime(sound, time);
                    break;
            }
        }

        private NetOutgoingMessage PrepareSoundMessage(NetOutgoingMessage message, PlayType playType, String sound)
        {
            message.Write((int)PacketType.PLAY_SOUND);
            message.Write((int)playType);
            message.Write(sound);

            return message;
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
