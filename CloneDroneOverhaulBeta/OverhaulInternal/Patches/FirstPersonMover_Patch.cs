using Bolt;
using CDOverhaul.Gameplay;
using HarmonyLib;
using System.Collections.Generic;

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

            List<FirstPersonMoverExtention> list = FirstPersonMoverExtention.GetExtentions(__instance);
            if (list.IsNullOrEmpty())
            {
                return;
            }

            foreach (FirstPersonMoverExtention ext in list)
            {
                ext.OnExecuteCommand((FPMoveCommand)command, resetState);
            }
        }
    }
}
