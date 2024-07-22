using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(UpgradeUI))]
    internal static class UpgradeUI_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UpgradeUI.OnResetButtonClicked))]
        private static bool OnResetButtonClicked_Prefix(UpgradeUI __instance)
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            if (autoBuildManager && autoBuildManager.isInAutoBuildConfigurationMode)
            {
                autoBuildManager.ResetUpgrades();
                return false;
            }
            return true;
        }

        /*
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix(UpgradeUI __instance)
        {
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            if (autoBuildManager)
            {
                Transform transform = __instance.transform.FindChildRecursive("PreviousPageButton(Clone)");
                if (!transform)
                    transform = __instance.transform.FindChildRecursive("PreviousPageButton");

                Transform transform2 = __instance.transform.FindChildRecursive("NextPageButton(Clone)");
                if (!transform2)
                    transform2 = __instance.transform.FindChildRecursive("NextPageButton");

                if (transform)
                    transform.gameObject.SetActive(!autoBuildManager.isInAutoBuildConfigurationMode);

                if (transform2)
                    transform2.gameObject.SetActive(!autoBuildManager.isInAutoBuildConfigurationMode);
            }
        }*/
    }
}
