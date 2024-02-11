using OverhaulMod.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModUIUtils
    {
        public static void KeyBinder(string name, KeyCode defaultKey, Action<KeyCode> callback, Transform parent)
        {
            UISetKeyBindWindow setKeyBindWindow = ModUIConstants.ShowSetKeyBindWindow(parent);
            setKeyBindWindow.callBack = callback;
            setKeyBindWindow.SetContents(name, defaultKey);
        }

        public static void LevelDescriptionBrowser(List<LevelDescription> levelDescriptions, Action<List<LevelDescription>> callback, bool allowMultiSelection)
        {
            UILevelDescriptionBrowser levelDescriptionBrowser = ModUIConstants.ShowLevelDescriptionBrowser();
            levelDescriptionBrowser.allowMultiSelection = allowMultiSelection;
            levelDescriptionBrowser.callback = callback;
            levelDescriptionBrowser.Populate(levelDescriptions);
        }

        public static void InputFieldWindow(string header, string description, float height = 125f, Action<string> doneAction = null)
        {
            UIGenericInputFieldWindow genericInputFieldWindow = ModUIConstants.ShowGenericInputFieldWindow();
            genericInputFieldWindow.SetTexts(header, description);
            genericInputFieldWindow.SetHeight(height);
            genericInputFieldWindow.doneAction = doneAction;
        }

        public static void MessagePopup(bool fullScreen, string header, string description, float height = 125f, MessageMenu.ButtonLayout buttonLayout = MessageMenu.ButtonLayout.OkButton, string okText = null, string yesText = null, string noText = null, Action okAction = null, Action yesAction = null, Action noAction = null)
        {
            UIMessagePopup messagePopup = fullScreen ? ModUIConstants.ShowFullScreenMessagePopup() : ModUIConstants.ShowMessagePopup();
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
            MessagePopupOK("This doesn't work now!", "Functionality will be added in later updates...", false);
        }

        public static void ShowVanillaEscMenu()
        {
            UIPauseMenuRework.disableOverhauledVersion = true;
            ModCache.gameUIRoot.EscMenu.Show();
            ModCache.gameUIRoot.RefreshCursorEnabled();
            UIPauseMenuRework.disableOverhauledVersion = false;
        }

        public static bool IsEscKeyDown()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
    }
}
