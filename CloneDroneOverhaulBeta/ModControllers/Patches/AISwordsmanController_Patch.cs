using CDOverhaul.Gameplay;
using HarmonyLib;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AISwordsmanController))]
    internal static class AISwordsmanController_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(AISwordsmanController __instance)
        {
            List<FirstPersonMoverExtention> list = FirstPersonMoverExtention.GetExtentions(__instance.GetComponent<FirstPersonMover>());
            if (list == null)
            {
                return;
            }

            foreach (FirstPersonMoverExtention ext in list)
            {
                if (ext != null)
                {
                    ext.OnAIUpdate(__instance);
                }
            }
        }
    }
}
