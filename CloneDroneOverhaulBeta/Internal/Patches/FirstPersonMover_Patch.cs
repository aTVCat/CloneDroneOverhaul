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

            FirstPersonMoverExpansionBase[] expansionBases = __instance.GetComponents<FirstPersonMoverExpansionBase>();
            foreach(FirstPersonMoverExpansionBase b in expansionBases)
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

            FirstPersonMoverExpansionBase[] expansionBases = __instance.GetComponents<FirstPersonMoverExpansionBase>();
            foreach (FirstPersonMoverExpansionBase b in expansionBases)
            {
                b.OnPostCommandExecute((FPMoveCommand)command);
            }
        }
    }
}
