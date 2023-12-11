using HarmonyLib;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(GameUIRoot))]
    internal static class GameUIRoot_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RefreshCursorEnabled")]
        private static bool RefreshCursorEnabled_Prefix()
        {
            if (ModDebug.forceDisableCursor)
            {
                InputManager.Instance.SetCursorEnabled(false);
                return false;
            }

            ModUIManager manager = ModUIManager.Instance;
            if(manager && manager.ShouldEnableCursor())
            {
                InputManager.Instance.SetCursorEnabled(true);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CloseCurrentMenu")]
        private static bool CloseCurrentMenu_Prefix(bool uiCancelDown, bool pauseDown, bool force = false)
        {
            bool flag = false;
            GameObject nativeControllerSelectTarget = Singleton<InputManager>.Instance.GetNativeControllerSelectTarget();
            if (nativeControllerSelectTarget != null)
            {
                Dropdown component = nativeControllerSelectTarget.GetComponent<Dropdown>();
                ControllerDropdownItem component2 = nativeControllerSelectTarget.GetComponent<ControllerDropdownItem>();
                if (component != null)
                {
                    if (component.transform.Find("Dropdown List") != null)
                        flag = true;
                }
                else if (component2 != null)
                {
                    component2.CloseParentDropdown();
                    flag = true;
                }
            }
            if (!uiCancelDown || !flag)
            {
                ModUIManager modUIManager = ModUIManager.Instance;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_PAUSE_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_COMMUNITY_HUB))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_FEEDBACK_UI))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_WORKSHOP_BROWSER))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_ADVANCEMENTS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_ENDLESS_MODE_LEADERBOARD))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_ENDLESS_MODE))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_SETTINGS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, UIConstants.UI_OTHER_MODS))
                    return false;
            }
            return true;
        }
    }
}
