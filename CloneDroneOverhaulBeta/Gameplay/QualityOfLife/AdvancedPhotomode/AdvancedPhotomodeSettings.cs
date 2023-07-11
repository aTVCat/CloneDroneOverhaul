using CDOverhaul.BuiltIn.AdditionalContent;
using UnityEngine;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public static class AdvancedPhotomodeSettings
    {
        public static Material SkyboxMaterial;

        public static bool FogEnabled;
        public static float FogStartDistance;
        public static float FogEndDistance;
        public static Color FogColor;

        public static void RememberCurrentSettings()
        {
            SkyboxMaterial = RenderSettings.skybox;

            FogEnabled = RenderSettings.fog;
            FogStartDistance = RenderSettings.fogStartDistance;
            FogEndDistance = RenderSettings.fogEndDistance;
            FogColor = RenderSettings.fogColor;
        }

        public static void RestoreSettings()
        {
            if (SkyboxMaterial)
                RenderSettings.skybox = SkyboxMaterial;

            RenderSettings.fog = FogEnabled;
            RenderSettings.fogStartDistance = FogStartDistance;
            RenderSettings.fogEndDistance = FogEndDistance;
            RenderSettings.fogColor = FogColor;
        }

        [AdvancedPhotomodeSetting("Override parameters", "General")]
        public static bool OverrideSettings = false;
        public static bool IsOverridingSettings => OverrideSettings && AdvancedPhotomodeController.Instance && AdvancedPhotomodeController.Instance.PhotoManager.IsInPhotoMode();


        [AdvancedPhotomodeSetting("Enable", "Fog")]
        public static bool Fog = true;

        [AdvancedPhotomodeSliderParameters(1f, 4000f)]
        [AdvancedPhotomodeSetting("Start Distance", "Fog")]
        public static float FogStart = 100f;

        [AdvancedPhotomodeSliderParameters(1.1f, 4000f)]
        [AdvancedPhotomodeSetting("End Distance", "Fog")]
        public static float FogEnd = 400f;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Hue", "Fog Color")]
        public static float FogColH;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Saturation", "Fog Color")]
        public static float FogColS;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Brightness", "Fog Color")]
        public static float FogColB;


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
