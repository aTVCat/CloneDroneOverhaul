using ModBotWebsiteAPI;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIFeedbackMenu : OverhaulUIBehaviour
    {
        public const int CHARACTER_LIMIT = 500;

        [ModSetting(ModSettingIDs.HAS_EVER_SENT_FEEDBACK, false)]
        public static bool HasEverSentFeedback;

        [ModSetting(ModSettingIDs.FEEDBACK_MENU_RATE, 0)]
        public static int SavedRating;

        [ModSetting(ModSettingIDs.FEEDBACK_MENU_IMPROVE_TEXT, null)]
        public static string SavedImproveText;

        [ModSetting(ModSettingIDs.FEEDBACK_MENU_FAVORITE_TEXT, null)]
        public static string SavedFavoriteText;

        public static bool HasSentFeedback, HasLikedTheMod;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(On1RankClicked))]
        [UIElement("BadRank")]
        private readonly Button _1rankButton;

        [UIElementAction(nameof(On2RankClicked))]
        [UIElement("MehRank")]
        private readonly Button _2rankButton;

        [UIElementAction(nameof(On3RankClicked))]
        [UIElement("NeutralRank")]
        private readonly Button _3rankButton;

        [UIElementAction(nameof(On4RankClicked))]
        [UIElement("GoodRank")]
        private readonly Button _4rankButton;

        [UIElementAction(nameof(On5RankClicked))]
        [UIElement("SatisfiedRank")]
        private readonly Button _5rankButton;

        [UIElementAction(nameof(OnSendButtonClicked))]
        [UIElement("SendButton")]
        private readonly Button _sendButton;

        [UIElementAction(nameof(OnLikeButtonClicked))]
        [UIElement("LikeButton")]
        private readonly Button _likeButton;

        [UIElementAction(nameof(OnExitGameButtonClicked))]
        [UIElement("ExitGameButton")]
        private readonly Button _exitGameButton;

        [UIElementAction(nameof(OnSkipButtonClicked))]
        [UIElement("SkipButton")]
        private readonly Button _skipButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;

        [UIElementAction(nameof(OnImproveFieldChanged))]
        [UIElement("ImproveTextInputField")]
        private readonly InputField _improveField;

        [UIElementAction(nameof(OnFavoriteFieldChanged))]
        [UIElement("FavouriteTextInputField")]
        private readonly InputField _favoriteField;

        [UIElement("charLeftText_Improve")]
        private readonly Text _improveFieldCharsLeftText;

        [UIElement("charLeftText_Favorite")]
        private readonly Text _favoriteFieldCharsLeftText;

        public override bool HideTitleScreen => true;

        private bool _refreshElementsNextFrame;

        private bool _usedSavedSettings;

        private bool _isSendingFeedback;

        private string _charsLeftText;

        public int selectedRank
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            _improveField.characterLimit = CHARACTER_LIMIT;
            _favoriteField.characterLimit = CHARACTER_LIMIT;
        }

        public override void Update()
        {
            if (_refreshElementsNextFrame)
            {
                _refreshElementsNextFrame = false;
                refreshElements();
            }
        }

        public override void Show()
        {
            base.Show();

            _charsLeftText = LocalizationManager.Instance.GetTranslatedString("charsleft");
            _likeButton.interactable = API.HasSession && !HasLikedTheMod;

            _skipButton.interactable = true;

            if (!_usedSavedSettings)
            {
                _usedSavedSettings = true;

                selectedRank = SavedRating;
                _improveField.text = SavedImproveText;
                _favoriteField.text = SavedFavoriteText;
            }

            refreshElementsNextFrame();

            _improveFieldCharsLeftText.text = getCharLeftTextForField(_improveField);
            _favoriteFieldCharsLeftText.text = getCharLeftTextForField(_favoriteField);
        }

        public override void Hide()
        {
            base.Hide();

            ModSettingsManager.SetIntValue(ModSettingIDs.FEEDBACK_MENU_RATE, selectedRank);
            ModSettingsManager.SetStringValue(ModSettingIDs.FEEDBACK_MENU_IMPROVE_TEXT, _improveField.text);
            ModSettingsManager.SetStringValue(ModSettingIDs.FEEDBACK_MENU_FAVORITE_TEXT, _favoriteField.text);
            ModSettingsDataManager.Instance.Save();
        }

        private void refreshElementsNextFrame()
        {
            _refreshElementsNextFrame = true;
        }

        private void refreshElements()
        {
            bool shouldBeInteractable = !HasSentFeedback && !_isSendingFeedback;

            _improveField.interactable = shouldBeInteractable;
            _favoriteField.interactable = shouldBeInteractable;
            _1rankButton.interactable = selectedRank != 1 && shouldBeInteractable;
            _2rankButton.interactable = selectedRank != 2 && shouldBeInteractable;
            _3rankButton.interactable = selectedRank != 3 && shouldBeInteractable;
            _4rankButton.interactable = selectedRank != 4 && shouldBeInteractable;
            _5rankButton.interactable = selectedRank != 5 && shouldBeInteractable;
            _sendButton.interactable = shouldBeInteractable && selectedRank > 0 && selectedRank < 6 && !_improveField.text.IsNullOrEmpty() && !_improveField.text.IsNullOrWhiteSpace();

            _loadingIndicator.SetActive(_isSendingFeedback);
        }

        private void likeTheMod()
        {
            HasLikedTheMod = true;
            _likeButton.interactable = false;
            API.Like("rAnDomPaTcHeS1", "true", delegate (JsonObject jsonObject)
            {
                ModUIUtils.MessagePopupOK(LocalizationManager.Instance.GetTranslatedString("feedback_like_mod_header"), LocalizationManager.Instance.GetTranslatedString("feedback_like_mod_description"), false);
            });
        }

        private string getCharLeftTextForField(InputField inputField)
        {
            return $"{inputField.characterLimit - inputField.text.Length} {_charsLeftText}";
        }

        public void SetExitButtonVisible(bool value)
        {
            _exitGameButton.gameObject.SetActive(value);
            _skipButton.gameObject.SetActive(value && !HasEverSentFeedback);
        }

        public void OnExitGameButtonClicked()
        {
            Hide();
            Application.Quit();
        }

        public void OnImproveFieldChanged(string text)
        {
            refreshElementsNextFrame();
            _improveFieldCharsLeftText.text = getCharLeftTextForField(_improveField);
        }

        public void OnFavoriteFieldChanged(string text)
        {
            refreshElementsNextFrame();
            _favoriteFieldCharsLeftText.text = getCharLeftTextForField(_favoriteField);
        }

        public void OnSendButtonClicked()
        {
            HasSentFeedback = true;
            _isSendingFeedback = true;
            refreshElementsNextFrame();
            PostmanManager.Instance.SendFeedback(selectedRank, _improveField.text, _favoriteField.text, delegate
            {
                _isSendingFeedback = false;
                refreshElementsNextFrame();
                ModUIUtils.MessagePopupOK(LocalizationManager.Instance.GetTranslatedString("feedback_success_header"), LocalizationManager.Instance.GetTranslatedString("feedback_success_text"), true);

                if (!HasEverSentFeedback)
                {
                    ModSettingsManager.SetStringValue(ModSettingIDs.FEEDBACK_MENU_IMPROVE_TEXT, string.Empty);
                    ModSettingsManager.SetStringValue(ModSettingIDs.FEEDBACK_MENU_FAVORITE_TEXT, string.Empty);
                    ModSettingsManager.SetBoolValue(ModSettingIDs.HAS_EVER_SENT_FEEDBACK, true);
                    _skipButton.gameObject.SetActive(false);
                }
            }, delegate (string error)
            {
                _isSendingFeedback = false;
                refreshElementsNextFrame();
                ModUIUtils.MessagePopupOK("Could not send the feedback", $"Error details:\n{error}\n\nTry again later", true);
            });
        }

        public void OnLikeButtonClicked()
        {
            likeTheMod();
        }

        public void On1RankClicked()
        {
            selectedRank = 1;
            refreshElementsNextFrame();
        }

        public void On2RankClicked()
        {
            selectedRank = 2;
            refreshElementsNextFrame();
        }

        public void On3RankClicked()
        {
            selectedRank = 3;
            refreshElementsNextFrame();
        }

        public void On4RankClicked()
        {
            selectedRank = 4;
            refreshElementsNextFrame();
        }

        public void On5RankClicked()
        {
            selectedRank = 5;
            refreshElementsNextFrame();
        }

        public void OnSkipButtonClicked()
        {
            _skipButton.interactable = false;
            ModSettingsManager.SetBoolValue(ModSettingIDs.HAS_EVER_SENT_FEEDBACK, true);
        }
    }
}
