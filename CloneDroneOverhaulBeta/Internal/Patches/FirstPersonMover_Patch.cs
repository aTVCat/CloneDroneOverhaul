using Bolt;
using CDOverhaul.Gameplay;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    internal static class FirstPersonMover_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ExecuteCommand")]
        private static void ExecuteCommand_Prefix(FirstPersonMover __instance, Command command, bool resetState)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach (OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnPreCommandExecute((FPMoveCommand)command);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("ExecuteCommand")]
        private static void ExecuteCommand_Postfix(FirstPersonMover __instance, Command command, bool resetState)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach (OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnPostCommandExecute((FPMoveCommand)command);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateArrowAndDrawBow")]
        private static void CreateArrowAndDrawBow_Postfix(FirstPersonMover __instance)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            WeaponSkinsWearer w = __instance.GetComponent<WeaponSkinsWearer>();
            if (w == null)
            {
                return;
            }

            WeaponSkinBehaviour s = w.GetSpecialBehaviourInEquippedWeapon<WeaponSkinBehaviour>();
            if (s == null)
            {
                return;
            }
            s.OnBeginDraw();
        }

        [HarmonyPostfix]
        [HarmonyPatch("ReleaseNockedArrow")]
        private static void ReleaseNockedArrow_Postfix(FirstPersonMover __instance)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            WeaponSkinsWearer w = __instance.GetComponent<WeaponSkinsWearer>();
            if (w == null)
            {
                return;
            }

            WeaponSkinBehaviour s = w.GetSpecialBehaviourInEquippedWeapon<WeaponSkinBehaviour>();
            if (s == null)
            {
                return;
            }
            s.OnEndDraw();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FirstPersonMover), "OnEvent", new System.Type[] { typeof(SendFallingEvent) })]
        private static void OnEvent_Postfix(FirstPersonMover __instance, SendFallingEvent fallingEvent)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach (OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnEvent(fallingEvent);
            }
        }
    }
}
