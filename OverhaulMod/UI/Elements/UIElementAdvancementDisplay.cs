using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementAdvancementDisplay : OverhaulUIBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [UIElement("Name")]
        private readonly Text m_advancementName;
        [UIElement("Description")]
        private readonly Text m_advancementDescription;
        [UIElement("Image")]
        private readonly Image m_advancementImage;

        [UIElement("ProgressText")]
        private readonly Text m_progressText;
        [UIElement("BarBG")]
        private readonly GameObject m_progressBar;
        [UIElement("ProgressFill")]
        private readonly Image m_progressBarFill;

        [UIElement("RewardInfo", false)]
        private readonly GameObject m_rewardDisplayObject;
        [UIElement("RewardText")]
        private readonly Text m_rewardText;

        [UIElement("CompletedIndicator", false)]
        private readonly GameObject m_completedIndicator;

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

            m_rewardText.text = gameplayAchievement.GetRewardText();
            m_advancementName.text = LocalizationManager.Instance.GetTranslatedString(gameplayAchievement.Name);
            m_advancementDescription.text = (gameplayAchievement.IsHidden && !isComplete) ? "???" : LocalizationManager.Instance.GetTranslatedString(gameplayAchievement.Description);
            m_advancementImage.sprite = gameplayAchievement.GetImageSprite();
            SetProgressDisplays(currentProgress, targetProgress, isComplete);

            this.gameplayAchievement = gameplayAchievement;
        }

        public void SetProgressDisplays(int currentProgress, int targetProgress, bool isComplete)
        {
            m_completedIndicator.SetActive(isComplete);

            bool shouldActivate = targetProgress > 1;
            if (!shouldActivate || isComplete)
            {
                m_progressText.enabled = false;
                m_progressBar.SetActive(false);
                return;
            }

            m_progressText.enabled = true;
            m_progressText.text = $"{currentProgress}/{targetProgress}";
            m_progressBar.SetActive(true);
            m_progressBarFill.fillAmount = currentProgress / (float)targetProgress;
        }

        public override void OnDisable()
        {
            m_rewardDisplayObject.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            m_rewardDisplayObject.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            m_rewardDisplayObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_rewardDisplayObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_rewardDisplayObject.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_rewardDisplayObject.SetActive(false);
        }
    }
}
