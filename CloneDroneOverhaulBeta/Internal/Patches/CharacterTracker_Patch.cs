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
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            if (player is FirstPersonMover)
            {
                OverhaulEventManager.DispatchEvent<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, player as FirstPersonMover);
            }
            else
            {
                OverhaulEventManager.DispatchEvent<Character>(OverhaulGameplayCoreController.PlayerSetAsCharacter, player);
            }
        }
    }
}
