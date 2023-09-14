using OverhaulAPI.SharedMonoBehaviours;
using System;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class ChromaticAberrationImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.SHADERS, "Enable Chromatic aberration shader")]
        public static bool ChromaticAberrationEnabled = true;

        [OverhaulSettingSliderParameters(false, 0f, 0.001f)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.SHADERS, "Chromatic aberration intensity")]
        public static float ChromaticAberrationIntensity = 0.0002f;

        private static Material s_Material;
        private static readonly Func<bool> s_EnableFunction = () => ChromaticAberrationEnabled;

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (!s_Material)
                s_Material = OverhaulAssetLoader.GetAsset<Material>("M_IE_ChromaticAb", OverhaulAssetPart.Part2);

            s_Material.SetFloat("_RedX", -0.0007f - ChromaticAberrationIntensity);
            s_Material.SetFloat("_BlueX", 0.0007f + ChromaticAberrationIntensity);

            if (PreviousCamera && PreviousCamera != CurrentCamera)
            {
                OverhaulImageEffect effectComponent = GetImageEffect(PreviousCamera, "ChromaticAberration");
                if (effectComponent)
                {
                    Destroy(effectComponent);
                }
            }
            if (CurrentCamera)
            {
                OverhaulImageEffect effectComponent = GetImageEffect(camera, "ChromaticAberration");
                if (!effectComponent)
                {
                    effectComponent = OverhaulImageEffect.AddEffect(camera, s_Material, s_EnableFunction);
                    effectComponent.effectName = "ChromaticAberration";
                }
            }
        }
    }
}
