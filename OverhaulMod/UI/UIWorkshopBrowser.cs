using OverhaulMod.Content;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIWorkshopBrowser : OverhaulUIBehaviour
    {
        public const string ADVENTURE_LEVEL_TYPE_TAB = "Adventure";
        public const string CHALLENGE_LEVEL_TYPE_TAB = "Challenge";
        public const string ENDLESS_LEVEL_TYPE_TAB = "Endless Level";
        public const string LBS_LEVEL_TYPE_TAB = "Last Bot Standing Level";
        public const string TB_LEVEL_TYPE_TAB = "Team Battle Level";
        public const string COLLECTIONS_TYPE_TAB = "collections";

        public const string YOUR_LEVELS_SOURCE_TYPE = "user";
        public const string SUBSCRIPTIONS_SOURCE_TYPE = "subscriptions";
        public const string ALL_SOURCE_TYPE = "all";

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button _legacyUIButton;

        [UIElementAction(nameof(OnHistoryButtonClicked))]
        [UIElement("HistoryButton")]
        private readonly Button _historyButton;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnSourceTabSelected))]
        private readonly TabManager _sourceTabs;

        [UIElement("YourLevelsTab")]
        public GameObject _yourLevelsTab;
        [UIElement("SubscriptionsTab")]
        public GameObject _subscriptionsTab;
        [UIElementAction(nameof(OnBrowseButtonClicked))]
        [UIElement("BrowseTab")]
        public Button _browseTab;

        [UIElementAction(nameof(Populate))]
        [UIElement("ReloadButton")]
        public Button _reloadButton;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnQueryTabSelected))]
        private readonly TabManager _queryTabs;

        [UIElement("TrendingTab")]
        public GameObject _trendingTab;
        [UIElement("RecentTab")]
        public GameObject _recentTab;
        [UIElement("MostPopularTab")]
        public GameObject _mostPopularTab;
        [UIElement("MostSubscribersTab")]
        public GameObject _mostSubscribersTab;
        [UIElement("ByFollowedUsersTab")]
        public GameObject _byFollowedTab;
        [UIElement("ByFriendsTab")]
        public GameObject _byFriendsTab;
        [UIElement("FriendsFavoritesTab")]
        public GameObject _friendsFavoritesTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnLevelTypeTabSelected))]
        private readonly TabManager _levelTypeTabs;

        [UIElement("AdventuresTab")]
        public GameObject _adventuresTab;
        [UIElement("ChallengesTab")]
        public GameObject _challengesTab;
        [UIElement("EndlessLevelsTab")]
        public GameObject _endlessLevelsTab;
        [UIElement("LBSLevelsTab")]
        public GameObject _lastBotStandingLevelsTab;
        [UIElement("TeamBattleLevelsTab")]
        public GameObject _teamBattleLevelsTab;
        [UIElement("CollectionsTab")]
        public GameObject _collectionsTab;

        [UIElement("WorkshopItemDisplay", false)]
        public ModdedObject _workshopItemDisplay;
        [UIElement("WorkshopCollectionDisplay", false)]
        public ModdedObject _workshopCollectionDisplay;

        [UIElement("ScrollRect")]
        public ScrollRect _scrollRect;
        [UIElement("Content")]
        public Transform _gridContainer;
        [UIElement("VerticalContent")]
        public Transform _verticalContainer;
        [UIElement("Content")]
        public GridLayoutGroup _containerGridLayoutGroup;

        [UIElement("LoadingIndicator", false)]
        public GameObject _loadingIndicator;
        [UIElement("NothingToDisplayLabel", false)]
        public GameObject _nothingToDisplayLabel;

        [UIElement("Tabs")]
        public CanvasGroup _tabsCanvasGroup;

        [UIElement("BrowseItemsOfTypeDropdown", false)]
        public GameObject _browseItemsOfTypeDropdownObject;

        [UIElementAction(nameof(OnBrowseLevelsButtonClicked))]
        [UIElement("BrowseLevelsButton")]
        public Button _browseLevelsButton;
        [UIElement("BrowseLevelsSelectedIndicator", true)]
        public GameObject _browseLevelsSelectedIndicatorObject;

        [UIElementAction(nameof(OnBrowseCollectionsButtonClicked))]
        [UIElement("BrowseCollectionsButton")]
        public Button _browseCollectionsButton;
        [UIElement("BrowseCollectionsSelectedIndicator", false)]
        public GameObject _browseCollectionsSelectedIndicatorObject;

        [UIElementAction(nameof(OnPrevPageButtonClicked))]
        [UIElement("PrevPageButton")]
        private readonly Button _prevPageButton;
        [UIElementAction(nameof(OnNextPageButtonClicked))]
        [UIElement("NextPageButton")]
        private readonly Button _nextPageButton;
        [UIElementAction(nameof(OnPageButtonClicked))]
        [UIElement("CurrentPageButton")]
        private readonly Button _currentPageButton;
        [UIElement("CurrentPageText")]
        private readonly Text _currentPageText;

        [UIElement("PageDropdown", false)]
        public GameObject _pageDropdownObject;
        [UIElement("PageButton", false)]
        public ModdedObject _pageButtonPrefab;
        [UIElement("PageContainer")]
        public Transform _pageContainer;

        [UIElementAction(nameof(OnTypedSearchText))]
        [UIElement("SearchBox")]
        public InputField _searchBox;
        [UIElementAction(nameof(OnSearchButtonClicked))]
        [UIElement("SearchButton")]
        public Button _searchButton;
        [UIElementAction(nameof(OnClearButtonClicked))]
        [UIElement("ClearButton")]
        public Button _clearButton;
        [UIElementAction(nameof(OnBrowseLevelsButtonClicked))]
        [UIElement("SearchLevelsButton")]
        public Button _searchLevelsButton;

        [UIElement("SearchLevelsByTitleHolder", false)]
        public GameObject _searchLevelsByTitleHolderObject;
        [UIElement("SearchLevelsByTitleText")]
        public Text _searchLevelsByTitleText;

        [UIElement("SearchLevelsByUserHolder", false)]
        public GameObject _searchLevelsByUserHolderObject;
        [UIElement("SearchLevelsByUserText")]
        public Text _searchLevelsByUserText;

        [UIElementAction(nameof(OnHelpButtonClicked))]
        [UIElement("HelpButton")]
        public Button _controlsButton;
        [UIElement("ControlsPanel", false)]
        public GameObject _controlsPanel;

        [UIElement("ContextMenu", false)]
        private readonly RectTransform _contextMenu;
        [UIElement("ContextMenu", typeof(UIElementMouseEventsComponent))]
        private readonly UIElementMouseEventsComponent _contextMenuMouseChecker;

        [UIElementAction(nameof(OnContextMenuSubscribeButtonClicked))]
        [UIElement("ContextMenuSubscribeButton")]
        public Button _contextMenuSubscribeButton;
        [UIElementAction(nameof(OnContextMenuPlayButtonClicked))]
        [UIElement("ContextMenuPlayButton")]
        public Button _contextMenuPlayButton;

        [UIElement("QuickPreview", typeof(UIElementWorkshopItemQuickPreview), false)]
        private readonly UIElementWorkshopItemQuickPreview _quickPreview;

        [UIElementAction(nameof(OnBackButtonClicked))]
        [UIElement("BackButton")]
        public Button _backButton;

        [UIElement("MainBG")]
        public RectTransform _mainBG;

        [UIElement("ContentCategoryContainer")]
        public GameObject _tagsContainerObject;

        [UIElement("ViewFavoritesToggle", false)]
        public Toggle _viewFavoritesToggle;

        private List<UIElementWorkshopItemDisplay> _selectedItemDisplays;

        private Transform _container;

        public override bool HideTitleScreen => true;

        public bool BrowseCollections;

        public CSteamID ViewingUser;

        public PublishedFileId_t ViewingCollection;

        public int SourceType;

        public string SearchLevelType;

        public EUserUGCList SearchUserList;

        public EUGCQuery SearchQuery;

        public int Page;

        public string SearchText;

        private bool _steamInitialized, _initializedTabs, _getWorkshopItemsNextFrame, _isLoading;

        private float _timeLeftToPopulate;

        protected override void OnInitialized()
        {
            Page = 1;
            _timeLeftToPopulate = -1f;
            _selectedItemDisplays = new List<UIElementWorkshopItemDisplay>();

            _controlsButton.gameObject.SetActive(ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserContextMenu));
            ViewingCollection = default;

            _viewFavoritesToggle.isOn = false;
            _viewFavoritesToggle.onValueChanged.AddListener(delegate (bool b)
            {
                Populate();
            });
        }

        public override void Show()
        {
            base.Show();

            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                ModUIUtils.MessagePopupOK("Steam not initialized", "To browse Steam workshop you must have Steam connection established.", "Ok", Hide, 125f, true);
                return;
            }
            _steamInitialized = true;

            if (!_initializedTabs)
            {
                _levelTypeTabs.AddTab(_adventuresTab, ADVENTURE_LEVEL_TYPE_TAB);
                _levelTypeTabs.AddTab(_challengesTab, CHALLENGE_LEVEL_TYPE_TAB);
                _levelTypeTabs.AddTab(_endlessLevelsTab, ENDLESS_LEVEL_TYPE_TAB);
                _levelTypeTabs.AddTab(_lastBotStandingLevelsTab, LBS_LEVEL_TYPE_TAB);
                _levelTypeTabs.AddTab(_teamBattleLevelsTab, TB_LEVEL_TYPE_TAB);
                _levelTypeTabs.SelectTab(ADVENTURE_LEVEL_TYPE_TAB);

                _queryTabs.AddTab(_trendingTab, EUGCQuery.k_EUGCQuery_RankedByTrend.ToString());
                _queryTabs.AddTab(_recentTab, EUGCQuery.k_EUGCQuery_RankedByPublicationDate.ToString());
                _queryTabs.AddTab(_mostPopularTab, EUGCQuery.k_EUGCQuery_RankedByVote.ToString());
                _queryTabs.AddTab(_mostSubscribersTab, EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions.ToString());
                _queryTabs.AddTab(_byFollowedTab, EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate.ToString());
                _queryTabs.AddTab(_byFriendsTab, EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate.ToString());
                _queryTabs.AddTab(_friendsFavoritesTab, EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate.ToString());
                _queryTabs.SelectTab(EUGCQuery.k_EUGCQuery_RankedByTrend.ToString());

                _sourceTabs.AddTab(_yourLevelsTab, YOUR_LEVELS_SOURCE_TYPE);
                _sourceTabs.AddTab(_subscriptionsTab, SUBSCRIPTIONS_SOURCE_TYPE);
                _sourceTabs.SelectTab(ALL_SOURCE_TYPE);
                _initializedTabs = true;
            }

            Populate();
        }

        public override void Hide()
        {
            base.Hide();

            _timeLeftToPopulate = -1f;
            if (_gridContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_gridContainer);
        }

        public override void Update()
        {
            if (_getWorkshopItemsNextFrame)
                populate();

            if (_timeLeftToPopulate >= 0f)
            {
                _timeLeftToPopulate -= Time.unscaledDeltaTime;
                if (_timeLeftToPopulate <= 0f)
                {
                    Populate();
                }
            }

            if (Input.GetMouseButtonDown(0) && _contextMenu.gameObject.activeSelf && !_contextMenuMouseChecker.IsMouseOverElement)
            {
                ShowContextMenu(null);
            }
        }

        public void OnLevelTypeTabSelected(UIElementTab elementTab)
        {
            Page = 1;

            UIElementTab oldTab = _levelTypeTabs.PreviousSelectedTab;
            UIElementTab newTab = _levelTypeTabs.SelectedTab;
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

            SearchLevelType = elementTab.tabId;
            Populate();
        }

        public void OnQueryTabSelected(UIElementTab elementTab)
        {
            Page = 1;

            UIElementTab oldTab = _levelTypeTabs.PreviousSelectedTab;
            UIElementTab newTab = _levelTypeTabs.SelectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.x = 150f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.x = 160f;
                rt.sizeDelta = vector;
            }

            if (!Enum.TryParse(elementTab.tabId, out EUGCQuery result))
                result = EUGCQuery.k_EUGCQuery_RankedByTrend;

            SearchQuery = result;
            Populate();
        }

        public void OnSourceTabSelected(UIElementTab elementTab)
        {
            ViewingCollection = default;
            Page = 1;
            SearchText = null;

            CSteamID steamId = CSteamID.Nil;
            string tabId = elementTab.tabId;
            if (tabId == YOUR_LEVELS_SOURCE_TYPE)
            {
                SourceType = 2;
                SearchUserList = EUserUGCList.k_EUserUGCList_Published;
                steamId = SteamUser.GetSteamID();

                _tabsCanvasGroup.alpha = 0.25f;
                _tabsCanvasGroup.interactable = false;
                _viewFavoritesToggle.isOn = false;

                setBrowseItemType(false);
            }
            else if (tabId == SUBSCRIPTIONS_SOURCE_TYPE)
            {
                SourceType = 1;
                SearchUserList = EUserUGCList.k_EUserUGCList_Subscribed;
                steamId = SteamUser.GetSteamID();

                _tabsCanvasGroup.alpha = 0.25f;
                _tabsCanvasGroup.interactable = false;
                _viewFavoritesToggle.isOn = false;

                setBrowseItemType(false);
            }
            else
            {
                SourceType = 0;

                _tabsCanvasGroup.alpha = 1f;
                _tabsCanvasGroup.interactable = true;
            }
            ViewingUser = steamId;
            Populate();
        }

        public void Populate()
        {
            if (!_steamInitialized)
                return;

            _timeLeftToPopulate = -1f;
            _getWorkshopItemsNextFrame = !_isLoading;
        }

        private void populate()
        {
            _getWorkshopItemsNextFrame = false;
            setIsLoading(true);
            refreshTabContainers();
            refreshSearchBox();

            if (_gridContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_gridContainer);

            if (_verticalContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_verticalContainer);

            _selectedItemDisplays.Clear();

            if (SearchLevelType != COLLECTIONS_TYPE_TAB)
            {
                bool collections = BrowseCollections && ViewingCollection == default;
                setGridLayout(collections);

                ModSteamUGC.RequestParameters requestParameters = ModSteamUGC.RequestParameters.Create(collections ? EUGCMatchingUGCType.k_EUGCMatchingUGCType_Collections : EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items);
                requestParameters.EnableCaching();
                requestParameters.RequireTags(collections ? null : new List<string>() { SearchLevelType });
                requestParameters.ReturnPreviews();
                requestParameters.ReturnLongDescription();

                if (!SearchText.IsNullOrEmpty())
                    requestParameters.SearchText(SearchText);

                bool success;
                if (collections)
                {
                    success = ModSteamUGC.GetWorkshopItems(new PublishedFileId_t[]
                    {
                        (PublishedFileId_t)3345320549, // Apocalypse competition winners
                        (PublishedFileId_t)3045196841, // Imagine Chapter 6 Competition Winners
                        (PublishedFileId_t)2783921326, // Beginners Welcome Comp Entries - Clone Drone
                        (PublishedFileId_t)2669983696, // Zombie Adventure Competition Winners 2021
                        (PublishedFileId_t)2670197366, // LAUNCH Level Competition Winners
                        (PublishedFileId_t)2369933278, // Winter Level Editor Competition 2020
                    }, onGotItems, onError, null);
                }
                else
                {
                    if (ViewingCollection != default)
                    {
                        success = ModSteamUGC.GetWorkshopItem(ViewingCollection, onGotItem, onError, null);
                    }
                    else
                    {
                        success = SourceType == 0 ? ModSteamUGC.GetAllWorkshopItems(SearchQuery, Page, requestParameters, onGotItems, onError, null) : ModSteamUGC.GetWorkshopUserItemList(ViewingUser, Page, ViewingUser == SteamUser.GetSteamID() ? (_viewFavoritesToggle.isOn ? EUserUGCList.k_EUserUGCList_Favorited : SearchUserList) : SearchUserList, EUserUGCListSortOrder.k_EUserUGCListSortOrder_SubscriptionDateDesc, requestParameters, onGotItems, onError, null);
                    }
                }

                if (!success)
                {
                    onError("Internal error.");
                    setIsLoading(false);
                }
            }
        }

        private void onGotItems(List<WorkshopItem> list)
        {
            bool isEmpty = list.IsNullOrEmpty();
            setIsLoading(false);
            _nothingToDisplayLabel.SetActive(isEmpty);
            refreshContainer();

            if (isEmpty)
                return;

            bool collections = BrowseCollections && ViewingCollection == default;
            foreach (WorkshopItem workshopItem in list)
            {
                ModdedObject moddedObject = Instantiate(collections ? _workshopCollectionDisplay : _workshopItemDisplay, _container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = workshopItem.Name;
                UIElementWorkshopItemDisplay workshopItemDisplay = moddedObject.gameObject.AddComponent<UIElementWorkshopItemDisplay>();
                workshopItemDisplay.itemPageWindowParentTransform = base.transform;
                workshopItemDisplay.browserUI = this;
                workshopItemDisplay.isCollection = collections;
                workshopItemDisplay.InitializeAsElement();
                workshopItemDisplay.Populate(workshopItem);
            }
        }

        private void onGotItem(WorkshopItem workshopItem)
        {
            if (!ModSteamUGC.GetWorkshopItems(workshopItem.Children, onGotItems, onError, null))
            {
                onError("Internal error.");
                setIsLoading(false);
            }
        }

        private void onError(string error)
        {
            setIsLoading(false);

            ModUIUtils.MessagePopupOK("Could not get workshop items", error, "ok", Populate, 150f, true);
        }

        private void setGridLayout(bool collections)
        {
            GridLayoutGroup gridLayoutGroup = _containerGridLayoutGroup;
            if (collections)
            {
                gridLayoutGroup.cellSize = new Vector2((_gridContainer as RectTransform).rect.width - 24f, 100f);
            }
            else
            {
                gridLayoutGroup.cellSize = new Vector2(150f, 108f);
            }
        }

        private void setPageButtonsActive(bool value)
        {
            _prevPageButton.interactable = value;
            _nextPageButton.interactable = value;
            _currentPageButton.interactable = value;
        }

        private void setIsLoading(bool value)
        {
            _isLoading = value;

            bool searchByUser = SourceType != 0 && SearchUserList == EUserUGCList.k_EUserUGCList_Published;
            _searchLevelsByUserHolderObject.SetActive(searchByUser);
            _searchLevelsByUserText.text = searchByUser ? SteamFriends.GetFriendPersonaName(ViewingUser) : "none";

            bool searchByTitle = !SearchText.IsNullOrEmpty();
            _searchLevelsByTitleHolderObject.SetActive(!searchByUser && searchByTitle);
            _searchLevelsByTitleText.text = SearchText;
            _clearButton.interactable = !value && searchByTitle;

            if (SourceType != 0)
            {
                _browseCollectionsSelectedIndicatorObject.SetActive(false);
                _browseLevelsSelectedIndicatorObject.SetActive(false);
            }

            _viewFavoritesToggle.gameObject.SetActive(SourceType == 1 && ViewingUser == SteamUser.GetSteamID());
            _viewFavoritesToggle.interactable = !value;

            _backButton.interactable = !value;
            _reloadButton.interactable = !value;
            _sourceTabs.IsInteractable = !value;
            _levelTypeTabs.IsInteractable = !value && !BrowseCollections;
            _queryTabs.IsInteractable = !value && !BrowseCollections;
            _loadingIndicator.SetActive(value);
            _nothingToDisplayLabel.SetActive(false);
            _pageDropdownObject.SetActive(false);
            setPageButtonsActive(!value);
            refreshPagePageButton();
        }

        private void refreshPagePageButton()
        {
            bool notCollections = !BrowseCollections;

            int p = Page;
            _prevPageButton.gameObject.SetActive(notCollections && p > 1);
            _nextPageButton.gameObject.SetActive(notCollections && p < ModSteamUGC.pageCount);
            _currentPageButton.gameObject.SetActive(notCollections);
            _backButton.gameObject.SetActive(ViewingCollection != default);
            _currentPageText.text = p.ToString();
        }

        private void refreshContainer()
        {
            _container = BrowseCollections && ViewingCollection == default ? _verticalContainer : _gridContainer;
            _scrollRect.content = _container as RectTransform;
        }

        private void refreshTabContainers()
        {
            bool collections = BrowseCollections;
            _tagsContainerObject.SetActive(!collections);

            if (collections)
            {
                Vector2 vector = _mainBG.sizeDelta;
                vector.y = -55f;
                _mainBG.sizeDelta = vector;
            }
            else
            {
                Vector2 vector = _mainBG.sizeDelta;
                vector.y = -75f;
                _mainBG.sizeDelta = vector;
            }
        }

        private void refreshSearchBox()
        {
            InputField inputField = _searchBox;
            bool makeVisible = !BrowseCollections && SourceType == 0;

            if (!makeVisible && inputField.IsActive())
                inputField.DeactivateInputField();

            if (!makeVisible)
                inputField.text = string.Empty;

            _searchBox.gameObject.SetActive(makeVisible);
            _clearButton.gameObject.SetActive(makeVisible);
            _searchLevelsButton.gameObject.SetActive(!makeVisible);
        }

        private void setBrowseItemType(bool collections)
        {
            BrowseCollections = collections;
            _browseItemsOfTypeDropdownObject.SetActive(false);
            _browseCollectionsSelectedIndicatorObject.SetActive(collections);
            _browseLevelsSelectedIndicatorObject.SetActive(!collections);
        }

        public bool HideContextMenuIfShown()
        {
            if (_contextMenu.gameObject.activeSelf)
            {
                _contextMenu.gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        public void ShowContextMenu(UIElementWorkshopItemDisplay itemDisplay)
        {
            bool isNull = !itemDisplay;

            _contextMenu.gameObject.SetActive(!isNull);
            if (!isNull)
                _contextMenu.position = itemDisplay.transform.position;

            _contextMenuSubscribeButton.gameObject.SetActive(true);
            _contextMenuPlayButton.gameObject.SetActive(_selectedItemDisplays.Count == 1);
        }

        public void SetItemSelected(UIElementWorkshopItemDisplay itemDisplay, bool value)
        {
            if (value)
            {
                if (!_selectedItemDisplays.Contains(itemDisplay))
                    _selectedItemDisplays.Add(itemDisplay);
            }
            else
            {
                _ = _selectedItemDisplays.Remove(itemDisplay);
            }
        }

        public bool IsItemSelected(UIElementWorkshopItemDisplay itemDisplay)
        {
            return _selectedItemDisplays.Contains(itemDisplay);
        }

        public void QuickPreview(WorkshopItem workshopItem)
        {
            if (workshopItem == null)
            {
                _quickPreview.gameObject.SetActive(false);
                return;
            }
            _quickPreview.gameObject.SetActive(true);
            _quickPreview.Populate(workshopItem);
        }

        public void OnHelpButtonClicked()
        {
            _controlsPanel.SetActive(!_controlsPanel.activeSelf);
        }

        public void OnTypedSearchText(string text)
        {
            SearchText = text;
            _timeLeftToPopulate = 1f;
        }

        public void OnSearchButtonClicked()
        {
            SearchText = _searchBox.text;
            Populate();
        }

        public void OnClearButtonClicked()
        {
            _searchBox.text = string.Empty;
            SearchText = null;
            Populate();
        }

        public void OnPageButtonClicked()
        {
            bool active = !_pageDropdownObject.activeSelf && ModSteamUGC.pageCount > 1;
            _pageDropdownObject.SetActive(active);

            if (_pageContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_pageContainer);

            if (!active)
                return;

            for (int i = 1; i < ModSteamUGC.pageCount + 1; i++)
            {
                int pageIndex = i;
                ModdedObject pageObject = Instantiate(_pageButtonPrefab, _pageContainer);
                pageObject.gameObject.SetActive(true);
                pageObject.GetObject<Text>(0).text = i.ToString();
                pageObject.GetObject<GameObject>(1).SetActive(i == Page);
                Button button = pageObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    Page = pageIndex;
                    Populate();
                });
            }
        }

        public void OnNextPageButtonClicked()
        {
            Page++;
            Populate();
        }

        public void OnPrevPageButtonClicked()
        {
            Page--;
            Populate();
        }

        public void OnBrowseButtonClicked()
        {
            _browseItemsOfTypeDropdownObject.SetActive(!_browseItemsOfTypeDropdownObject.activeSelf);
        }

        public void OnBrowseLevelsButtonClicked()
        {
            if (_isLoading)
                return;

            setBrowseItemType(false);

            ViewingCollection = default;
            Page = 1;
            SourceType = 0;
            _tabsCanvasGroup.alpha = 1f;
            _tabsCanvasGroup.interactable = true;
            _sourceTabs.DeselectAllTabs();

            Populate();
        }

        public void OnBrowseCollectionsButtonClicked()
        {
            if (_isLoading)
                return;

            setBrowseItemType(true);

            ViewingCollection = default;
            Page = 1;
            SourceType = 0;
            _tabsCanvasGroup.alpha = 0.25f;
            _tabsCanvasGroup.interactable = false;
            _sourceTabs.DeselectAllTabs();

            Populate();
        }

        public void OnContextMenuSubscribeButtonClicked()
        {

        }

        public void OnContextMenuPlayButtonClicked()
        {

        }

        public void OnBackButtonClicked()
        {
            if (_isLoading)
                return;

            ViewingCollection = default;
            Populate();
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.TitleScreenUI;
            if (titleScreenUI)
            {
                Hide();
                titleScreenUI.OnWorkshopBrowserButtonClicked();
            }
        }

        public void OnHistoryButtonClicked()
        {
            ModUIs.ShowWorkshopBrowserHistoryPanel(base.transform);
        }
    }
}
