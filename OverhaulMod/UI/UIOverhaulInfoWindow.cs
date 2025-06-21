using OverhaulMod.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIOverhaulInfoWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnModBotPageButtonClicked))]
        [UIElement("ModBotPageButton")]
        private readonly Button m_modBotPageButton;

        [UIElementAction(nameof(OnAuthorButtonClicked))]
        [UIElement("authorNameButton")]
        private readonly Button m_authorNameButton;

        [UIElementAction(nameof(OnCreditsButtonClicked))]
        [UIElement("CreditsButton")]
        private readonly Button m_creditsButton;

        [UIElement("ModVersionText")]
        private readonly Text m_versionText;

        [UIElement("ModCompilationDateText")]
        private readonly Text m_compilationTimeText;

        protected override void OnInitialized()
        {
            m_versionText.text = ModBuildInfo.versionString;

            ModBuildInfo.ExtraInfo extraInfo = ModBuildInfo.extraInfo;
            if (extraInfo != null && !ModBuildInfo.extraInfoError)
            {
                m_compilationTimeText.text = extraInfo.CompileTime.ToShortDateString();
            }
            else
            {
                m_compilationTimeText.text = "Unknown";
            }
        }

        public void OnAuthorButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/profiles/76561199028311109");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/profiles/76561199028311109");
            }
        }

        public void OnCreditsButtonClicked()
        {
            _ = ModUIConstants.ShowCreditsMenu(base.transform);
        }

        public void OnModBotPageButtonClicked()
        {
            Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }
    }
}
