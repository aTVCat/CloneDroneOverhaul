using HarmonyLib;
using InternalModBot;
using ModLibrary;
using OverhaulMod.UI;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ModBotUserIdentifier))]
    internal static class ModBotUserIdentifier_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ModBotUserIdentifier.onSignedIn))]
        private static void onSignedIn_Postfix()
        {
            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Get<UISettingsMenuRework>(AssetBundleConstants.UI, ModUIConstants.UI_SETTINGS_MENU);
            if (settingsMenuRework && settingsMenuRework.IsVisible && settingsMenuRework.GetSelectedTabID() == "Advanced")
                settingsMenuRework.PopulatePage("Advanced");
        }
    }
}
