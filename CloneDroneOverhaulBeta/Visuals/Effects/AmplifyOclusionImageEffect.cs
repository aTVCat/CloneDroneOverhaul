using AmplifyOcclusion;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class AmplifyOclusionImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.AMPLIFY_OCCLUSION, "Enable SSAO")]
        public static bool AOEnabled = true;

        [OverhaulSettingSliderParameters(false, 0.7f, 1.3f)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.AMPLIFY_OCCLUSION, "SSAO Intensity")]
        public static float AOIntensity = 0.75f;

        [OverhaulSettingDropdownParameters("Low@Medium@High@Very high")]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.AMPLIFY_OCCLUSION, "SSAO Sample count")]
        public static int AOSampleCount = 1;

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (PreviousCamera && PreviousCamera != CurrentCamera)
            {
                AmplifyOcclusionEffect amplifyOcclusionEffect = PreviousCamera.GetComponent<AmplifyOcclusionEffect>();
                if (amplifyOcclusionEffect)
                    Destroy(amplifyOcclusionEffect);
            }
            if (CurrentCamera)
            {
                AmplifyOcclusionEffect effect = CurrentCamera.GetComponent<AmplifyOcclusionEffect>();
                if (!effect)
                    effect = CurrentCamera.gameObject.AddComponent<AmplifyOcclusionEffect>();

                effect.PerPixelNormals = AmplifyOcclusionEffect.PerPixelNormalSource.None;
                effect.Bias = 0f;
                effect.BlurSharpness = 4f;
                effect.FilterResponse = 0.7f;
                effect.Bias = 0.2f;
                effect.SampleCount = (SampleCountLevel)AOSampleCount;
                effect.Intensity = AOIntensity;
                effect.ApplyMethod = AmplifyOcclusionEffect.ApplicationMethod.PostEffect;
                effect.enabled = AOEnabled;
            }
        }
    }
}
