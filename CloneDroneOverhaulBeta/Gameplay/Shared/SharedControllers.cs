namespace CDOverhaul.Shared
{
    /// <summary>
    /// Utilities in short
    /// </summary>
    public class SharedControllers : OverhaulController
    {
        public VoxelsController Voxels { get; private set; }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
            Voxels = OverhaulController.InitializeController<VoxelsController>();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}