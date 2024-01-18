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

        [UIElement("CurrentGameplayProgress")]
        private readonly Text m_progressText;

        public override bool dontRefreshUI => true;

        protected override void OnInitialized()
        {
            GameData gameData = GameDataManager.Instance?._endlessModeData;
            if (gameData != null)
            {
                EndlessTierDescription tierDesc = EndlessModeManager.Instance?.GetNextLevelDifficultyTierDescription(gameData.LevelIDsBeatenThisPlaythrough.Count);
                if (tierDesc == null)
                {
                    m_progressText.text = "Error: difficulty";
                }
                else
                {
                    string name = gameData.HumanFacts?.GetFullName();
                    string difficultyText = $" <color={tierDesc.TextColor.ToHex()}>{GetTierString(tierDesc.Tier)}</color>";
                    m_progressText.text = $"{name} - Level {gameData.LevelIDsBeatenThisPlaythrough.Count + 1}{difficultyText}";
                }
            }
            else
            {
                m_progressText.text = "N/A";
            }
        }

        public string GetTierString(DifficultyTier difficultyTier)
        {
            switch (difficultyTier)
            {
                case (DifficultyTier)9:
                    return "Nightmarium";
            }
            return difficultyTier.ToString();
        }

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
