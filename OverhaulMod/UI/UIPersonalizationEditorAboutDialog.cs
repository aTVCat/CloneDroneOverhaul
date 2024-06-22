using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorAboutDialog : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;
    }
}
