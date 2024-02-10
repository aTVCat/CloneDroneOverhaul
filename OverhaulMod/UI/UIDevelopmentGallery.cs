using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDevelopmentGallery : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;
    }
}
