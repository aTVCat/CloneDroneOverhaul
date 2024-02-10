using HarmonyLib;
using OverhaulMod.Engine;

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

            CameraManager.Instance.AddControllers(firstPersonMover._playerCamera, firstPersonMover);
        }
    }
}
