using Bolt;
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
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach(OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnPreCommandExecute((FPMoveCommand)command);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("ExecuteCommand")]
        private static void ExecuteCommand_Postfix(FirstPersonMover __instance, Command command, bool resetState)
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach (OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnPostCommandExecute((FPMoveCommand)command);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FirstPersonMover),"OnEvent", new System.Type[] { typeof(SendFallingEvent) })]
        private static void OnEvent_Postfix(FirstPersonMover __instance, SendFallingEvent fallingEvent)
        {
            if (!OverhaulMod.IsCoreCreated)
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
