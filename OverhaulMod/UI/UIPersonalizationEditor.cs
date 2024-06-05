using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditor : OverhaulUIBehaviour
    {
        private static List<UIElementPersonalizationEditorDropdown.OptionData> s_fileOptions;

        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnSelectItemButtonClicked))]
        [UIElement("SelectItemButton")]
        private readonly Button m_selectItemButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnSendToVerificationButtonClicked))]
        [UIElement("SendToVerificationButton")]
        private readonly Button m_sendToVerificationButton;

        [UIElement("DeveloperPanel", false)]
        private readonly RectTransform m_developerPanel;

        [UIElement("ToolBar")]
        public RectTransform ToolBarTransform;

        [UIElement("InspectorWindow", typeof(UIElementPersonalizationEditorInspector), false)]
        public readonly UIElementPersonalizationEditorInspector Inspector;

        [UIElement("UtilitiesPanel", typeof(UIElementPersonalizationEditorUtilitiesPanel), false)]
        public readonly UIElementPersonalizationEditorUtilitiesPanel Utilities;

        [UIElement("ObjectPropertiesWindow", typeof(UIElementPersonalizationEditorPropertiesPanel), false)]
        public readonly UIElementPersonalizationEditorPropertiesPanel PropertiesPanel;

        [UIElement("Dropdown", typeof(UIElementPersonalizationEditorDropdown), false)]
        public readonly UIElementPersonalizationEditorDropdown Dropdown;

        [UIElementAction(nameof(OnFileButtonClicked))]
        [UIElement("FileButton")]
        private readonly Button m_toolbarFileButton;

        public string InspectorWindowID, DeveloperWindowID, ObjectPropertiesWindowID;

        public override bool enableCursor => true;

        public static UIPersonalizationEditor instance
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            instance = this;
            s_fileOptions = new List<UIElementPersonalizationEditorDropdown.OptionData>()
            {
                new UIElementPersonalizationEditorDropdown.OptionData("Open", "Redirect-16x16", instance.OnSelectItemButtonClicked),
                new UIElementPersonalizationEditorDropdown.OptionData("Save", "Save16x16", instance.OnSaveButtonClicked),
                new UIElementPersonalizationEditorDropdown.OptionData(true),
                new UIElementPersonalizationEditorDropdown.OptionData("Upload", "Redirect-16x16", instance.OnUploadButtonClicked),
                new UIElementPersonalizationEditorDropdown.OptionData(true),
                new UIElementPersonalizationEditorDropdown.OptionData("Exit", "Exit-V2-16x16", instance.OnExitButtonClicked),
            };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ShowSaveErrorMessage(string message)
        {
            ModUIUtils.MessagePopupOK("Item save error", message, 150f, true);
        }

        public void ShowInspectorWindow()
        {
            ModUIManager.WindowManager windowManager = ModUIManager.Instance.windowManager;
            if (InspectorWindowID == null)
                InspectorWindowID = windowManager.Window(base.transform, Inspector.transform, "Edit item", Vector2.one * -1f, (Vector2.right * -250f) + (Vector2.up * 220f));
            else
                windowManager.ShowWindow(InspectorWindowID);

            if (ObjectPropertiesWindowID == null)
                ObjectPropertiesWindowID = windowManager.Window(base.transform, PropertiesPanel.transform, "Edit object", Vector2.one * -1f, (Vector2.right * 250f) + (Vector2.up * 220f));
            else
                windowManager.ShowWindow(ObjectPropertiesWindowID);

            if (PersonalizationEditorManager.Instance.canVerifyItems)
            {
                if (DeveloperWindowID == null)
                    DeveloperWindowID = windowManager.Window(base.transform, m_developerPanel, "Item moderator", Vector2.one * -1f, Vector2.up * -120f);
                else
                    windowManager.ShowWindow(DeveloperWindowID);
            }
        }

        public void OnExitButtonClicked()
        {
            Dropdown.Hide();
            ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, SceneTransitionManager.Instance.DisconnectAndExitToMainMenu, null);
        }

        public void OnSelectItemButtonClicked()
        {
            Dropdown.Hide();
            ModUIConstants.ShowPersonalizationEditorItemsBrowser(base.transform);
        }

        public void OnSaveButtonClicked()
        {
            Dropdown.Hide();
            if (!PersonalizationEditorManager.Instance.SaveItem(out string error))
                ShowSaveErrorMessage(error);
        }

        public void OnSendToVerificationButtonClicked()
        {
            Dropdown.Hide();
            ModUIConstants.ShowPersonalizationEditorVerificationMenu(base.transform);
        }

        public void OnUploadButtonClicked()
        {
            Dropdown.Hide();
            ModUIConstants.ShowPersonalizationEditorVerificationMenu(base.transform);
        }

        public void OnFileButtonClicked()
        {
            if (Dropdown.gameObject.activeSelf)
                return;

            Dropdown.ShowWithOptions(s_fileOptions, m_toolbarFileButton.transform as RectTransform);
        }
    }
}
