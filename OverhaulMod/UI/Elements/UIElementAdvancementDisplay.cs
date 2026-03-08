using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementAdvancementDisplay : OverhaulUIBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [UIElement("Name")]
        private readonly Text _advancementName;
        [UIElement("Description")]
        private readonly Text _advancementDescription;
        [UIElement("Image")]
        private readonly Image _advancementImage;

        [UIElement("ProgressText")]
        private readonly Text _progressText;
        [UIElement("BarBG")]
        private readonly GameObject _progressBar;
        [UIElement("ProgressFill")]
        private readonly Image _progressBarFill;

        [UIElement("RewardInfo", false)]
        private readonly GameObject _rewardDisplayObject;
        [UIElement("RewardText")]
        private readonly Text _rewardText;

        [UIElement("CompletedIndicator", false)]
        private readonly GameObject _completedIndicator;

        public GameplayAchievement gameplayAchievement
        {
            get;
            private set;
        }

        public void Populate(GameplayAchievement gameplayAchievement, GameplayAchievementManager gameplayAchievementManager)
        {
            InitializeElement();

            bool isComplete = gameplayAchievementManager.HasUnlockedAchievement(gameplayAchievement.AchievementID);
            int currentProgress = gameplayAchievementManager.GetProgress(gameplayAchievement.AchievementID);
            int targetProgress = gameplayAchievement.TargetProgress;

            _rewardText.text = gameplayAchievement.GetRewardText();
            _advancementName.text = LocalizationManager.Instance.GetTranslatedString(gameplayAchievement.Name);
            _advancementDescription.text = (gameplayAchievement.IsHidden && !isComplete) ? "???" : LocalizationManager.Instance.GetTranslatedString(gameplayAchievement.Description);
            _advancementImage.sprite = gameplayAchievement.GetImageSprite();
            SetProgressDisplays(currentProgress, targetProgress, isComplete);

            this.gameplayAchievement = gameplayAchievement;
        }

        public void SetProgressDisplays(int currentProgress, int targetProgress, bool isComplete)
        {
            _completedIndicator.SetActive(isComplete);

            bool shouldActivate = targetProgress > 1;
            if (!shouldActivate || isComplete)
            {
                _progressText.enabled = false;
                _progressBar.SetActive(false);
                return;
            }

            _progressText.enabled = true;
            _progressText.text = $"{currentProgress}/{targetProgress}";
            _progressBar.SetActive(true);
            _progressBarFill.fillAmount = currentProgress / (float)targetProgress;
        }

        public override void OnDisable()
        {
            _rewardDisplayObject.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _rewardDisplayObject.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _rewardDisplayObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _rewardDisplayObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _rewardDisplayObject.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _rewardDisplayObject.SetActive(false);
        }
    }
}
