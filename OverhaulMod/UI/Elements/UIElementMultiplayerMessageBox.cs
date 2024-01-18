using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementMultiplayerMessageBox : OverhaulUIBehaviour
    {
        [UIElement("Header")]
        public Text m_headerText;
        [UIElement("Header")]
        public Outline m_headerOutline;

        [UIElement("Description")]
        public Text m_descriptionText;
        [UIElement("Description")]
        public Outline m_descriptionOutline;

        [UIElement("ErrorMessage")]
        public Graphic m_bgGraphic;
        [UIElement("Frame")]
        public Graphic m_frameGraphic;

        [UIElementAction(nameof(OnRulesButtonClicked))]
        [UIElement("RulesButton")]
        public Button m_rulesButton;

        private (Color, Color, Color)[] m_colors;

        protected override void OnInitialized()
        {
            m_colors = new (Color, Color, Color)[]
            {
                (ModParseUtils.TryParseToColor("E62E2E", Color.white), ModParseUtils.TryParseToColor("661919", Color.gray), ModParseUtils.TryParseToColor("A60000", Color.white)),
                (ModParseUtils.TryParseToColor("E6B92E", Color.white), ModParseUtils.TryParseToColor("998126", Color.gray), ModParseUtils.TryParseToColor("F3B500", Color.white)),
                (ModParseUtils.TryParseToColor("00B301", Color.white), ModParseUtils.TryParseToColor("00610A", Color.gray), ModParseUtils.TryParseToColor("08AE00", Color.white))
            };
        }

        public override void Show()
        {
            base.Show();
            if (!showedFromCode)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            bool userBanned = MultiplayerLoginManager.Instance.IsBanned();
            bool userLoggedIn = MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab();
            BanOrWarningMessage banOrWarningMessage = PlayFabPlayerDataManager.Instance.GetBanOrWarningMessage();
            if (!userLoggedIn || userBanned)
            {
                if (banOrWarningMessage != null)
                {
                    ShowError(banOrWarningMessage.GetTranslatedTitle(), banOrWarningMessage.GetTranslatedBanMessage(), banOrWarningMessage.ShouldShowRules(), banOrWarningMessage.IsWarning);
                    return;
                }
                else if (userBanned)
                {
                    string translatedString = LocalizationManager.Instance.GetTranslatedString("Banned from Multiplayer!");
                    string translatedString2 = LocalizationManager.Instance.GetTranslatedString("bannedMessageGeneric");
                    ShowError(translatedString, translatedString2, false, false);
                    return;
                }
                ShowError(LocalizedStrings.CONNECTION_PROBLEM, LocalizedStrings.USER_NOT_LOGGED_IN, false, false);
            }
            else if (banOrWarningMessage != null)
            {
                ShowError(banOrWarningMessage.GetTranslatedTitle(), banOrWarningMessage.GetTranslatedBanMessage(), banOrWarningMessage.ShouldShowRules(), banOrWarningMessage.IsWarning);
            }
            else
            {
                ShowSuccess();
            }
        }

        public void ShowError(string errorLabel, string errorDetails, bool showRulesButton = false, bool isWarning = false)
        {
            m_headerText.text = errorLabel;
            m_descriptionText.text = errorDetails;
            m_rulesButton.gameObject.SetActive(showRulesButton);
            SetColor(isWarning ? m_colors[1].Item1 : m_colors[0].Item1, isWarning ? m_colors[1].Item2 : m_colors[0].Item2, isWarning ? m_colors[1].Item3 : m_colors[0].Item3);
        }

        public void ShowSuccess()
        {
            m_headerText.text = "No errors encountered.";
            m_descriptionText.text = "Multiplayer should work fine for you.";
            m_rulesButton.gameObject.SetActive(false);
            SetColor(m_colors[2].Item1, m_colors[2].Item2, m_colors[2].Item3);
        }

        public void SetColor(Color firstColor, Color secondColor, Color buttonColor)
        {
            m_bgGraphic.color = firstColor;
            m_frameGraphic.color = secondColor;
            m_headerOutline.effectColor = Color.black;
            m_descriptionOutline.effectColor = Color.black;
            m_rulesButton.image.color = buttonColor;
        }

        public void OnRulesButtonClicked()
        {

        }
    }
}
