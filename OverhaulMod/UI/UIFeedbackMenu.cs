using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIFeedbackMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        public override bool hideTitleScreen => true;
    }
}
