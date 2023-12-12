using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    [UI(true)]
    public class UIAdvancementsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElementAction(nameof(OnSyncWthSteamButtonClicked))]
        [UIElement("SyncWithSteamButton")]
        private readonly Button m_syncWithSteamButton;

        [UIElement("Content")]
        private readonly Transform m_pageContentsTransform;

        [UIElement("ProgressText")]
        private readonly Text m_progressText;
        [UIElement("ProgressFill")]
        private readonly Image m_progressBarFill;

        [UIElement("AdvancementPrefab", false)]
        private readonly ModdedObject m_displayPrefab;

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void ClearPageContents()
        {
            if (m_pageContentsTransform && m_pageContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(m_pageContentsTransform);
        }

        public void Populate()
        {
            ClearPageContents();

            GameplayAchievementManager manager = GameplayAchievementManager.Instance;
            if (!manager)
                return;

            float fractionOfAchievementsCompleted = manager.GetFractionOfAchievementsCompleted();
            m_progressBarFill.fillAmount = fractionOfAchievementsCompleted;
            m_progressText.text = Mathf.FloorToInt(fractionOfAchievementsCompleted * 100f) + "%";

            foreach (GameplayAchievement achievement in manager.Achievements)
            {
                ModdedObject moddedObject = Instantiate(m_displayPrefab, m_pageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                UIElementAdvancementDisplay elementAdvancementDisplay = moddedObject.gameObject.AddComponent<UIElementAdvancementDisplay>();
                elementAdvancementDisplay.Populate(achievement, manager);
            }
        }

        public void OnSyncWthSteamButtonClicked()
        {
            if (ModGameUtils.SyncSteamAchievements())
            {
                Populate();
            }
        }
    }
}
