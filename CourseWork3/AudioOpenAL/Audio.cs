using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.AudioOpenAL
{
    class Audio : Game.IUpdateable
    {
        protected static Audio instance;
        public static Audio Instance
        {
            get
            {
                if (instance == null)
                    instance = new Audio();
                return instance;
            }
        }

        public void Init()
        {

        }

        float EffectsVolume;
        float MusicVolume;
        bool AllowOverlayingOfSoundEffects;

        Audiotrack currentMusicAudiotrack;
        float CurrentMusicPlaybackTime;

        List<PlaybackSoundEffect> OngoingSountEffects;

        public void PlaySoundEffect(Audiotrack audiotrack) { }

        public void PlayMusic(Audiotrack audiotrack) { }
        public void PauseMusic() { }
        public void StopMusic() { }

        public void Update(float elapsedTime)
        {
            throw new NotImplementedException();
        }
    }
}
