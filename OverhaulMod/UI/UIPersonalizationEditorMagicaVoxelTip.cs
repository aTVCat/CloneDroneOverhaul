using System;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorMagicaVoxelTip : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnOkButtonClicked))]
        [UIElement("OKButton")]
        private readonly Button m_okButton;

        public Action Callback;

        public override void Show()
        {
            base.Show();
            UIElementPersonalizationEditorFileImportPanel.HasShownMagicaVoxelTip = true;
        }

        public override void Hide()
        {
            base.Hide();
            Callback = null;
        }

        public void OnOkButtonClicked()
        {
            Callback?.Invoke();
            Hide();
        }
    }
}
