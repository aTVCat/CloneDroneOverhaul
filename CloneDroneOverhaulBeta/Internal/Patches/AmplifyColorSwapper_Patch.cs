using CDOverhaul.Graphics;
using HarmonyLib;
using ModLibrary;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AmplifyColorSwapper))]
    internal static class AmplifyColorSwapper_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("refreshLutValues")]
        private static void refreshLutValues_Postfix(AmplifyColorSwapper __instance)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            AmplifyColorBase amplifyColor = __instance.GetPrivateField<AmplifyColorBase>("_amplifyColorBase");
            OverhaulGraphicsController.PatchAmplifyColorMode(amplifyColor);
        }
    }
}
