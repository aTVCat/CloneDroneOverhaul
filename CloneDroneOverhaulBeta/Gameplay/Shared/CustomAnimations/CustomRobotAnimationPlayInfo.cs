using UnityEngine;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationPlayBehaviour
    {
        public CustomRobotAnimation Animation { get; private set; }
        public bool AnimationIsNull { get; set; }

        public float TimeAnimationStartedToPlay { get; set; }
        public float TimePlayedAtAll { get; set; }
        public int CurrentFrame { get; set; }

        public CustomRobotAnimationPlayBehaviour()
        {
            AnimationIsNull = true;
        }

        public void StartPlaying(in CustomRobotAnimation anim)
        {
            Animation = anim;
            TimePlayedAtAll = 0f;
            TimeAnimationStartedToPlay = Time.time;
            AnimationIsNull = anim == null;
        }

        public void SetFrame(in CustomRobotAnimationFPMExtention ext, in int frame)
        {
            if (AnimationIsNull)
            {
                return;
            }
            CurrentFrame = frame;
            Animation.SetFrame(frame, ext.OwnerModel);
        }

        public void OnUpdate(in CustomRobotAnimationFPMExtention ext)
        {
            if (ext.IsPlayingCustomAnimation && !AnimationIsNull)
            {
                TimePlayedAtAll = Time.time - TimeAnimationStartedToPlay;
                int frame = Mathf.RoundToInt(TimePlayedAtAll * 60f);
                SetFrame(ext, frame);
            }
        }
    }
}
