using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal static class Character_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("createPlayerCamera")]
        private static void createPlayerCamera_Postfix(Character __instance)
        {
            if (!(__instance is FirstPersonMover firstPersonMover))
                return;

            CameraModeManager.Instance.AddController(firstPersonMover._playerCamera, firstPersonMover);
        }
    }
}
