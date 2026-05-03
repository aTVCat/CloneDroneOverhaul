using OverhaulMod.Content.Personalization;
using OverhaulMod.UI.Windows;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    /// <summary>
    /// PE = Personalization editor
    /// </summary>
    public class UIPE : OverhaulUIBehaviour
    {
        public const float WINDOWS_SCALE = 0.75f;

        private List<UIElementPEDropdown.OptionData> s_fileOptions, s_windowOptions, s_helpOptions;

        [UIElement("UtilitiesPane", typeof(UIElementPEUtilitiesPane), false)]
        public readonly UIElementPEUtilitiesPane Utilities;

        [UIElement("ModeratorPanel", typeof(UIElementPEModeratorPanel), false)]
        public readonly UIElementPEModeratorPanel ModeratorPanel;

        [UIElement("ItemInformationWindow", typeof(UIElementPEItemInformationPanel), false)]
        public readonly UIElementPEItemInformationPanel ItemInformationPanel;

        [UIElement("ItemAdditionalConfigurationWindow", typeof(UIElementPEItemAdditionalConfigurationPanel), false)]
        public readonly UIElementPEItemAdditionalConfigurationPanel ItemAdditionalConfigurationPanel;

        [UIElement("ImportedFilesWindow", typeof(UIElementPEImportedFilesPanel), false)]
        public readonly UIElementPEImportedFilesPanel ImportedFilesPanel;

        [UIElement("HierarchyWindow", typeof(UIElementPEHierarchyPanel), false)]
        public readonly UIElementPEHierarchyPanel HierarchyPanel;


        [UIElement("InspectorWindow", typeof(UIElementPEInspectorPanel), false)]
        public readonly UIElementPEInspectorPanel Inspector;

        [UIElement("ItemOffsetsWindow", typeof(UIElementPEItemOffsetsPanel), false)]
        public readonly UIElementPEItemOffsetsPanel ItemOffsets;

        [UIElement("Dropdown", typeof(UIElementPEDropdown), false)]
        public readonly UIElementPEDropdown Dropdown;

        [UIElement("Notification", typeof(UIElementPENotification), false)]
        public readonly UIElementPENotification Notification;

        [UIElement("GuideWindow", typeof(UIElementPEGuideWindow), false)]
        public readonly UIElementPEGuideWindow GuideWindow;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

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

        [UIElementAction(nameof(OnPlaytestButtonClicked))]
        [UIElement("PlaytestButton")]
        private readonly Button _playtestButton;

        [UIElement("WindowBoundaries")]
        private readonly RectTransform _windowBoundaries;

        public WindowHandle ItemInformationWindow, ItemAdditionalConfigurationWindow, ImportedFilesWindow, HierarchyWindow;

        public WindowHandle ModeratorWindow, InspectorWindow, ItemOffsetsWindow;

        public override bool EnableCursor => true;

        public override bool CloseOnEscapeButtonPress => false;

        public static UIPE Instance
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

            ModActionUtils.DoInTime(Guide, 3f);
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

        public void SetValuesFromItem()
        {
            ModeratorPanel.Populate();
            ItemInformationPanel.Populate();
            ItemAdditionalConfigurationPanel.Populate();
            ImportedFilesPanel.Populate();
            HierarchyPanel.Populate();
        }

        public void ApplyValuesToItem()
        {
            if (PersonalizationEditorManager.Instance.CanVerifyItems) ModeratorPanel.ApplyValues();
            ItemInformationPanel.ApplyValues();
            ItemAdditionalConfigurationPanel.ApplyValues();
        }

        private void initializeOptions()
        {
            s_fileOptions = new List<UIElementPEDropdown.OptionData>()
            {
                new UIElementPEDropdown.OptionData("Open", "Redirect-16x16", Instance.OnSelectItemButtonClicked),
                new UIElementPEDropdown.OptionData("Import items", "Import-16x16", Instance.OnImportItemsButtonClicked)
                {
                    DisplayedForVerifiers = true
                },
                new UIElementPEDropdown.OptionData("Export items", "Export-16x16", Instance.OnExportItemsButtonClicked)
                {
                    DisplayedForVerifiers = true
                },
                new UIElementPEDropdown.OptionData(true),
                new UIElementPEDropdown.OptionData("Exit", "Exit-V2-16x16", Instance.OnExitButtonClicked),
            };

            s_windowOptions = new List<UIElementPEDropdown.OptionData>()
            {
                new UIElementPEDropdown.OptionData("Item information", "Redirect-16x16", Instance.ShowItemInformation),
                new UIElementPEDropdown.OptionData("Item configuration", "Redirect-16x16", Instance.ShowItemAdditionalConfiguration),
                new UIElementPEDropdown.OptionData("Imported files", "Redirect-16x16", Instance.ShowImportedFiles),
                new UIElementPEDropdown.OptionData("Hierarchy", "Redirect-16x16", Instance.ShowHierarchy),
                new UIElementPEDropdown.OptionData("Show object editor", "Redirect-16x16", Instance.ShowInspector),
                new UIElementPEDropdown.OptionData(true)
                {
                    DisplayedForVerifiers = true
                },
                new UIElementPEDropdown.OptionData("Show item moderator", "Redirect-16x16", Instance.ShowItemModerator)
                {
                    DisplayedForVerifiers = true
                },
            };

            s_helpOptions = new List<UIElementPEDropdown.OptionData>
            {
                new UIElementPEDropdown.OptionData("Welcome message", "Redirect-16x16", PersonalizationEditorManager.Instance.WelcomeMessage),
                new UIElementPEDropdown.OptionData("Guide: Introduction", "Redirect-16x16", Instance.DropdownGuide),
                new UIElementPEDropdown.OptionData("Tutorial video", "Redirect-16x16", Instance.TutorialVideo),
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
            Notification.ShowNotification(header, text, UIElementPENotification.ErrorColor, duration);
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

            ShowItemModerator();
            ShowItemInformation();
            ShowItemAdditionalConfiguration();
            ShowImportedFiles();
            ShowHierarchy();

            ShowInspector();
        }

        public void ShowItemModerator()
        {
            if (!PersonalizationEditorManager.Instance.CanVerifyItems) return;

            if (ModeratorWindow.IsInvalid())
            {
                ModeratorWindow = WindowManager.Instance.NewWindow(_windowBoundaries, ModeratorPanel.transform as RectTransform, "Moderation", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect(_windowBoundaries.rect.width - 45f - (Inspector.transform as RectTransform).rect.width, 25f + (ModeratorPanel.transform as RectTransform).rect.height, 0f, 0f),
                    Scale = WINDOWS_SCALE,
                    HorizontalAnchor = 1f
                });
            }
            else
                WindowManager.Instance.ShowWindow(ModeratorWindow);

            if (Dropdown.IsVisible) Dropdown.Hide();
        }

        public void ShowItemInformation()
        {
            if (ItemInformationWindow.IsInvalid())
            {
                ItemInformationWindow = WindowManager.Instance.NewWindow(_windowBoundaries, ItemInformationPanel.transform as RectTransform, "Information", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect(25f, _windowBoundaries.rect.height - 25f, 0f, 0f),
                    Scale = WINDOWS_SCALE,
                });
            }
            else
            {
                WindowManager.Instance.ShowWindow(ItemInformationWindow);
            }

            if (Dropdown.IsVisible) Dropdown.Hide();
        }

        public void ShowItemAdditionalConfiguration()
        {
            if (ItemAdditionalConfigurationWindow.IsInvalid())
            {
                ItemAdditionalConfigurationWindow = WindowManager.Instance.NewWindow(_windowBoundaries, ItemAdditionalConfigurationPanel.transform as RectTransform, "Configuration", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect(((ItemInformationPanel.transform as RectTransform).rect.width + 50f) * WINDOWS_SCALE, _windowBoundaries.rect.height - 25f, 0f, 0f),
                    Scale = WINDOWS_SCALE,
                });
            }
            else
            {
                WindowManager.Instance.ShowWindow(ItemAdditionalConfigurationWindow);
            }

            if (Dropdown.IsVisible) Dropdown.Hide();
        }

        public void ShowImportedFiles()
        {
            if (ImportedFilesWindow.IsInvalid())
            {
                ImportedFilesWindow = WindowManager.Instance.NewWindow(_windowBoundaries, ImportedFilesPanel.transform as RectTransform, "Files", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect(((ItemInformationPanel.transform as RectTransform).rect.width + 50f) * WINDOWS_SCALE, _windowBoundaries.rect.height - (50f + (ItemAdditionalConfigurationPanel.transform as RectTransform).rect.height) * WINDOWS_SCALE, 0f, 0f),
                    Scale = WINDOWS_SCALE,
                });
            }
            else
            {
                WindowManager.Instance.ShowWindow(ImportedFilesWindow);
            }

            if (Dropdown.IsVisible) Dropdown.Hide();
        }

        public void ShowHierarchy()
        {
            if (HierarchyWindow.IsInvalid())
            {
                HierarchyWindow = WindowManager.Instance.NewWindow(_windowBoundaries, HierarchyPanel.transform as RectTransform, "Hierarchy", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect(25f, _windowBoundaries.rect.height - (50f + (ItemInformationPanel.transform as RectTransform).rect.height) * WINDOWS_SCALE, 0f, 0f),
                    Scale = WINDOWS_SCALE,
                });
            }
            else
            {
                WindowManager.Instance.ShowWindow(HierarchyWindow);
            }

            if (Dropdown.IsVisible) Dropdown.Hide();
        }

        public void ShowInspector()
        {
            if (InspectorWindow.IsInvalid())
            {
                InspectorWindow = WindowManager.Instance.NewWindow(_windowBoundaries, Inspector.transform as RectTransform, "Edit object", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect(_windowBoundaries.rect.width - 45f - (Inspector.transform as RectTransform).rect.width, _windowBoundaries.rect.height - 25f, 0f, 0f),
                    Scale = WINDOWS_SCALE,
                    HorizontalAnchor = 1f,
                });
            }
            else
            {
                WindowManager.Instance.ShowWindow(InspectorWindow);
            }

            if (Dropdown.IsVisible) Dropdown.Hide();
        }


        public void ShowItemOffsets()
        {
            if (ItemOffsetsWindow.IsInvalid())
            {
                ItemOffsetsWindow = WindowManager.Instance.NewWindow(_windowBoundaries, ItemOffsets.transform as RectTransform, "Configure offsets", new WindowRectSettings()
                {
                    PreserveContentSize = true,
                    Rect = new Rect((_windowBoundaries.rect.width / 2f) - ((ItemOffsets.transform as RectTransform).rect.width / 2f), (_windowBoundaries.rect.height / 2f) + ((ItemOffsets.transform as RectTransform).rect.height / 2f), 0f, 0f),
                    Scale = WINDOWS_SCALE,
                });
            }
            else
            {
                WindowManager.Instance.ShowWindow(ItemOffsetsWindow);
            }

            if (Dropdown.IsVisible) Dropdown.Hide();
        }

        public void OnExitButtonClicked()
        {
            Dropdown.Hide();
            ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, SceneTransitionManager.Instance.DisconnectAndExitToMainMenu, null);
        }

        public void OnSelectItemButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIs.ShowPersonalizationEditorItemsBrowser(base.transform);
        }

        public void OnSaveButtonClicked()
        {
            Dropdown.Hide();
            PersonalizationItemSaveResult saveResult = PersonalizationEditorManager.Instance.SaveItem();
            if (saveResult.HasFailed())
                ShowSaveErrorMessage(saveResult.Error);
            else
                ShowNotification("Success", $"Saved the item ({PersonalizationEditorManager.Instance.EditingItemInfo.Name})", UIElementPENotification.SuccessColor);
        }

        public void OnExportItemsButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIs.ShowPersonalizationEditorExportAllMenu(base.transform);
        }

        public void OnImportItemsButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIs.ShowPersonalizationEditorItemImportHelper(base.transform);
        }

        public void OnPlaytestButtonClicked()
        {
            Utilities.SetAnimationToggleOn();
            PersonalizationEditorManager.Instance.EnterPlaytestMode();
        }

        public void OnSendToVerificationButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIs.ShowPersonalizationEditorVerificationMenu(base.transform);
        }

        public void OnUploadButtonClicked()
        {
            Dropdown.Hide();
            _ = ModUIs.ShowPersonalizationEditorVerificationMenu(base.transform);
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
