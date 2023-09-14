using OverhaulAPI.SharedMonoBehaviours;
using System;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class VignetteImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.OTHER, "Enable Vignette")]
        public static bool VignetteEnabled = true;

        [OverhaulSettingSliderParameters(false, -0.2f, 0.3f)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.OTHER, "Vignette intensity")]
        public static float VignetteIntensity = 0.05f;

        private static Material s_Material;
        private static readonly Func<bool> s_EnableFunction = () => VignetteEnabled;

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (!s_Material)
                s_Material = OverhaulAssetLoader.GetAsset<Material>("M_IE_Spotlight", OverhaulAssetPart.Part2);

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
