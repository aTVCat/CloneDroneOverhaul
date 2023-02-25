using CDOverhaul.ArenaRemaster;

namespace CDOverhaul.Patches
{
    public class NewArenaActivator : ReplacementBase
    {
        private ArenaRemasterController _controller;

        public override void Replace()
        {
            base.Replace();

            _controller = OverhaulController.InitializeController<ArenaRemasterController>();

            SuccessfullyPatched = true;
        }

        public ArenaRemasterController GetController()
        {
            return _controller;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
