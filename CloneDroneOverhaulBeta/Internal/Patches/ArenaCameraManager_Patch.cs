using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ArenaCameraManager))]
    internal static class ArenaCameraManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("updateLogoCameraRect")]
        private static void updateLogoCameraRect_Postfix(ArenaCameraManager __instance)
        {
            if (!OverhaulFeaturesSystem.IsFeatureImplemented(EBuildFeatures.TitleScreen_Overhaul))
                return;

            Camera camera = __instance.TitleScreenLogoCamera;
            if (TitleScreenCustomizationSystem.UIAlignment == 1)
            {
                camera.rect = new Rect(0f, camera.rect.y, 1920f, camera.rect.height);
            }
            else
            {
                camera.pixelRect = new Rect(0f, 175f, 600f, 835f);
            }
        }
    }
}
