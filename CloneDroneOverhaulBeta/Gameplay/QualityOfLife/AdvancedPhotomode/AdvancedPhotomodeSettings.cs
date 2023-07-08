namespace CDOverhaul.Gameplay.QualityOfLife
{
    public static class AdvancedPhotomodeSettings
    {
        [AdvancedPhotomodeSetting("Override parameters", "General")]
        public static bool OverrideSettings = false;
        public static bool IsOverridingSettings => OverrideSettings && AdvancedPhotomodeController.Instance.PhotoManager.IsInPhotoMode();

        [AdvancedPhotomodeSetting("Enable", "Ambient Occlusion")]
        public static bool SSAOEnable = true;

        [AdvancedPhotomodeSliderParameters(0.1f, 1.2f)]
        [AdvancedPhotomodeSetting("Intensity", "Ambient Occlusion")]
        public static float SSAOIntensity = 0.75f;

        [AdvancedPhotomodeSliderParameters(0, 3)]
        [AdvancedPhotomodeSetting("Sample Count", "Ambient Occlusion")]
        public static int SSAOSampleCount = 1;
    }
}
