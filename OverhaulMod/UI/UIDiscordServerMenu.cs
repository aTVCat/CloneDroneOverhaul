using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDiscordServerMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("InviteLinkField")]
        private readonly InputField _urlField;

        [UIElementAction(nameof(OnCopyURLButtonClicked))]
        [UIElement("CopyURLButton")]
        private readonly Button _copyUrlButton;

        [UIElementAction(nameof(OnOpenURLInBrowserButtonClicked))]
        [UIElement("OpenURLInBrowserButton")]
        private readonly Button _openUrlInBrowserButton;

        public override bool hideTitleScreen => true;

        private string _url;

        protected override void OnInitialized()
        {
            string url = "https://discord.gg/ezhvabY63m";
            _url = url;
            _urlField.text = url;
        }

        public void OnCopyURLButtonClicked()
        {
            GUIUtility.systemCopyBuffer = _url;
        }

        public void OnOpenURLInBrowserButtonClicked()
        {
            Application.OpenURL(_url);
        }
    }
}
