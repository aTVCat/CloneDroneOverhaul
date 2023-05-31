namespace CDOverhaul
{
    public class OverhaulRequestProgressInfo
    {
        public OverhaulRequestProgressInfo NewProgress() => new OverhaulRequestProgressInfo();

        public float Progress { get; private set; }

        public static void SetProgress(OverhaulRequestProgressInfo progress, float value)
        {
            if (progress == null)
                return;

            progress.Progress = value;
        }
    }
}
