namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
