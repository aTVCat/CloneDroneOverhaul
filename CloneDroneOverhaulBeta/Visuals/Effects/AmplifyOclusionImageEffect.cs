using AmplifyOcclusion;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class AmplifyOclusionImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSettingAttribute_Old("Graphics.Amplify Occlusion.Enable", true, false, "Add shadows to everything", "AmbientOcc.png")]
        public static bool AOEnabled;

        [OverhaulSettingSliderParameters(false, 0.7f, 1.3f)]
        [OverhaulSettingAttribute_Old("Graphics.Amplify Occlusion.Intensity", 0.75f, false, null, "Graphics.Amplify Occlusion.Enable")]
        public static float AOIntensity;

        [OverhaulSettingDropdownParameters("Low@Medium@High@Very high")]
        [OverhaulSettingAttribute_Old("Graphics.Amplify Occlusion.Sample Count", 1, false, null, "Graphics.Amplify Occlusion.Enable")]
        public static int AOSampleCount;

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
