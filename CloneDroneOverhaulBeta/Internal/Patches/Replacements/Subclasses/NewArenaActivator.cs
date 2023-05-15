namespace CDOverhaul.Patches
{
    public class ArenaRevampActivator : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
