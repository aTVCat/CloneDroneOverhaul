using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationItemsBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        public override bool enableCursor => true;
    }
}
