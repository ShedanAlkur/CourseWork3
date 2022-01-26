using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class AnimationSet
    {
        public Sprite[] Frames;
        public float FrameDelay;

        public AnimationSet(Sprite[] frames, float frameDelay)
        {
            Frames = frames ?? throw new ArgumentNullException(nameof(frames));
            FrameDelay = frameDelay;
        }
    }
}
