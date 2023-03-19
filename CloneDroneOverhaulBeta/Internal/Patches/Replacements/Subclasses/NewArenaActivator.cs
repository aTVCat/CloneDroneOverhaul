using CDOverhaul.ArenaRemaster;
using CDOverhaul.Graphics.ArenaRevamp;

namespace CDOverhaul.Patches
{
    public class ArenaRevampActivator : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            OverhaulController.AddController<ArenaRevampController>();

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
