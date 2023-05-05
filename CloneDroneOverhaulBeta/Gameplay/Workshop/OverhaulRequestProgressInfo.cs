namespace CDOverhaul
{
    public class OverhaulRequestProgressInfo
    {
        public OverhaulRequestProgressInfo NewProgress()
        {
            return new OverhaulRequestProgressInfo();
        }

        private float m_Progress;
        public float Progress => m_Progress;

        public static void SetProgress(OverhaulRequestProgressInfo progress, float value)
        {
            if(progress == null)
            {
                return;
            }
            progress.m_Progress = value;
        }
    }
}
