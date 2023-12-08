using CDOverhaul.Gameplay;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(CharacterTracker))]
    internal static class CharacterTracker_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetPlayer")]
        private static void SetPlayer_Postfix(Character player)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            if (player is FirstPersonMover)
                OverhaulEvents.DispatchEvent(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, player as FirstPersonMover);

            OverhaulEvents.DispatchEvent(OverhaulGameplayCoreController.PlayerSetAsCharacter, player);
        }
    }
}
