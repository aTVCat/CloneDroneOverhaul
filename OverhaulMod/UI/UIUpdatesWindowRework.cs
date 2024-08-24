using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesWindowRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("InstalledVersionText")]
        private readonly Text m_installedVersionText;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_installedVersionText.text = ModBuildInfo.versionString;
        }
    }
}
