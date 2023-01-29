using UnityEngine;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationPlayBehaviour
    {
        public CustomRobotAnimation Animation { get; set; }
        public float TimeAnimationStartedToPlay { get; set; }
        public float TimePlayedAtAll { get; set; }
        public int CurrentFrame { get; set; }

        public void ResetToNextPlay(in CustomRobotAnimation anim)
        {
            Animation = anim;
            TimePlayedAtAll = 0f;
            TimeAnimationStartedToPlay = Time.time;
        }

        public void SetFrame(in CustomRobotAnimationFPMExtention ext, in int frame)
        {
            if (Animation == null)
            {
                return;
            }
            CurrentFrame = frame;
            Animation.SetFrame(frame, ext.OwnerModel);
        }

        public void OnNewFrame(in CustomRobotAnimationFPMExtention ext)
        {
            if (ext.IsPlayingCustomAnimation)
            {
                TimePlayedAtAll = Time.time - TimeAnimationStartedToPlay;
                int frame = Mathf.RoundToInt(TimePlayedAtAll * 60f);
                SetFrame(ext, frame);
            }
        }
    }
}
