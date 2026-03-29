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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnPlayButtonClicked))]
        [UIElement("PlayButton")]
        private readonly Button _playButton;

        [UIElementAction(nameof(OnLeaderboardButtonClicked))]
        [UIElement("ViewLeaderBoardButton")]
        private readonly Button _leaderboardButton;

        [ShowTooltipOnHighLight("erase progress", 1.5f, true)]
        [UIElementAction(nameof(OnResetProgressButtonClicked))]
        [UIElement("DeleteProgressButton")]
        private readonly Button _resetProgressButton;

        [UIElement("CurrentGameplayProgress")]
        private readonly Text _progressText;

        public override bool RefreshOnlyCursor => true;

        public void RefreshProgressDisplays()
        {
            GameData gameData = GameDataManager.Instance?._endlessModeData;
            if (gameData != null)
            {
                EndlessTierDescription tierDesc = EndlessModeManager.Instance?.GetNextLevelDifficultyTierDescription(gameData.LevelIDsBeatenThisPlaythrough.Count);
                if (tierDesc == null)
                {
                    _progressText.text = "Error: difficulty";
                }
                else
                {
                    string name = gameData.HumanFacts?.GetFullName();
                    string difficultyText = $" <color={tierDesc.TextColor.ToHex()}>{LocalizationManager.Instance.GetTranslatedString(tierDesc.Tier.GetTierString())}</color>";
                    _progressText.text = $"{name} - {LocalizationManager.Instance.GetTranslatedString("level_context")} {gameData.LevelIDsBeatenThisPlaythrough.Count + 1}{difficultyText}";
                }
            }
            else
            {
                _progressText.text = "N/A";
            }
        }

        public override void Show()
        {
            base.Show();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(false);
            RefreshProgressDisplays();
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
            if (!TransitionManager.OverhaulSceneTransitions)
            {
                Hide();
                ModCache.titleScreenUI.OnPlayEndlessButtonClicked();
                return;
            }
            TransitionManager.Instance.DoNonSceneTransition(transitionCoroutine());
        }

        public void OnLeaderboardButtonClicked()
        {
            _ = ModUIConstants.ShowLeaderboard(base.transform, GameDataManager.Instance._endlessHighScores, "EndlessHighScores.json");
        }

        public void OnResetProgressButtonClicked()
        {
            ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("endless_mode_erase_progress_header  "), LocalizationManager.Instance.GetTranslatedString("endless_mode_erase_progress_description"), 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                GameData gameData = new GameData
                {
                    HumanFacts = HumanFactsManager.Instance.GetRandomFactSet()
                };
                gameData.RepairAnyMissingFields(true);
                gameData.SetDirty(true);
                GameDataManager.Instance._endlessModeData = gameData;

                RefreshProgressDisplays();
            });
        }

        private IEnumerator transitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(1f);
            Hide();
            ModCache.titleScreenUI.OnPlayEndlessButtonClicked();
            yield return new WaitUntil(() => CharacterTracker.Instance._player);
            yield return new WaitForSecondsRealtime(0.1f);
            TransitionManager.Instance.EndTransition();
            yield break;
        }
    }
}
