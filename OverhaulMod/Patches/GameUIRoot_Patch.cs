using HarmonyLib;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.UI;
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
            if (PersonalizationEditorCamera.IsControllingTheCamera)
                return false;

#if DEBUG
            if (ModDebug.forceDisableCursor)
            {
                InputManager.Instance.SetCursorEnabled(false);
                return false;
            }
#endif
            ModUIManager manager = ModUIManager.Instance;
            if (!manager)
                return true;

            if (manager.ShouldEnableCursor() || (manager.IsUIVisible(AssetBundleConstants.UI, ModUIConstants.UI_PHOTO_MODE_UI_REWORK) && AdvancedPhotoModeManager.Instance.IsInPhotoMode() && UIManager.Instance.IsMouseOverUIElement()))
            {
                Debug.Log("Overhaul mod enabled the cursor");
                InputManager.Instance.SetCursorEnabled(true);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CloseCurrentMenu")]
        private static bool CloseCurrentMenu_Prefix(GameUIRoot __instance, bool uiCancelDown, bool pauseDown, bool force = false)
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

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_GENERIC_IMAGE_VIEWER))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_IMAGE_EXPLORER))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_SET_KEY_BIND_WINDOW))
                    return false;

                UIAutoBuildMenu autoBuildMenu = modUIManager.Get<UIAutoBuildMenu>(AssetBundleConstants.UI, ModUIConstants.UI_AUTO_BUILD_MENU);
                if (autoBuildMenu && autoBuildMenu.visibleInHierarchy)
                {
                    if (autoBuildMenu.isShowingUpgradeUI)
                    {
                        autoBuildMenu.OnCloseUpgradeUIButtonClicked();
                        return false;
                    }
                    autoBuildMenu.Hide();
                    return false;
                }

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_DISCORD_SERVER_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_DEVELOPMENT_GALLERY))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN_CUSTOMIZATION_PANEL))
                    return false;

                UIDownloadPersonalizationAssetsMenu downloadPersonalizationAssetsMenu = modUIManager.Get<UIDownloadPersonalizationAssetsMenu>(AssetBundleConstants.UI, ModUIConstants.UI_DOWNLOAD_PERSONALIZATION_ASSETS_MENU);
                if (downloadPersonalizationAssetsMenu && downloadPersonalizationAssetsMenu.visibleInHierarchy)
                {
                    if (!downloadPersonalizationAssetsMenu.CanExit())
                        return false;

                    downloadPersonalizationAssetsMenu.Hide();
                    return false;
                }

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_DOWNLOAD_PERSONALIZATION_ASSETS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_PERSONALIZATION_ITEMS_BROWSER))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_PERSONALIZATION_EDITOR_ITEMS_BROWSER))
                    return false;

                UIPersonalizationEditorVerificationMenu personalizationEditorVerificationMenu = modUIManager.Get<UIPersonalizationEditorVerificationMenu>(AssetBundleConstants.UI, ModUIConstants.UI_PERSONALIZATION_EDITOR_VERIFICATION_MENU);
                if (personalizationEditorVerificationMenu && personalizationEditorVerificationMenu.visibleInHierarchy)
                {
                    if (!personalizationEditorVerificationMenu.CanExit())
                        return false;

                    downloadPersonalizationAssetsMenu.Hide();
                    return false;
                }

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_MESSAGE_POPUP_FULL_SCREEN))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_MULTIPLAYER_GAMEMODE_SELECT_SCREEN))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_LEVEL_DESCRIPTION_LIST_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_WORKSHOP_ITEM_PAGE_WINDOW))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_CREDITS_MENU))
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
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_LOCALIZATION_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ADDONS_DOWNLOAD_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ADDONS_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ADDONS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_NEWS_PANEL))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_EXCLUSIVE_CONTENT_EDITOR))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_EXCLUSIVE_CONTENT_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ENDLESS_MODE_LEADERBOARD))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_CHALLENGES_MENU_REWORK))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_DUEL_INVITE_MENU_REWORK))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_CHAPTER_LEVEL_SELECT_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_CHAPTER_SELECT_MENU))
                    return false;

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_OVERHAUL_UI_MANAGEMENT_PANEL))
                    return false;

                UISettingsMenuRework settingsMenuRework = modUIManager.Get<UISettingsMenuRework>(AssetBundleConstants.UI, ModUIConstants.UI_SETTINGS_MENU);
                if (settingsMenuRework && settingsMenuRework.visibleInHierarchy)
                {
                    EmoteSettingsUI menu = __instance.EmoteSettingsUI;
                    if (menu.EmoteSelectionUI.activeInHierarchy)
                    {
                        menu.HideEmoteLibraryUI();
                        return false;
                    }

                    if (menu.gameObject.activeInHierarchy)
                    {
                        menu.Hide();
                        return false;
                    }

                    if (!settingsMenuRework.disallowUsingKey)
                        settingsMenuRework.Hide();

                    return false;
                }

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ADVANCEMENTS_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_FEEDBACK_UI))
                    return false;

                SettingsMenu settingsMenu = ModCache.gameUIRoot?.SettingsMenu;
                if (settingsMenu && settingsMenu.gameObject.activeInHierarchy)
                {
                    settingsMenu.Hide();
                    return false;
                }

                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_PAUSE_MENU))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_COMMUNITY_HUB))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_WORKSHOP_BROWSER))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_ENDLESS_MODE))
                    return false;
                if (modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_OTHER_MODS))
                    return false;
            }
            return true;
        }
    }
}
