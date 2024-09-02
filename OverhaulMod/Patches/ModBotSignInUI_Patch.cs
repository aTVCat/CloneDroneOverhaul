using HarmonyLib;
using InternalModBot;
using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ModBotSignInUI))]
    internal static class ModBotSignInUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ModBotSignInUI.onSignedIn))]
        private static void onSignedIn_Postfix(WorldAudioSource __instance)
        {
            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Get<UISettingsMenuRework>(AssetBundleConstants.UI, ModUIConstants.UI_SETTINGS_MENU);
            if (settingsMenuRework && settingsMenuRework.visible && settingsMenuRework.GetSelectedTabID() == "Advanced")
                settingsMenuRework.PopulatePage("Advanced");
        }
    }
}
