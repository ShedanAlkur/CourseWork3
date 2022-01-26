using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.AudioOpenAL
{
    class PlaybackSoundEffect : Game.IUpdateable
    {
        public Audiotrack Audiotrack;
        public float CurrentPlaybackTime;

        public void Update(float elapsedTime)
        {
            CurrentPlaybackTime += CurrentPlaybackTime;
        }
    }
}
