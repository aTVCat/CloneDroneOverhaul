using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(TitleScreenMessagePanel))]
    internal static class TitleScreenMessagePanel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("PopulateUI")]
        private static bool PopulateUI_Prefix(TitleScreenMessagePanel __instance)
        {
            __instance.InnerHolder.gameObject.SetActive(false);
            return false;
        }
    }
}
