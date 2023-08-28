using CDOverhaul.Gameplay;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(CharacterTracker))]
    internal static class CharacterTracker_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetPlayer")]
        private static void SetPlayer_Prefix(CharacterTracker __instance, Character player)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            if (__instance._player == player)
                return;

            OverhaulEventsController.DispatchEvent(OverhaulGameplayManager.PLAYER_SET_EVENT, player);
        }
    }
}
