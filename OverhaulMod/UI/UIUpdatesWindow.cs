using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_editorButton;

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowUpdateInfoEditor(base.transform);
        }
    }
}
