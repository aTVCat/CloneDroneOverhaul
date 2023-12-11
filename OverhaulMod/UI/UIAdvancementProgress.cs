using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAdvancementProgress : OverhaulUIBehaviour 
    {
        [UIElement("Panel")]
        private CanvasGroup m_canvasGroup;

        [UIElement("Image")]
        private Image m_achievementImage;
        [UIElement("ProgressText")]
        private Text m_achievementProgressText;
        [UIElement("ProgressFill")]
        private Image m_achievementProgressBarFill;

        public float ShowUntil;

        public bool shouldBeVisible
        {
            get
            {
                return Time.unscaledTime < ShowUntil;
            }
        }

        public void ShowProgress(GameplayAchievement gameplayAchievement)
        {
            Show();

            int progress = GameplayAchievementManager.Instance.GetProgress(gameplayAchievement.AchievementID);
            int targetProgress = gameplayAchievement.TargetProgress;

            if(progress >= targetProgress || progress % 20 != 0)
                return;

            m_achievementImage.sprite = gameplayAchievement.GetImageSprite();
            m_achievementProgressBarFill.fillAmount = progress / (float)targetProgress;
            m_achievementProgressText.text = $"{progress}/{targetProgress}";
            ShowUntil = Time.unscaledTime + 5f;
        }

        public override void Update()
        {
            base.Update();

            float deltaTime = Time.unscaledDeltaTime * 10f;
            m_canvasGroup.alpha = Mathf.Lerp(m_canvasGroup.alpha, shouldBeVisible ? 0.6f : 0f, deltaTime);
        }
    }
}
