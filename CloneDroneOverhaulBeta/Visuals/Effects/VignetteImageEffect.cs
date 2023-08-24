using OverhaulAPI.SharedMonoBehaviours;
using System;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class VignetteImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSetting("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;

        [OverhaulSettingSliderParameters(false, -0.2f, 0.3f)]
        [OverhaulSetting("Graphics.Shaders.Vignette Intensity", 0.05f, false, null, "Graphics.Shaders.Vignette")]
        public static float VignetteIntensity;

        private static Material s_Material;
        private static readonly Func<bool> s_EnableFunction = () => VignetteEnabled;

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (!s_Material)
                s_Material = OverhaulAssetsController.GetAsset<Material>("M_IE_Spotlight", OverhaulAssetPart.Part2);

            s_Material.SetFloat("_CenterY", -0.14f);

            if (PreviousCamera && PreviousCamera != CurrentCamera)
            {
                OverhaulImageEffect effectComponent = GetImageEffect(PreviousCamera, "Vignette");
                if (effectComponent)
                {
                    Destroy(effectComponent);
                }
            }
            if (CurrentCamera)
            {
                OverhaulImageEffect effectComponent = GetImageEffect(camera, "Vignette");
                if (!effectComponent)
                {
                    effectComponent = OverhaulImageEffect.AddEffect(camera, s_Material, s_EnableFunction);
                    effectComponent.effectName = "Vignette";
                }
            }
        }
    }
}
