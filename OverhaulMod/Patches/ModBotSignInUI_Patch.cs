using HarmonyLib;
using InternalModBot;
using OverhaulMod.UI;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ModBotSignInUI))]
    internal static class ModBotSignInUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ModBotSignInUI.onSignedIn))]
        private static void onSignedIn_Postfix(WorldAudioSource __instance)
        {
            if (!ModCore.IsActive()) return;

            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Get<UISettingsMenuRework>(ModAssetBundles.UI, ModUIs.UI_SETTINGS_MENU_REWORK);
            if (settingsMenuRework && settingsMenuRework.IsVisible && settingsMenuRework.GetSelectedTabID() == "Advanced")
                settingsMenuRework.PopulatePage("Advanced");
        }
    }
}
