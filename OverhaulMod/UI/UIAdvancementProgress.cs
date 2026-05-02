using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAdvancementProgress : OverhaulUIBehaviour
    {
        [UIElement("Panel")]
        private readonly CanvasGroup _canvasGroup;

        [UIElement("Image")]
        private readonly Image _achievementImage;
        [UIElement("ProgressText")]
        private readonly Text _achievementProgressText;
        [UIElement("ProgressFill")]
        private readonly Image _achievementProgressBarFill;

        public override bool CloseOnEscapeButtonPress => false;

        public float ShowUntil;

        public bool shouldBeVisible
        {
            get
            {
                return Time.unscaledTime < ShowUntil;
            }
        }

        protected override void OnInitialized()
        {
            _canvasGroup.alpha = 0f;
        }

        public void ShowProgress(GameplayAchievement gameplayAchievement)
        {
            Show();

            int progress = GameplayAchievementManager.Instance.GetProgress(gameplayAchievement.AchievementID);
            int targetProgress = gameplayAchievement.TargetProgress;

            if (progress >= targetProgress || progress % 25 != 0)
                return;

            _achievementImage.sprite = gameplayAchievement.GetImageSprite();
            _achievementProgressBarFill.fillAmount = progress / (float)targetProgress;
            _achievementProgressText.text = $"{progress}/{targetProgress}";
            waitThenHide().Run();
        }

        private IEnumerator waitThenHide()
        {
            _canvasGroup.alpha = 0.6f;
            yield return new WaitForSecondsRealtime(5f);
            _canvasGroup.alpha = 0f;
            yield break;
        }
    }
}
