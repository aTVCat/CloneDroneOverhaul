using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIEndlessModeMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnPlayButtonClicked))]
        [UIElement("PlayButton")]
        private readonly Button m_playButton;

        [UIElementAction(nameof(OnLeaderboardButtonClicked))]
        [UIElement("ViewLeaderBoardButton")]
        private readonly Button m_leaderboardButton;

        public override void Show()
        {
            base.Show();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(false);
        }

        public override void Hide()
        {
            base.Hide();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        public void OnExitButtonClicked()
        {
            Hide();
        }

        public void OnPlayButtonClicked()
        {
            Hide();
            ModCache.titleScreenUI.OnPlayEndlessButtonClicked();
        }

        public void OnLeaderboardButtonClicked()
        {
            UIConstants.ShowLeaderboard(base.transform);
        }
    }
}
