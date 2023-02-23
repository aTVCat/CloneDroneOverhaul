using CDOverhaul.ArenaRemaster;

namespace CDOverhaul.Patches
{
    public class NewArenaActivator : ReplacementBase
    {
        private ArenaRemasterController _controller;

        public override void Replace()
        {
            base.Replace();

            _controller = ModControllerManager.InitializeController<ArenaRemasterController>();

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
