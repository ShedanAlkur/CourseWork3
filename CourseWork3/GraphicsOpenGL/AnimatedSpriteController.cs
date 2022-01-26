using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class AnimatedSpriteController : SpriteController
    {
        AnimatedSprite animatedSprite;
        AnimationSet currentAnimationSet;
        float currentFrameDelay;
        int currentFrameIndex;

        public void ChangeAnimationSet(string animationSetName) 
        {
            currentAnimationSet = animatedSprite.Animations[animationSetName];
        }
        
        public override Sprite GetSprite()
        {
            return currentAnimationSet.Frames[currentFrameIndex];
        }

        public override void Update(float elapsedTime)
        {
            currentFrameDelay += elapsedTime;
            if (currentFrameDelay >= currentAnimationSet.FrameDelay)
            {
                currentFrameDelay -= currentAnimationSet.FrameDelay;
                if (++currentFrameIndex >= currentAnimationSet.Frames.Length)
                    currentFrameIndex = 0;
            }
        }
    }
}
