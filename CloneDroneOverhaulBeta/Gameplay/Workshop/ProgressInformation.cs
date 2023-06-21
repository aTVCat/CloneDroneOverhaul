namespace CDOverhaul
{
    public class ProgressInformation
    {
        public ProgressInformation NewProgress() => new ProgressInformation();

        public float Progress { get; private set; }

        public static void SetProgress(ProgressInformation progress, float value)
        {
            if (progress == null)
                return;

            progress.Progress = value;
        }
    }
}
