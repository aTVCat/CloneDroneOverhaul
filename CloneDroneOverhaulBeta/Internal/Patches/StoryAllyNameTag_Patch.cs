using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(StoryAllyNameTag))]
    internal static class StoryAllyNameTag_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Initialize")]
        private static void Initialize_Prefix(Character character, ref string name, Color nameColor)
        {
            if (!OverhaulMod.IsModInitialized || OverhaulLocalizationManager.Error)
                return;

            if (name == "Ally")
                name = OverhaulLocalizationManager.GetTranslation("Ally");
        }
    }
}
