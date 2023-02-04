namespace CDOverhaul.Shared
{
    /// <summary>
    /// Utilities in short
    /// </summary>
    public class SharedControllers : ModController
    {
        public CustomAnimationsController CustomAnimations { get; private set; }
        public VoxelsController Voxels { get; private set; }

        public override void Initialize()
        {
            CustomAnimations = ModControllerManager.NewController<CustomAnimationsController>();
            Voxels = ModControllerManager.NewController<VoxelsController>();
        }
    }
}