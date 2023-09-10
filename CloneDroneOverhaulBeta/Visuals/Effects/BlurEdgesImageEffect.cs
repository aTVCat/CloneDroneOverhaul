using OverhaulAPI.SharedMonoBehaviours;
using System;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class BlurEdgesImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSettingAttribute_Old("Graphics.Shaders.Blur edges", false, false, "I don't really like it, but you may turn this setting on for fun, I guess")]
        public static bool BlurEdgesEnabled;

        private static Material s_Material;
        private static readonly Func<bool> s_EnableFunction = () => BlurEdgesEnabled;

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (!s_Material)
                s_Material = OverhaulAssetLoader.GetAsset<Material>("M_SnapshotTest", OverhaulAssetPart.Part2);

            if (PreviousCamera && PreviousCamera != CurrentCamera)
            {
                OverhaulImageEffect effectComponent = GetImageEffect(PreviousCamera, "BlurEdges");
                if (effectComponent)
                {
                    Destroy(effectComponent);
                }
            }
            if (CurrentCamera)
            {
                OverhaulImageEffect effectComponent = GetImageEffect(camera, "BlurEdges");
                if (!effectComponent)
                {
                    effectComponent = OverhaulImageEffect.AddEffect(camera, s_Material, s_EnableFunction);
                    effectComponent.effectName = "BlurEdges";
                }
            }
        }
    }
}
