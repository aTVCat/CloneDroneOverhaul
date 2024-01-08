using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
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
            TransitionManager.Instance.DoNonSceneTransition(transitionCoroutine());
        }

        public void OnLeaderboardButtonClicked()
        {
            ModUIConstants.ShowLeaderboard(base.transform);
        }

        private IEnumerator transitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.25f);
            Hide();
            ModCache.titleScreenUI.OnPlayEndlessButtonClicked();
            yield return new WaitUntil(() => CharacterTracker.Instance._player);
            yield return new WaitForSecondsRealtime(0.1f);
            TransitionManager.Instance.EndTransition();
            yield break;
        }
    }
}
