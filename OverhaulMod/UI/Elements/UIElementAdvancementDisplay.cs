using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementAdvancementDisplay : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [UIElement("Name")]
        private Text m_advancementName;
        [UIElement("Description")]
        private Text m_advancementDescription;
        [UIElement("Image")]
        private Image m_advancementImage;

        [UIElement("ProgressText")]
        private Text m_progressText;
        [UIElement("BarBG")]
        private GameObject m_progressBar;
        [UIElement("ProgressFill")]
        private Image m_progressBarFill;

        [UIElement("RewardInfo", false)]
        private GameObject m_rewardDisplayObject;
        [UIElement("RewardText")]
        private Text m_rewardText;

        [UIElement("CompletedIndicator", false)]
        private GameObject m_completedIndicator;

        public GameplayAchievement gameplayAchievement
        {
            get;
            private set;
        }

        public bool mouseIn
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
            mouseIn = false;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseIn = true;
            m_rewardDisplayObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseIn = false;
            m_rewardDisplayObject.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mouseIn = false;
            m_rewardDisplayObject.SetActive(false);
        }
    }
}
