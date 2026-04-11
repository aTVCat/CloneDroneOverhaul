using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAdvancementsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElementAction(nameof(OnSyncWthSteamButtonClicked))]
        [UIElement("SyncWithSteamButton")]
        private readonly Button _syncWithSteamButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button _legacyUIButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("SearchBox")]
        private readonly InputField _searchBox;

        [UIElement("ScrollRect")]
        private readonly ScrollRect _scrollRect;
        [UIElement("GridContent")]
        private readonly Transform _pageGridContentsTransform;
        [UIElement("VerticalContent")]
        private readonly Transform _pageVerticalContentsTransform;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;
        [UIElement("ProgressText")]
        private readonly Text _progressText;
        [UIElement("ProgressFill")]
        private readonly Image _progressBarFill;

        [UIElement("AdvancementPrefab", false)]
        private readonly ModdedObject _displayPrefab;
        [UIElement("GlobalAdvancementPrefab", false)]
        private readonly ModdedObject _globalDisplayPrefab;
        [UIElement("GlobalAdvancementsTable", false)]
        private readonly ModdedObject _tablePrefab;
        [UIElement("AllAchievementsUnlockedLabel", false)]
        private readonly ModdedObject _allAchievementsUnlockedLabelPrefab;

        [UIElement("MyAchTabButton")]
        private readonly ModdedObject _localAdvancementsTab;
        [UIElement("GlbAchTabButton")]
        private readonly ModdedObject _globalAdvancementsTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager _tabs;

        public override bool HideTitleScreen => true;

        private bool _hasUpdatedLabels;

        protected override void OnInitialized()
        {
            ClearPageContents();

            _tabs.AddTab(_localAdvancementsTab.gameObject, "local advancements");
            _tabs.AddTab(_globalAdvancementsTab.gameObject, "global advancements");
            _tabs.SelectTab("local advancements");
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool local = elementTab.tabId == "local advancements";

            UIElementTab oldTab = _tabs.PreviousSelectedTab;
            UIElementTab newTab = _tabs.SelectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            ClearPageContents();
            SetContentLayout(local);
            if (local)
                PopulateLocalAchievements();
            else
                PopulateGlobalAchievments();

            _syncWithSteamButton.interactable = local;
        }

        public void ClearPageContents()
        {
            if (_pageGridContentsTransform && _pageGridContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(_pageGridContentsTransform);

            if (_pageVerticalContentsTransform && _pageVerticalContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(_pageVerticalContentsTransform);
        }

        public void PopulateLocalAchievements()
        {
            GameplayAchievementManager manager = GameplayAchievementManager.Instance;
            if (!manager)
            {
                ModUIUtils.MessagePopupOK("Achievements get error", "why and how");
                return;
            }

            float fraction = GameplayAchievementManager.Instance.GetFractionOfAchievementsCompleted();
            int percentage = Mathf.FloorToInt(fraction * 100f);

            if (!_hasUpdatedLabels)
            {
                _hasUpdatedLabels = true;
                _progressBarFill.fillAmount = fraction;
                _progressText.text = $"{ModGameUtils.GetNumOfAchievementsCompleted()}/{ModGameUtils.GetNumOfAchievements()} ({percentage}%)";
            }

            GridLayoutGroup gridLayoutGroup = _pageGridContentsTransform.GetComponent<GridLayoutGroup>();
            if (percentage >= 100f)
            {
                ModdedObject moddedObject1 = Instantiate(_allAchievementsUnlockedLabelPrefab, _pageGridContentsTransform);
                moddedObject1.gameObject.SetActive(true);

                RectTransform rectTransform = moddedObject1.transform as RectTransform;
                rectTransform.anchoredPosition = Vector2.zero;

                if (gridLayoutGroup)
                    gridLayoutGroup.padding.top = 50;
            }
            else if (gridLayoutGroup)
                gridLayoutGroup.padding.top = 8;

            foreach (GameplayAchievement achievement in manager.Achievements)
            {
                ModdedObject moddedObject = Instantiate(_displayPrefab, _pageGridContentsTransform);
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

            _tabs.IsInteractable = false;
            _loadingIndicator.SetActive(true);
            ModSteamUserStatsUtils.RefreshAllStats(delegate (bool result)
            {
                _tabs.IsInteractable = true;
                _loadingIndicator.SetActive(false);
                if (!result)
                {
                    ModUIUtils.MessagePopupOK("Error", "Something went wrong while getting user statistics.", true);
                    _tabs.SelectTab("local advancements");
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

                ModdedObject moddedObject1 = Instantiate(_tablePrefab, _pageVerticalContentsTransform);
                moddedObject1.gameObject.SetActive(true);

                foreach ((GameplayAchievement, float) tuple in list)
                {
                    ModdedObject moddedObject = Instantiate(_globalDisplayPrefab, _pageVerticalContentsTransform);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(tuple.Item1.Name);
                    moddedObject.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString(tuple.Item1.Description);
                    moddedObject.GetObject<Image>(2).sprite = tuple.Item1.GetImageSprite();
                    moddedObject.GetObject<Image>(3).fillAmount = tuple.Item2 / 100f;
                    moddedObject.GetObject<Text>(4).text = $"{Mathf.Round(tuple.Item2 * 10f) / 10f}%";
                    moddedObject.GetObject<GameObject>(5).SetActive(tuple.Item1.IsComplete());
                }
            });
        }

        public void SetContentLayout(bool grid)
        {
            _pageGridContentsTransform.gameObject.SetActive(grid);
            _pageVerticalContentsTransform.gameObject.SetActive(!grid);
            _scrollRect.content = (grid ? _pageGridContentsTransform : _pageVerticalContentsTransform) as RectTransform;
        }

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            if (GameModeManager.IsOnTitleScreen())
            {
                TitleScreenUI titleScreenUI = ModCache.TitleScreenUI;
                if (titleScreenUI)
                {
                    titleScreenUI.OnAchievementsButtonClicked();
                }
                return;
            }

            ModCache.UIRoot.AchievementProgressUI.Show();
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
