using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementMultiplayerMessageBox : OverhaulUIBehaviour
    {
        [UIElement("Header")]
        public Text _headerText;
        [UIElement("Header")]
        public Outline _headerOutline;

        [UIElement("Description")]
        public Text _descriptionText;
        [UIElement("Description")]
        public Outline _descriptionOutline;

        [UIElement("ErrorMessage")]
        public Graphic _bgGraphic;
        [UIElement("Frame")]
        public Graphic _frameGraphic;

        [UIElementAction(nameof(OnRulesButtonClicked))]
        [UIElement("RulesButton")]
        public Button _rulesButton;

        private (Color, Color, Color)[] _colors;

        public bool showedFromCode
        {
            get;
            set;
        }

        public bool hasEverShowed
        {
            get;
            protected set;
        }

        protected override void OnInitialized()
        {
            _colors = new (Color, Color, Color)[]
            {
                (ModParseUtils.TryParseColor("E62E2E", Color.white), ModParseUtils.TryParseColor("661919", Color.gray), ModParseUtils.TryParseColor("A60000", Color.white)),
                (ModParseUtils.TryParseColor("E6B92E", Color.white), ModParseUtils.TryParseColor("998126", Color.gray), ModParseUtils.TryParseColor("F3B500", Color.white)),
                (ModParseUtils.TryParseColor("00B301", Color.white), ModParseUtils.TryParseColor("00610A", Color.gray), ModParseUtils.TryParseColor("08AE00", Color.white))
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
            _headerText.text = errorLabel;
            _descriptionText.text = errorDetails;
            _rulesButton.gameObject.SetActive(showRulesButton);
            SetColor(isWarning ? _colors[1].Item1 : _colors[0].Item1, isWarning ? _colors[1].Item2 : _colors[0].Item2, isWarning ? _colors[1].Item3 : _colors[0].Item3);
        }

        public void ShowSuccess()
        {
            _headerText.text = LocalizationManager.Instance.GetTranslatedString("multiplayer_connection_fine");
            _descriptionText.text = LocalizationManager.Instance.GetTranslatedString("multiplayer_connection_fine_desc");
            _rulesButton.gameObject.SetActive(false);
            SetColor(_colors[2].Item1, _colors[2].Item2, _colors[2].Item3);
        }

        public void SetColor(Color firstColor, Color secondColor, Color buttonColor)
        {
            _bgGraphic.color = firstColor;
            _frameGraphic.color = secondColor;
            _headerOutline.effectColor = Color.black;
            _descriptionOutline.effectColor = Color.black;
            _rulesButton.image.color = buttonColor;
        }

        public void OnRulesButtonClicked()
        {

        }
    }
}
