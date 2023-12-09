using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Transform m_pageContentsTransform;

        [UIElement("ProgressText")]
        private Text m_progressText;
        [UIElement("ProgressFill")]
        private Image m_progressBarFill;

        [UIElement("AdvancementPrefab", false)]
        private ModdedObject m_displayPrefab;

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

            var manager = GameplayAchievementManager.Instance;
            if (!manager)
                return;

            float fractionOfAchievementsCompleted = manager.GetFractionOfAchievementsCompleted();
            m_progressBarFill.fillAmount = fractionOfAchievementsCompleted;
            m_progressText.text = Mathf.FloorToInt(fractionOfAchievementsCompleted * 100f) + "%";

            foreach (var achievement in manager.Achievements)
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
