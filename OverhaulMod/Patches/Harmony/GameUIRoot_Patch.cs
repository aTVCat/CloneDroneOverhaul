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
            if (manager && manager.ShouldEnableCursor())
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
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_UPDATES_WINDOW))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_NEWS_PANEL))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_EXCLUSIVE_CONTENT_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_EXCLUSIVE_CONTENT_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_CHAPTER_LEVEL_SELECT_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_CHAPTER_SELECT_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_PAUSE_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_COMMUNITY_HUB))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_FEEDBACK_UI))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_WORKSHOP_BROWSER))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ADVANCEMENTS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ENDLESS_MODE_LEADERBOARD))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ENDLESS_MODE))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_SETTINGS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_OTHER_MODS))
                    return false;
            }
            return true;
        }
    }
}
