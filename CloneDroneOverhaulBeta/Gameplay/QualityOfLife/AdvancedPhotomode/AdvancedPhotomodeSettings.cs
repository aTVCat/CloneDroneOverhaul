using CDOverhaul.BuiltIn.AdditionalContent;
using UnityEngine;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public static class AdvancedPhotomodeSettings
    {
        public static Material SkyboxMaterial;

        [AdvancedPhotomodeSetting("Override parameters", "General")]
        public static bool OverrideSettings = false;
        public static bool IsOverridingSettings => OverrideSettings && AdvancedPhotomodeController.Instance && AdvancedPhotomodeController.Instance.PhotoManager.IsInPhotoMode();

        [AdvancedPhotomodeSetting("Enable", "Ambient Occlusion")]
        public static bool SSAOEnable = true;

        [AdvancedPhotomodeSliderParameters(0.1f, 1.2f)]
        [AdvancedPhotomodeSetting("Intensity", "Ambient Occlusion")]
        public static float SSAOIntensity = 0.75f;

        [AdvancedPhotomodeSliderParameters(0, 3)]
        [AdvancedPhotomodeSetting("Sample Count", "Ambient Occlusion")]
        public static int SSAOSampleCount = 1;


        [AdvancedPhotomodeRequireContent(MoreSkyboxesController.RequiredContent)]
        [AdvancedPhotomodeSliderParameters(-1, 15)]
        [AdvancedPhotomodeSetting("Skybox ID", "More skyboxes")]
        public static int MoreSkyboxesIndex = -1;
    }
}
