using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditor : OverhaulUIBehaviour
    {
        private List<UIElementPersonalizationEditorDropdown.OptionData> s_fileOptions, s_viewOptions, s_windowOptions, s_helpOptions, s_screenshotOptions;

        [UIElementAction(nameof(OnSelectItemButtonClicked))]
        [UIElement("SelectItemButton")]
        private readonly Button _selectItemButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElementAction(nameof(OnSendToVerificationButtonClicked))]
        [UIElement("SendToVerificationButton")]
        private readonly Button _sendToVerificationButton;

        [UIElementAction(nameof(OnPlaytestButtonClicked))]
        [UIElement("PlaytestButton")]
        private readonly Button _playtestButton;

        [UIElement("DeveloperPanel", false)]
        private readonly RectTransform _developerPanel;

        [UIElement("ToolBar")]
        public RectTransform ToolBarTransform;

        [UIElement("ItemConfigWindow", typeof(UIElementPersonalizationEditorItemConfigPanel), false)]
        public readonly UIElementPersonalizationEditorItemConfigPanel ItemConfig;

        [UIElement("RightSide", typeof(UIElementPersonalizationEditorUtilitiesPanel), false)]
        public readonly UIElementPersonalizationEditorUtilitiesPanel Utilities;

        [UIElement("InspectorWindow", typeof(UIElementPersonalizationEditorInspectorPanel), false)]
        public readonly UIElementPersonalizationEditorInspectorPanel Inspector;

        [UIElement("ItemOffsetsWindow", typeof(UIElementPersonalizationEditorItemOffsetsPanel), false)]
        public readonly UIElementPersonalizationEditorItemOffsetsPanel ItemOffsets;

        [UIElement("Dropdown", typeof(UIElementPersonalizationEditorDropdown), false)]
        public readonly UIElementPersonalizationEditorDropdown Dropdown;

        [UIElement("Notification", typeof(UIElementPersonalizationEditorNotification), false)]
        public readonly UIElementPersonalizationEditorNotification Notification;

        [UIElement("GuideWindow", typeof(UIElementPersonalizationEditorGuideWindow), false)]
        public readonly UIElementPersonalizationEditorGuideWindow GuideWindow;

        [UIElementAction(nameof(OnFileButtonClicked))]
        [UIElement("FileButton")]
        private readonly Button _toolbarFileButton;

        [UIElementAction(nameof(OnWindowButtonClicked))]
        [UIElement("WindowButton")]
        private readonly Button _toolbarWindowButton;

        [UIElementAction(nameof(OnHelpButtonClicked))]
        [UIElement("HelpButton")]
        private readonly Button _toolbarHelpButton;

        [UIElementAction(nameof(OnUploadButtonClicked))]
        [UIElement("UploadButton")]
        private readonly Button _toolbarUploadButton;

        [UIElementAction(nameof(OnScreenshotButtonClicked))]
        [UIElement("ScreenshotButton")]
        private readonly Button _toolbarScreenshotButton;

        public string ItemConfigWindowID, DeveloperWindowID, InspectorWindowID, ItemOffsetsWindowID;

        public override bool EnableCursor => true;

        public override bool CloseOnEscapeButtonPress => false;

        public static UIPersonalizationEditor Instance
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            Instance = this;
            initializeOptions();

            _toolbarScreenshotButton.gameObject.SetActive(PersonalizationEditorManager.Instance.CanVerifyItems);
            _toolbarWindowButton.interactable = false;
            _toolbarUploadButton.interactable = false;
            _saveButton.interactable = false;

            if (PersonalizationEditorGuideManager.NeverShowIntroductionGuide) return;

            DelegateScheduler.Instance.Schedule(delegate
            {
                Guide();
            }, 3f);
        }

        public override void Update()
        {
            if (InputManager.Instance.GetKeyMode() != KeyMode.EditingInputField)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
                {
                    OnSaveButtonClicked();
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void initializeOptions()
        {
            s_fileOptions = new List<UIElementPersonalizationEditorDropdown.OptionData>()
            {
                new UIElementPersonalizationEditorDropdown.OptionData("Open", "Redirect-16x16", Instance.OnSelectItemButtonClicked),
                new UIElementPersonalizationEditorDropdown.OptionData("Import items", "Import-16x16", Instance.OnImportItemsButtonClicked)
                {
                    DisplayedForVerifiers = true
                },
                new UIElementPersonalizationEditorDropdown.OptionData("Export items", "Export-16x16", Instance.OnExportItemsButtonClicked)
                {
                    DisplayedForVerifiers = true
                },
                new UIElementPersonalizationEditorDropdown.OptionData(true),
                new UIElementPersonalizationEditorDropdown.OptionData("Exit", "Exit-V2-16x16", Instance.OnExitButtonClicked),
            };

            s_viewOptions = new List<UIElementPersonalizationEditorDropdown.OptionData>()
            {
                new UIElementPersonalizationEditorDropdown.OptionData("Welcome message", "Redirect-16x16", PersonalizationEditorManager.Instance.WelcomeMessage),
            };

            s_windowOptions = new List<UIElementPersonalizationEditorDropdown.OptionData>()
            {
                new UIElementPersonalizationEditorDropdown.OptionData("Show item info editor", "Redirect-16x16", Instance.ShowItemConfig),
                new UIElementPersonalizationEditorDropdown.OptionData("Show object editor", "Redirect-16x16", Instance.ShowInspector),
                new UIElementPersonalizationEditorDropdown.OptionData(true),
                new UIElementPersonalizationEditorDropdown.OptionData("Show item moderator", "Redirect-16x16", Instance.ShowItemModerator),
            };

            s_helpOptions = new List<UIElementPersonalizationEditorDropdown.OptionData>
            {
                new UIElementPersonalizationEditorDropdown.OptionData("Welcome message", "Redirect-16x16", PersonalizationEditorManager.Instance.WelcomeMessage),
                new UIElementPersonalizationEditorDropdown.OptionData("Guide: Introduction", "Redirect-16x16", Instance.DropdownGuide),
                new UIElementPersonalizationEditorDropdown.OptionData("Tutorial video", "Redirect-16x16", Instance.TutorialVideo),
            };

            s_screenshotOptions = new List<UIElementPersonalizationEditorDropdown.OptionData>
            {
                new UIElementPersonalizationEditorDropdown.OptionData("Enter screenshot mode", "Exit-V2-16x16", Instance.EnterScreenshotMode),
            };
        }

        public void DropdownGuide()
        {
            Dropdown.Hide();
            Guide();
        }

        public void Guide()
        {
            GuideWindow.StartGuide(PersonalizationEditorGuideManager.Instance.GetGuide("getting_started"));
        }

        public void ShowNotification(string header, string text, Color baseColor, float duration = 7f)
        {
            Notification.ShowNotification(header, text, baseColor, duration);
        }

        public void ShowErrorNotification(string header, string text, float duration = 7f)
        {
            Notification.ShowNotification(header, text, UIElementPersonalizationEditorNotification.ErrorColor, duration);
        }

        public void ShowSaveErrorMessage(string message)
        {
            ShowErrorNotification("Could not save the item", message, 15f);
        }

        public void ShowEverything()
        {
            _toolbarWindowButton.interactable = true;
            _toolbarUploadButton.interactable = true;
            _saveButton.interactable = true;
            Utilities.Show();
            ShowItemConfig();
            ShowInspector();
            ShowItemModerator();
        }

        public void ShowItemConfig()
        {
            ModUIManager.WindowManager windowManager = ModUIManager.Instance.Windows;
            if (ItemConfigWindowID == null)
                ItemConfigWindowID = windowManager.Window(base.transform, ItemConfig.transform, "Edit item info", Vector2.one * -1f, (Vector2.right * -250f) + (Vector2.up * 220f));
            else
                windowManager.ShowWindow(ItemConfigWindowID);

            ModUIManager.WindowBehaviour windowBehaviour = windowManager.GetWindow(ItemConfigWindowID);
            windowBehaviour.transform.localScale = Vector3.one * 0.85f;
        }

        public void ShowInspector()
        {
            ModUIManager.WindowManager windowManager = ModUIManager.Instance.Windows;
            if (InspectorWindowID == null)
                InspectorWindowID = windowManager.Window(base.transform, Inspector.transform, "Edit object", Vector2.one * -1f, (Vector2.right * 250f) + (Vector2.up * 220f));
            else
                windowManager.ShowWindow(InspectorWindowID);

            ModUIManager.WindowBehaviour windowBehaviour = windowManager.GetWindow(InspectorWindowID);
            windowBehaviour.transform.localScale = Vector3.one * 0.85f;
        }

        public void ShowItemModerator()
        {
            TryShowItemModerator(false);
        }

        public void TryShowItemModerator(bool withMessage)
        {
            if (PersonalizationEditorManager.Instance.CanVerifyItems)
            {
                ModUIManager.WindowManager windowManager = ModUIManager.Instance.Windows;
                if (DeveloperWindowID == null)
                    DeveloperWindowID = windowManager.Window(base.transform, _developerPanel, "Item moderator", Vector2.one * -1f, Vector2.up * -120f);
                else
                    windowManager.ShowWindow(DeveloperWindowID);

                ModUIManager.WindowBehaviour windowBehaviour = windowManager.GetWindow(DeveloperWindowID);
                windowBehaviour.transform.localScale = Vector3.one * 0.85f;
            }
            else if (withMessage)
            {
                ModUIUtils.MessagePopupOK("Item moderator is not available", "This panel is made for developers.", 150f, true);
            }
        }

        public void ShowItemOffsets()
        {
            ModUIManager.WindowManager windowManager = ModUIManager.Instance.Windows;
            if (ItemOffsetsWindowID == null)
                ItemOffsetsWindowID = windowManager.Window(base.transform, ItemOffsets.transform, "Configure offsets", Vector2.one * -1f, Vector2.zero);
            else
                windowManager.ShowWindow(ItemOffsetsWindowID);

            ModUIManager.WindowBehaviour windowBehaviour = windowManager.GetWindow(ItemOffsetsWindowID);
            windowBehaviour.transform.localScale = Vector3.one * 0.85f;
        }

        public void OnExitButtonClicked()
        {
            Dropdown.Hide();
            ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, SceneTransitionManager.Instance.DisconnectAndExitToMainMenu, null);
        }

        public void OnSelectItemButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIConstants.ShowPersonalizationEditorItemsBrowser(base.transform);
        }

        public void OnSaveButtonClicked()
        {
            Dropdown.Hide();
            PersonalizationItemSaveResult saveResult = PersonalizationEditorManager.Instance.SaveItem();
            if (saveResult.HasFailed())
                ShowSaveErrorMessage(saveResult.Error);
            else
                ShowNotification("Success", $"Saved the item ({PersonalizationEditorManager.Instance.EditingItemInfo.Name})", UIElementPersonalizationEditorNotification.SuccessColor);
        }

        public void OnExportItemsButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIConstants.ShowPersonalizationEditorExportAllMenu(base.transform);
        }

        public void OnImportItemsButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIConstants.ShowPersonalizationEditorItemImportHelper(base.transform);
        }

        public void OnPlaytestButtonClicked()
        {
            Utilities.SetAnimationToggleOn();
            PersonalizationEditorManager.Instance.EnterPlaytestMode();
        }

        public void OnSendToVerificationButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIConstants.ShowPersonalizationEditorVerificationMenu(base.transform);
        }

        public void OnUploadButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIConstants.ShowPersonalizationEditorVerificationMenu(base.transform);
        }

        public void OnAboutButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIConstants.ShowPersonalizationEditorAboutDialog(base.transform);
        }

        public void OnFileButtonClicked()
        {
            Dropdown.ShowWithOptions(s_fileOptions, _toolbarFileButton.transform as RectTransform);
        }

        public void OnWindowButtonClicked()
        {
            Dropdown.ShowWithOptions(s_windowOptions, _toolbarWindowButton.transform as RectTransform);
        }

        public void OnHelpButtonClicked()
        {
            Dropdown.ShowWithOptions(s_helpOptions, _toolbarHelpButton.transform as RectTransform);
        }

        public void OnScreenshotButtonClicked()
        {
            EnterScreenshotMode();
            //Dropdown.ShowWithOptions(s_screenshotOptions, _toolbarScreenshotButton.transform as RectTransform);
        }

        public void TutorialVideo()
        {
            Dropdown.Hide();
            Application.OpenURL("https://youtu.be/xdbdb-WizSo");
        }

        public void EnterScreenshotMode()
        {
            Dropdown.Hide();
            PersonalizationEditorManager.Instance.EnterScreenshotMode();
        }
    }
}
