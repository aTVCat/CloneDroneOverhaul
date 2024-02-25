using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIEndlessGameLossWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("RestartButton")]
        private readonly Button m_restartButton;

        [UIElementAction(nameof(Hide))]
        [UIElement("MainMenuButton")]
        private readonly Button m_mainMenuButton;

        public override bool enableCursor => true;
    }
}
