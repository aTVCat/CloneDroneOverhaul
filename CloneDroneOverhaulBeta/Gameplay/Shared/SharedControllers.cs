namespace CDOverhaul.Shared
{
    /// <summary>
    /// Utilities in short
    /// </summary>
    public class SharedControllers : OverhaulController
    {
        public CustomAnimationsController CustomAnimations { get; private set; }
        public VoxelsController Voxels { get; private set; }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
            CustomAnimations = OverhaulController.InitializeController<CustomAnimationsController>();
            Voxels = OverhaulController.InitializeController<VoxelsController>();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}