using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDownloadCustomizationAssetsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;
    }
}
