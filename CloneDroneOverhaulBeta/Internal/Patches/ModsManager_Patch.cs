/*using HarmonyLib;
using InternalModBot;
using ModLibrary;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ModsManager))]
    internal class ModsManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("loadMod")]
        private static void loadMod_Postfix(ModInfo modInfo, ModLoadError error)
        {
            if (modInfo.UniqueID == OverhaulMod.MOD_ID)
            {
                OverhaulCore.isShuttingDownBolt = false;
                foreach (OverhaulController overhaulController in OverhaulController.allControllers)
                {
                    OverhaulDebug.Log("Calling OnSceneReloaded - " + overhaulController.GetType().ToString(), EDebugType.Initialize);
                    overhaulController.OnSceneReloaded();
                }
            }
        }
    }
}*/
