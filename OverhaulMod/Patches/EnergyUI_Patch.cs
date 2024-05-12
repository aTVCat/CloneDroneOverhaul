using HarmonyLib;
using OverhaulMod.Patches.Addons;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(EnergyUI))]
    internal static class EnergyUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix()
        {
            EnergyUIPatchBehaviour energyUIPatch = GamePatchBehaviour.GetBehaviour<EnergyUIPatchBehaviour>();
            if (energyUIPatch)
                energyUIPatch.PatchEnergyUI(false);
        }
    }
}
