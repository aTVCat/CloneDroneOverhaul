using HarmonyLib;
using OverhaulMod.Patches.Behaviours;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(EnergyUI))]
    internal static class EnergyUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnergyUI.Show))]
        private static void Show_Postfix()
        {
            ModActionUtils.DoInFrame(delegate
            {
                EnergyBarPatchBehaviour energyUIPatch = GamePatchBehaviour.GetBehaviour<EnergyBarPatchBehaviour>();
                if (energyUIPatch)
                {
                    energyUIPatch.PatchEnergyUI();
                }
            });
        }
    }
}
