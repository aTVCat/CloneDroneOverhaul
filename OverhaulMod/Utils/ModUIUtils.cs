using OverhaulMod.Content;
using OverhaulMod.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Utils
{
    public static class ModUIUtils
    {
        public static void ShowNewUpdateMessageOrChangelog(float delay, bool showNewUpdateMessage = true)
        {
            bool showChangelog;
            string lastBuildChangelogWasShownOn = UIPatchNotes.LastBuildChangelogWasShownOn;
            if (lastBuildChangelogWasShownOn.IsNullOrEmpty())
                showChangelog = true;
            else if (Version.TryParse(lastBuildChangelogWasShownOn, out Version prevVersion) && ModBuild.Version > prevVersion)
                showChangelog = true;
            else
                showChangelog = false;

            if (showChangelog)
            {
                ModActionUtils.DoInTime(delegate
                {
                    _ = ModUIs.ShowPatchNotes(new UIPatchNotes.ShowArguments()
                    {
                        CloseButtonActive = true,
                        PanelOffset = Vector2.zero,
                        ShrinkPanel = false,
                        HideVersionList = false,
                    });
                }, delay);
            }
            else if (showNewUpdateMessage && !UpdateManager.HasNotifiedAboutNewBuild && UpdateManager.CheckForUpdatesOnStartup)
            {
                ModActionUtils.DoInTime(delegate
                {
                    UpdateManager.HasNotifiedAboutNewBuild = true;

                    string savedVersion = UpdateManager.SavedNewVersion;
                    if (savedVersion.IsNullOrEmpty())
                        return;

                    if (!System.Version.TryParse(savedVersion, out System.Version newVersion))
                        return;

                    if (newVersion > ModBuild.Version && GameModeManager.IsOnTitleScreen())
                    {
                        MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("update_available_header"), string.Format(LocalizationManager.Instance.GetTranslatedString("update_available_description"), newVersion), 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                        {
                            UIUpdatesWindowRework windowNew = ModUIs.ShowUpdatesWindowRework();
                            windowNew.OnCheckForUpdatesButtonClicked();
                        });
                    }
                }, delay);
            }
        }

        public static void ImageExplorer(List<string> imagePaths, bool customLinks, Transform parent)
        {
            UIImageExplorer imageExplorer = ModUIs.ShowImageExplorer(parent);
            imageExplorer.Populate(imagePaths, customLinks);
        }

        public static void FileExplorer(Transform parent, bool selectMode, Action<string> callback, string initialFolder, string searchPattern = null, bool selectFolder = false)
        {
            UIFileExplorer fileExplorer = ModUIs.ShowFileExplorer(parent);

            if (initialFolder.IsNullOrEmpty() || !Directory.Exists(initialFolder))
                fileExplorer.OnDownloadsFolderButtonClicked();
            else
                fileExplorer.currentFolder = initialFolder;

            fileExplorer.searchPattern = searchPattern;
            fileExplorer.singleFileCallback = callback;
            fileExplorer.selectFolder = selectFolder;
            fileExplorer.selectMany = false;
        }

        public static void FileExplorer(Transform parent, bool selectMode, Action<List<string>> callback, string initialFolder, string searchPattern = null, bool selectFolder = false)
        {
            UIFileExplorer fileExplorer = ModUIs.ShowFileExplorer(parent);

            if (initialFolder.IsNullOrEmpty() || !Directory.Exists(initialFolder))
                fileExplorer.OnDownloadsFolderButtonClicked();
            else
                fileExplorer.currentFolder = initialFolder;

            fileExplorer.searchPattern = searchPattern;
            fileExplorer.multipleFilesCallback = callback;
            fileExplorer.selectFolder = selectFolder;
            fileExplorer.selectMany = true;
        }

        public static void Tooltip(string text, float duration = 2f)
        {
            UIScreenTooltips screenTooltips = UIScreenTooltips.Instance ?? ModUIs.ShowScreenTooltips();
            screenTooltips.ShowText(text, duration);
        }

        public static void ImageViewer(Texture2D texture, Transform parent, Action closedCallback = null)
        {
            UIGenericImageViewer genericImageViewer = ModUIs.ShowGenericImageViewer(parent);
            genericImageViewer.Populate(texture, closedCallback);
        }

        public static void ColorPicker(Color currentColor, bool showAlphaChannel, Action<Color> callback, Transform parent)
        {
            UIGenericColorPicker genericColorPicker = ModUIs.ShowGenericColorPicker(parent);
            genericColorPicker.Populate(currentColor, showAlphaChannel, callback);
        }

        public static void KeyBinder(string name, KeyCode defaultKey, Action<KeyCode> callback, Transform parent)
        {
            UISetKeyBindWindow setKeyBindWindow = ModUIs.ShowSetKeyBindWindow(parent);
            setKeyBindWindow.callBack = callback;
            setKeyBindWindow.SetContents(name, defaultKey);
        }

        public static void LevelDescriptionBrowser(List<LevelDescription> levelDescriptions, Action<LevelDescription> callback)
        {
            UILevelDescriptionBrowser levelDescriptionBrowser = ModUIs.ShowLevelDescriptionBrowser();
            levelDescriptionBrowser.callback = callback;
            levelDescriptionBrowser.Populate(levelDescriptions);
        }

        public static void InputFieldWindow(string header, string description, string initialText = null, int limit = 0, float height = 125f, Action<string> doneAction = null)
        {
            if (initialText == null)
                initialText = string.Empty;

            UIGenericInputFieldWindow genericInputFieldWindow = ModUIs.ShowGenericInputFieldWindow();
            genericInputFieldWindow.SetTexts(header, description);
            genericInputFieldWindow.SetHeight(height);
            genericInputFieldWindow.SetInputFieldText(initialText, limit);
            genericInputFieldWindow.doneAction = doneAction;
        }

        public static void MessagePopup(bool fullScreen, string header, string description, float height = 125f, MessageMenu.ButtonLayout buttonLayout = MessageMenu.ButtonLayout.OkButton, string okText = null, string yesText = null, string noText = null, Action okAction = null, Action yesAction = null, Action noAction = null)
        {
            UIMessagePopup messagePopup = fullScreen ? ModUIs.ShowFullScreenMessagePopup() : ModUIs.ShowMessagePopup();
            messagePopup.SetTexts(header, description);
            messagePopup.SetHeight(height);
            messagePopup.SetButtonLayout(buttonLayout);
            messagePopup.SetButtonActions(okAction, yesAction, noAction);
            messagePopup.SetButtonTexts(okText, yesText, noText);
        }

        public static void MessagePopupOK(string header, string description, bool fullScreen = false)
        {
            MessagePopup(fullScreen, header, description, 150f, MessageMenu.ButtonLayout.OkButton, "ok");
        }

        public static void MessagePopupOK(string header, string description, float height, bool fullScreen = false)
        {
            MessagePopup(fullScreen, header, description, height, MessageMenu.ButtonLayout.OkButton, "ok");
        }

        public static void MessagePopupOK(string header, string description, string buttonText, float height, bool fullScreen = false)
        {
            MessagePopup(fullScreen, header, description, height, MessageMenu.ButtonLayout.OkButton, buttonText);
        }

        public static void MessagePopupOK(string header, string description, string buttonText, Action buttonAction, float height, bool fullScreen = false)
        {
            MessagePopup(fullScreen, header, description, height, MessageMenu.ButtonLayout.OkButton, buttonText, null, null, buttonAction);
        }

        public static void MessagePopupNotImplemented()
        {
            MessagePopupOK(LocalizationManager.Instance.GetTranslatedString("wip_header"), LocalizationManager.Instance.GetTranslatedString("wip_text"), false);
        }

        public static void ShowVanillaEscMenu()
        {
            UIPauseMenuRework.disableOverhauledVersion = true;
            ModCache.UIRoot.EscMenu.Show();
            ModCache.UIRoot.RefreshCursorEnabled();
            UIPauseMenuRework.disableOverhauledVersion = false;
        }

        public static void ReplaceBackgroundSprite(Image image, bool addShadow)
        {
            image.sprite = ModResources.Sprite(ModAssetBundles.UI, "FrameBGDark-16x16");
            if (addShadow)
            {
                Shadow shadow = image.GetComponent<Shadow>() ?? image.gameObject.AddComponent<Shadow>();
                shadow.effectDistance = new Vector2(-2f, -2f);
                shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            }
        }
    }
}