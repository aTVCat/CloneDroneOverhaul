using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ArenaCameraManager))]
    internal static class ArenaCameraManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ArenaCameraManager.ShowTitleScreenCamera))]
        private static void ShowTitleScreenCamera_Postfix(ArenaCameraManager __instance)
        {
            if (!ModCore.IsActive()) return;

            __instance.ArenaCameraTransform.parent.SetParent(WorldRoot.Instance.transform);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ArenaCameraManager.HideTitleScreenCamera))]
        private static void HideTitleScreenCamera_Postfix(ArenaCameraManager __instance)
        {
            __instance.ArenaCameraTransform.parent.SetParent(WorldRoot.Instance.transform.GetChild(1));
            __instance.ArenaCameraTransform.parent.SetSiblingIndex(11);
        }
    }
}