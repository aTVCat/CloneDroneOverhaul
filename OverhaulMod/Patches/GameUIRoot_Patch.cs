using HarmonyLib;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches
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
            GameObject nativeControllerSelectTarget = InputManager.Instance.GetNativeControllerSelectTarget();
            if (nativeControllerSelectTarget)
            {
                Dropdown dropdownComponent = nativeControllerSelectTarget.GetComponent<Dropdown>();
                ControllerDropdownItem controllerDropdownItem = nativeControllerSelectTarget.GetComponent<ControllerDropdownItem>();
                if (dropdownComponent != null)
                {
                    if (dropdownComponent.transform.Find("Dropdown List"))
                        flag = true;
                }
                else if (controllerDropdownItem)
                {
                    controllerDropdownItem.CloseParentDropdown();
                    flag = true;
                }
            }

            if (!uiCancelDown || !flag)
            {
                ModUIManager modUIManager = ModUIManager.Instance;
                if (!modUIManager || modUIManager.skipHidingCustomUIs)
                    return true;

                if (modUIManager.TryInvokeAction())
                    return false;

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_MESSAGE_POPUP_FULL_SCREEN))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_WORKSHOP_ITEM_PAGE_WINDOW))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_OVERHAUL_MOD_INFO_WINDOW))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_INFORMATION_SELECT_WINDOW))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_NEWS_DETAILS_PANEL))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_UPDATE_INFO_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_NEWS_INFO_EDITOR))
                    return false;
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
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_OVERHAUL_UI_MANAGEMENT_PANEL))
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
