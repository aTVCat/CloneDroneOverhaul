using InternalModBot;
using ModBotWebsiteAPI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIFeedbackMenu : OverhaulUIBehaviour
    {
        public static bool HasSentFeedback, HasLikedTheMod;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(On1RankClicked))]
        [UIElement("BadRank")]
        private readonly Button m_1rankButton;

        [UIElementAction(nameof(On2RankClicked))]
        [UIElement("MehRank")]
        private readonly Button m_2rankButton;

        [UIElementAction(nameof(On3RankClicked))]
        [UIElement("NeutralRank")]
        private readonly Button m_3rankButton;

        [UIElementAction(nameof(On4RankClicked))]
        [UIElement("GoodRank")]
        private readonly Button m_4rankButton;

        [UIElementAction(nameof(On5RankClicked))]
        [UIElement("SatisfiedRank")]
        private readonly Button m_5rankButton;

        [UIElementAction(nameof(OnSendButtonClicked))]
        [UIElement("SendButton")]
        private readonly Button m_sendButton;

        [UIElementAction(nameof(OnLikeButtonClicked))]
        [UIElement("LikeButton")]
        private readonly Button m_likeButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElementAction(nameof(OnImproveFieldChanged))]
        [UIElement("ImproveTextInputField")]
        private readonly InputField m_improveField;

        [UIElementAction(nameof(OnFavouriteFieldChanged))]
        [UIElement("FavouriteTextInputField")]
        private readonly InputField m_favouriteField;

        public override bool hideTitleScreen => true;

        private bool m_isSendingFeedback;

        public int selectedRank
        {
            get;
            private set;
        }

        public override void Show()
        {
            base.Show();

            m_likeButton.interactable = ModBotSignInUI.HasSignedIn && !HasLikedTheMod;
            refreshElements();
        }

        private void refreshElements()
        {
            bool shouldBeInteractable = !HasSentFeedback && !m_isSendingFeedback;

            m_improveField.interactable = shouldBeInteractable;
            m_favouriteField.interactable = shouldBeInteractable;
            m_1rankButton.interactable = selectedRank != 1 && shouldBeInteractable;
            m_2rankButton.interactable = selectedRank != 2 && shouldBeInteractable;
            m_3rankButton.interactable = selectedRank != 3 && shouldBeInteractable;
            m_4rankButton.interactable = selectedRank != 4 && shouldBeInteractable;
            m_5rankButton.interactable = selectedRank != 5 && shouldBeInteractable;
            m_sendButton.interactable = shouldBeInteractable && selectedRank > 0 && selectedRank < 6 && !m_improveField.text.IsNullOrEmpty();

            m_loadingIndicator.SetActive(m_isSendingFeedback);
        }

        private void likeTheMod()
        {
            HasLikedTheMod = true;
            m_likeButton.interactable = false;
            ModBotWebsiteAPI.API.Like("rAnDomPaTcHeS1", "true", delegate (JsonObject jsonObject)
            {
                ModUIUtils.MessagePopupOK("Successfully liked the mod!", "Thanks!", false);
            });
        }

        public void OnImproveFieldChanged(string text)
        {
            refreshElements();
        }

        public void OnFavouriteFieldChanged(string text)
        {
            refreshElements();
        }

        public void OnSendButtonClicked()
        {
            HasSentFeedback = true;
            m_isSendingFeedback = true;
            refreshElements();
            WebhookManager.Instance.ExecuteFeedbacksWebhook(selectedRank, m_improveField.text, m_favouriteField.text, delegate
            {
                m_isSendingFeedback = false;
                refreshElements();
                ModUIUtils.MessagePopupOK("Successfully sent the feedback!", "Thanks!", true);
            }, delegate (string error)
            {
                m_isSendingFeedback = false;
                refreshElements();
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
            refreshElements();
        }

        public void On2RankClicked()
        {
            selectedRank = 2;
            refreshElements();
        }

        public void On3RankClicked()
        {
            selectedRank = 3;
            refreshElements();
        }

        public void On4RankClicked()
        {
            selectedRank = 4;
            refreshElements();
        }

        public void On5RankClicked()
        {
            selectedRank = 5;
            refreshElements();
        }
    }
}
