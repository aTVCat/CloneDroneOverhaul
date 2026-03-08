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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnModBotPageButtonClicked))]
        [UIElement("ModBotPageButton")]
        private readonly Button _modBotPageButton;

        [UIElementAction(nameof(OnAuthorButtonClicked))]
        [UIElement("authorNameButton")]
        private readonly Button _authorNameButton;

        [UIElementAction(nameof(OnCreditsButtonClicked))]
        [UIElement("CreditsButton")]
        private readonly Button _creditsButton;

        [UIElement("ModVersionText")]
        private readonly Text _versionText;

        [UIElement("ModCompilationDateText")]
        private readonly Text _compilationTimeText;

        protected override void OnInitialized()
        {
            _versionText.text = ModBuildInfo.versionString;

            ModBuildInfo.ExtraInfo extraInfo = ModBuildInfo.extraInfo;
            if (extraInfo != null && !ModBuildInfo.extraInfoError)
            {
                _compilationTimeText.text = extraInfo.CompileTime.ToShortDateString();
            }
            else
            {
                _compilationTimeText.text = "Unknown";
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
