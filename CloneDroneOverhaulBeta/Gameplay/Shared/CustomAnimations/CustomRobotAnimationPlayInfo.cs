namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationPlayInfo
    {
        public CustomRobotAnimation Animation { get; set; }
        public float TimePlayedAtAll { get; set; }
        public float TimeToNextFrame { get; set; }

        public void ResetToNextPlay(in CustomRobotAnimation anim)
        {
            Animation = anim;
            TimePlayedAtAll = 0f;
            TimeToNextFrame = 0f;
        }

        public void OnNewFrame(in CustomRobotAnimationFPMExtention ext)
        {

        }
    }
}
