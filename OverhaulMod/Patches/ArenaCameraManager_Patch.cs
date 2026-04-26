using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ArenaCameraManager))]
    internal static class ArenaCameraManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ArenaCameraManager.SetTitleScreenLogoVisible))]
        private static void SetTitleScreenLogoVisible_Postfix(ArenaCameraManager __instance, bool visible)
        {
            if (!ModCore.IsActive()) return;

            __instance.TitleScreenLogoCamera.enabled = visible;
        }
    }
}
