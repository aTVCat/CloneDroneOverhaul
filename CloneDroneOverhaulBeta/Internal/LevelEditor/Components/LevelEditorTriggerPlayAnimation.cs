using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorTriggerPlayAnimation : MonoBehaviour
    {
        [IncludeInLevelEditor(false, false)]
        public string AnimationName;

        public void PlayAnimation()
        {
            List<LevelEditorAnimation> anims = LevelEditorAnimationManager.Instance.GetAnimationsInLevel();
            foreach (LevelEditorAnimation anim in anims)
            {
                if (!anim.IsPlaying() && anim.AnimationName.Equals(AnimationName))
                {
                    anim.Play();
                    break;
                }
            }
        }
    }
}