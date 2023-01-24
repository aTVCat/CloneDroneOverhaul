namespace CDOverhaul.Shared
{
    public class SharedControllers : ModController
    {
        public VolumeEditorController VolumeEditor { get; private set; }
        public AltVolumeEditorController AltVolumeEditor { get; private set; }

        public override void Initialize()
        {
            VolumeEditor = ModControllerManager.NewController<VolumeEditorController>();
            AltVolumeEditor = ModControllerManager.NewController<AltVolumeEditorController>();
        }
    }
}