using HarmonyLib;
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
        private static void onSignedIn_Postfix(WorldAudioSource __instance)
        {
            if (!ModCore.IsActive()) return;

            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Get<UISettingsMenuRework>(ModAssetBundles.UI, ModUIs.UI_SETTINGS_MENU_REWORK);
            if (settingsMenuRework && settingsMenuRework.IsVisible && settingsMenuRework.GetSelectedTabID() == "Advanced")
                settingsMenuRework.PopulatePage("Advanced");
        }
    }
}
