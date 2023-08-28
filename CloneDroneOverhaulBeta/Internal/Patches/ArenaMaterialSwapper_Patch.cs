using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ArenaMaterialSwapper))]
    internal static class ArenaMaterialSwapper_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("onArenaSettingsRefreshed")]
        private static bool onArenaSettingsRefreshed_Prefix(AmplifyColorSwapper __instance)
        {
            return !OverhaulMod.IsModInitialized;
        }
    }
}
