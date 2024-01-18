using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAdvancementsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElementAction(nameof(OnSyncWthSteamButtonClicked))]
        [UIElement("SyncWithSteamButton")]
        private readonly Button m_syncWithSteamButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button m_legacyUIButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("SearchBox")]
        private readonly InputField m_searchBox;

        [UIElement("ScrollRect")]
        private readonly ScrollRect m_scrollRect;
        [UIElement("GridContent")]
        private readonly Transform m_pageGridContentsTransform;
        [UIElement("VerticalContent")]
        private readonly Transform m_pageVerticalContentsTransform;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;
        [UIElement("ProgressText")]
        private readonly Text m_progressText;
        [UIElement("ProgressFill")]
        private readonly Image m_progressBarFill;

        [UIElement("AdvancementPrefab", false)]
        private readonly ModdedObject m_displayPrefab;
        [UIElement("GlobalAdvancementPrefab", false)]
        private readonly ModdedObject m_globalDisplayPrefab;

        [UIElement("MyAchTabButton")]
        private readonly ModdedObject m_localAdvancementsTab;
        [UIElement("GlbAchTabButton")]
        private readonly ModdedObject m_globalAdvancementsTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager m_tabs;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_tabs.AddTab(m_localAdvancementsTab, "local advancements");
            m_tabs.AddTab(m_globalAdvancementsTab, "global advancements");
            m_tabs.SelectTab("local advancements");

            ClearPageContents();
            PopulateLocalAchievements();
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool local = elementTab.tabId == "local advancements";

            ClearPageContents();
            SetContentLayout(local);
            if (local)
                PopulateLocalAchievements();
            else
                PopulateGlobalAchievments();

            m_syncWithSteamButton.interactable = local;
        }

        public void ClearPageContents()
        {
            if (m_pageGridContentsTransform && m_pageGridContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(m_pageGridContentsTransform);

            if (m_pageVerticalContentsTransform && m_pageVerticalContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(m_pageVerticalContentsTransform);
        }

        public void PopulateLocalAchievements()
        {
            GameplayAchievementManager manager = GameplayAchievementManager.Instance;
            if (!manager)
            {
                ModUIUtils.MessagePopupOK("Achievements get error", "why and how");
                return;
            }

            float fractionOfAchievementsCompleted = manager.GetFractionOfAchievementsCompleted();
            m_progressBarFill.fillAmount = fractionOfAchievementsCompleted;
            m_progressText.text = Mathf.FloorToInt(fractionOfAchievementsCompleted * 100f) + "%";

            foreach (GameplayAchievement achievement in manager.Achievements)
            {
                ModdedObject moddedObject = Instantiate(m_displayPrefab, m_pageGridContentsTransform);
                moddedObject.gameObject.SetActive(true);
                UIElementAdvancementDisplay elementAdvancementDisplay = moddedObject.gameObject.AddComponent<UIElementAdvancementDisplay>();
                elementAdvancementDisplay.Populate(achievement, manager);
            }
        }

        public void PopulateGlobalAchievments()
        {
            GameplayAchievementManager manager = GameplayAchievementManager.Instance;
            if (!manager)
            {
                ModUIUtils.MessagePopupOK("Achievements get error", "why and how");
                return;
            }

            m_tabs.interactable = false;
            m_loadingIndicator.SetActive(true);
            ModSteamUserStatsUtils.RefreshAllStats(delegate (bool result)
            {
                m_tabs.interactable = true;
                m_loadingIndicator.SetActive(false);
                if (!result)
                {
                    ModUIUtils.MessagePopupOK("Error", "Something went wrong while getting user statistics.", true);
                    return;
                }

                List<(GameplayAchievement, float)> list = new List<(GameplayAchievement, float)>();
                foreach (GameplayAchievement achievement in manager.Achievements)
                {
                    if (!achievement || (achievement.IsHidden && !achievement.IsComplete()) || !ModSteamUserStatsUtils.GetAchievementAchievedPercent(achievement.SteamAchievementID, out float percent))
                        continue;

                    list.Add((achievement, percent));
                }
                list = list.OrderBy(f => -f.Item2).ToList();

                if (list.IsNullOrEmpty())
                {
                    ModUIUtils.MessagePopupOK("Error", "Something went wrong while preparing statistics", true);
                    return;
                }

                foreach(var tuple in list)
                {
                    ModdedObject moddedObject = Instantiate(m_globalDisplayPrefab, m_pageVerticalContentsTransform);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(tuple.Item1.Name);
                    moddedObject.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString(tuple.Item1.Description);
                    moddedObject.GetObject<Image>(2).sprite = tuple.Item1.GetImageSprite();
                    moddedObject.GetObject<Image>(3).fillAmount = tuple.Item2 / 100f;
                    moddedObject.GetObject<Text>(4).text = $"{Mathf.Round(tuple.Item2)}%";
                    moddedObject.GetObject<GameObject>(5).SetActive(tuple.Item1.IsComplete());
                }
            });
        }

        public void SetContentLayout(bool grid)
        {
            m_pageGridContentsTransform.gameObject.SetActive(grid);
            m_pageVerticalContentsTransform.gameObject.SetActive(!grid);
            m_scrollRect.content = (grid ? m_pageGridContentsTransform : m_pageVerticalContentsTransform) as RectTransform;
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                Hide();
                titleScreenUI.OnAchievementsButtonClicked();
            }
        }

        public void OnSyncWthSteamButtonClicked()
        {
            ModUIUtils.MessagePopup(true, "Synchronize Steam achievements with game?", "Use this feature to set achievement progress based on your Steam account statistics.\nThis action cannot be undone.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, string.Empty, "Yes", "No", null, delegate
            {
                if (ModGameUtils.SyncSteamAchievements())
                {
                    ClearPageContents();
                    PopulateLocalAchievements();
                }
            });
        }
    }
}
