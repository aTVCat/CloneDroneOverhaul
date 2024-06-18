using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDiscordServerMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("InviteLinkField")]
        private readonly InputField m_urlField;

        [UIElementAction(nameof(OnCopyURLButtonClicked))]
        [UIElement("CopyURLButton")]
        private readonly Button m_copyUrlButton;

        [UIElementAction(nameof(OnOpenURLInBrowserButtonClicked))]
        [UIElement("OpenURLInBrowserButton")]
        private readonly Button m_openUrlInBrowserButton;

        public override bool hideTitleScreen => true;

        private string m_url;

        protected override void OnInitialized()
        {
            string url = "https://discord.gg/ezhvabY63m";
            m_url = url;
            m_urlField.text = url;
        }

        public void OnCopyURLButtonClicked()
        {
            GUIUtility.systemCopyBuffer = m_url;
        }

        public void OnOpenURLInBrowserButtonClicked()
        {
            Application.OpenURL(m_url);
        }
    }
}
