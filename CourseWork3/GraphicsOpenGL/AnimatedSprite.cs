using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class AnimatedSprite
    {
        public Dictionary<string, AnimationSet> Animations;

        public AnimatedSprite(Dictionary<string, AnimationSet> animations)
        {
            Animations = animations;
        }
    }
}
