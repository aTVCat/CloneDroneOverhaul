namespace CDOverhaul.Visuals
{
    public class OverhaulUIEffectBehaviour : OverhaulBehaviour
    {
        protected OverhaulCameraManager CameraManager;

        public override void Start()
        {
            CameraManager = OverhaulCameraManager.reference;
        }
    }
}
