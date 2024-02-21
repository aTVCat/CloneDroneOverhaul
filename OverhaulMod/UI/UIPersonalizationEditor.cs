using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditor : OverhaulUIBehaviour
    {
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

        [UIElement("ToolBar")]
        public RectTransform ToolBarTransform;

        [UIElement("LeftPanel")]
        public RectTransform LeftPanelTransform;

        [UIElement("Inspector", typeof(UIElementPersonalizationEditorInspector))]
        public readonly UIElementPersonalizationEditorInspector Inspector;

        [UIElement("UtilitiesPanel", typeof(UIElementPersonalizationEditorUtilitiesPanel))]
        public readonly UIElementPersonalizationEditorUtilitiesPanel Utilities;

        public override bool enableCursor => true;

        public static UIPersonalizationEditor instance
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            instance = this;
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

        public void OnExitButtonClicked()
        {
            ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, SceneTransitionManager.Instance.DisconnectAndExitToMainMenu, null);
        }

        public void OnSelectItemButtonClicked()
        {
            ModUIConstants.ShowPersonalizationEditorItemsBrowser(base.transform);
        }

        public void OnSaveButtonClicked()
        {
            Inspector.ApplyValues();
            PersonalizationEditorManager.Instance.SaveItem();
        }

        public void OnSendToVerificationButtonClicked()
        {
            ModUIConstants.ShowPersonalizationEditorVerificationMenu(base.transform);
        }
    }
}
