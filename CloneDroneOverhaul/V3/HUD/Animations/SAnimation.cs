using System.Collections.Generic;

namespace CloneDroneOverhaul.V3.Gameplay.Animations
{
    public struct SAnimation
    {
        public SAnimation(in string name, in EAnimationType animationType)
        {
            AnimationName = name;
            AnimationType = animationType;
        }

        /// <summary>
        /// Get level editor animation
        /// </summary>
        /// <returns></returns>
        public LevelEditorAnimation LevelEditorAnimationInstance()
        {
            LevelEditorAnimation result = null;

            if (AnimationType == EAnimationType.LevelEditorAnimation)
            {
                List<LevelEditorAnimation> list = LevelEditorAnimationManager.Instance.GetAnimationsInLevel();
                foreach (LevelEditorAnimation anim in list)
                {
                    if (anim.AnimationName == AnimationName)
                    {
                        result = anim;
                        break;
                    }
                }
            }

            return result;
        }

        public string AnimationName;
        public EAnimationType AnimationType;
    }
}
