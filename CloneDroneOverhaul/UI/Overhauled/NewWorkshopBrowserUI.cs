using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.UI.Components;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewWorkshopBrowserUI : ModGUIBase
    {
        public static NewWorkshopBrowserUI Instance;
        private ItemDetailsUI _itemDetailsUI;
        private Pages _pages;
        private bool _hasPopulatedTabs;

        public static readonly string[] TAB_IDS = new string[]
        {
            "Featured",
            "Most popular",
            "Trending",
            "Latest",
            "Followed authors",
            "Subscriptions",
            "Made by friends",
            "Friends favourite",
        };

        public static readonly string[] LEVEL_TYPES = new string[]
        {
            "Endless Level",
            "Challenge",
            "Adventure",
            "Last Bot Standing Level",
        };

        public const string TAB_SELECTED_COLOR_HEX = "#3C9C63";
        public const string TAB_DEFAULT_COLOR_HEX = "#45474F";

        public const string TEMPORAL_PREFIX = "WorkshopBrowser_CDO_";

        private bool _canRereshItems;
        private int _selectedLeveltype;
        private int _currentPage = 1;
        private string _selectedTabID;

        private List<ItemDisplay> _spawnedItems = new List<ItemDisplay>();
        private List<string> _tabButtonImages_IDs = new List<string>();

        private SteamWorkshopItem _selectedItem;

        private BrowserState _currentState = BrowserState.Idle;

        private bool _hasShownEarlier;
        private bool _hasToRefreshTabs = true;

        public override void OnInstanceStart()
        {
            Instance = this;
            base.MyModdedObject = base.GetComponent<ModdedObject>();

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(Hide);

            _itemDetailsUI = MyModdedObject.GetObjectFromList<ModdedObject>(8).gameObject.AddComponent<ItemDetailsUI>();
            _itemDetailsUI.Initialize(this);

            _pages = Pages.Initialize(this, MyModdedObject.GetObjectFromList<ModdedObject>(13));

            Hide();
        }

        public override void RunFunction(string name, object[] arguments)
        {
            if (name == "onLanguageChanged")
            {
                _hasToRefreshTabs = true;
            }
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            populateTabsAndLevelTypeDropdowns();

            _canRereshItems = true;

            if (!_hasShownEarlier)
            {
                _hasShownEarlier = true;
                _selectedTabID = TAB_IDS[2];
                _selectedLeveltype = 0;
                MyModdedObject.GetObjectFromList<Text>(1).text = "";
                SetPage(1);
                this.PopulateWorkshopItems();
            }

            MyModdedObject.GetObjectFromList<Text>(15).text = OverhaulMain.GetTranslatedString("WBUI_Title");
            _pages.GetComponent<ModdedObject>().GetObjectFromList<Text>(9).text = OverhaulMain.GetTranslatedString("WBUI_Pages_Header");
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);

            if (GameModeManager.IsOnTitleScreen())
            {
                ArenaManager.SetRootAndLogoVisible(true);
            }
        }

        public void SetState(BrowserState newState)
        {
            _currentState = newState;

            switch (newState)
            {
                case BrowserState.Unknown:
                    MyModdedObject.GetObjectFromList<Text>(14).text = "";
                    break;
                case BrowserState.Idle:
                    MyModdedObject.GetObjectFromList<Text>(14).text = "";
                    break;
                case BrowserState.LoadingItems:
                    MyModdedObject.GetObjectFromList<Text>(14).text = "Loading...";
                    break;
                case BrowserState.InitializingItems:
                    MyModdedObject.GetObjectFromList<Text>(14).text = "Finishing...";
                    break;
            }
        }

        /// <summary>
        /// Get selected level
        /// </summary>
        /// <returns></returns>
        public SteamWorkshopItem GetSelectedWorkshopItem()
        {
            return _selectedItem;
        }

        /// <summary>
        /// 0 - Endless, 1 - Challenge, 2 - Adventure, 3 - Last Bot Standing
        /// </summary>
        /// <returns></returns>
        public int GetCurrentLevelType()
        {
            return _selectedLeveltype;
        }

        public bool AllowSwitching()
        {
            return _canRereshItems && _currentState != BrowserState.LoadingItems && _currentState != BrowserState.InitializingItems;
        }

        public void SetSelectedWorkshopItem(SteamWorkshopItem item)
        {
            _selectedItem = item;
        }
        public void SetPage(int pageNum)
        {
            _currentPage = pageNum;
        }

        /// <summary>
        /// Populates all tabs which decide, what items we want from
        /// </summary>
        void populateTabsAndLevelTypeDropdowns()
        {
            if (_hasPopulatedTabs && !_hasToRefreshTabs)
            {
                return;
            }
            _canRereshItems = false;
            _hasPopulatedTabs = true;
            _hasToRefreshTabs = false;

            if (_tabButtonImages_IDs.Count > 0)
            {
                foreach (string str in _tabButtonImages_IDs)
                {
                    OverhaulCacheManager.RemoveTemporalObject(str);
                    OverhaulCacheManager.RemoveTemporalObject(str.Replace("ImageComp_", string.Empty));
                }
            }
            _tabButtonImages_IDs.Clear();

            TransformUtils.DestroyAllChildren(MyModdedObject.GetObjectFromList<Transform>(3));

            foreach (string tabName in TAB_IDS)
            {
                ModdedObject mObj = Instantiate<ModdedObject>(MyModdedObject.GetObjectFromList<ModdedObject>(2), MyModdedObject.GetObjectFromList<Transform>(3));
                mObj.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("WBUI_Tab_" + tabName);

                ReferenceOnClick.CallOnClickWithReference(mObj.gameObject, onTabSelected);

                string str1 = TEMPORAL_PREFIX + mObj.gameObject.GetInstanceID();
                OverhaulCacheManager.AddTemporalObject<string>(tabName, str1);

                string str2 = TEMPORAL_PREFIX + "ImageComp_" + mObj.gameObject.GetInstanceID();
                OverhaulCacheManager.AddTemporalObject<Image>(mObj.GetComponent<Image>(), str2);
                mObj.GetComponent<Image>().color = BaseUtils.ColorFromHex(TAB_DEFAULT_COLOR_HEX);
                _tabButtonImages_IDs.Add(str2);

                mObj.gameObject.SetActive(true);
            }

            MyModdedObject.GetObjectFromList<Dropdown>(4).options.Clear();
            foreach (string itemType in LEVEL_TYPES)
            {
                MyModdedObject.GetObjectFromList<Dropdown>(4).options.Add(new Dropdown.OptionData(itemType));
            }
            MyModdedObject.GetObjectFromList<Dropdown>(4).onValueChanged.AddListener(onSelectedLevelType);
            _canRereshItems = true;
        }

        /// <summary>
        /// Calls when we dropdown value changes
        /// </summary>
        /// <param name="value"></param>
        void onSelectedLevelType(int value)
        {
            if (!AllowSwitching())
            {
                return;
            }
            SetPage(1);
            _selectedLeveltype = value;
            PopulateWorkshopItems();
            if (value == 0)
            {
                MyModdedObject.GetObjectFromList<Text>(1).text = LocalizationManager.Instance.GetTranslatedString("endlessLevelsDescription");
            }
            else if (value == 3)
            {
                MyModdedObject.GetObjectFromList<Text>(1).text = LocalizationManager.Instance.GetTranslatedString("lbsLevelsDescription");
            }
            else
            {
                MyModdedObject.GetObjectFromList<Text>(1).text = "";
            }
        }

        /// <summary>
        /// Calls when we click tab button
        /// </summary>
        /// <param name="tabText"></param>
        void onTabSelected(MonoBehaviour tab)
        {
            if (!AllowSwitching())
            {
                return;
            }
            SetPage(1);
            foreach (string str in _tabButtonImages_IDs)
            {
                Image image = OverhaulCacheManager.GetTemporalObject<Image>(str);
                image.color = BaseUtils.ColorFromHex(TAB_DEFAULT_COLOR_HEX);
            }
            _selectedTabID = OverhaulCacheManager.GetTemporalObject<string>(TEMPORAL_PREFIX + tab.gameObject.GetInstanceID());
            OverhaulCacheManager.GetTemporalObject<Image>(TEMPORAL_PREFIX + "ImageComp_" + tab.gameObject.GetInstanceID()).color = BaseUtils.ColorFromHex(TAB_SELECTED_COLOR_HEX);
            PopulateWorkshopItems();
        }

        /// <summary>
        /// Gets workshop items by choosen level type and tab id
        /// </summary>
        /// <param name="selectLevelType"></param>
        /// <param name="tabID"></param>
        /// <returns></returns>
        public List<SteamWorkshopItem> GetWorkshopItems(int selectLevelType, string tabID, int page, EUGCQuery queryType, System.Action<List<SteamWorkshopItem>> onReceive = null)
        {
            List<SteamWorkshopItem> result = new List<SteamWorkshopItem>();
            if (tabID == TAB_IDS[5])
            {
                Singleton<SteamWorkshopManager>.Instance.GetWorkshopItems(LEVEL_TYPES[selectLevelType], page, delegate (List<SteamWorkshopItem> items)
                {
                    result = items;
                    if (onReceive != null)
                    {
                        onReceive(items);
                    }
                });
                return result;
            }
            Singleton<SteamWorkshopManager>.Instance.GetAllNewWorkshopItems(LEVEL_TYPES[selectLevelType], queryType, page, delegate (List<SteamWorkshopItem> items)
            {
                result = items;
                if (onReceive != null)
                {
                    onReceive(items);
                }
            });
            return result;
        }

        /// <summary>
        /// Gets EUGCQuery by tab ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EUGCQuery EUGCQueryByTabID(string tabID)
        {
            EUGCQuery query = EUGCQuery.k_EUGCQuery_RankedByTrend;

            if (tabID == TAB_IDS[1])
            {
                query = EUGCQuery.k_EUGCQuery_RankedByVote;
            }
            else if (tabID == TAB_IDS[2])
            {
                query = EUGCQuery.k_EUGCQuery_RankedByTrend;
            }
            else if (tabID == TAB_IDS[3])
            {
                query = EUGCQuery.k_EUGCQuery_RankedByPublicationDate;
            }
            else if (tabID == TAB_IDS[4])
            {
                query = EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate;
            }
            else if (tabID == TAB_IDS[6])
            {
                query = EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate;
            }
            else if (tabID == TAB_IDS[7])
            {
                query = EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate;
            }

            return query;
        }

        /// <summary>
        /// Populates workshop items
        /// </summary>
        public void PopulateWorkshopItems()
        {
            if (!_canRereshItems)
            {
                return;
            }

            clearExistingItems();

            SetState(BrowserState.LoadingItems);
            GetWorkshopItems(_selectedLeveltype, _selectedTabID, _currentPage, EUGCQueryByTabID(_selectedTabID), delegate (List<SteamWorkshopItem> items)
            {
                SetState(BrowserState.InitializingItems);
                base.StartCoroutine(asyncPopulateWorkshopItems(items));
                _pages.UpdatePages();
            });

        }

        IEnumerator asyncPopulateWorkshopItems(List<SteamWorkshopItem> items)
        {
            foreach (SteamWorkshopItem item in items)
            {
                yield return new WaitForEndOfFrame();

                if (!_canRereshItems)
                {
                    SetState(BrowserState.Idle);
                    yield break;
                }

                ModdedObject itemMObject = Instantiate<ModdedObject>(MyModdedObject.GetObjectFromList<ModdedObject>(6), MyModdedObject.GetObjectFromList<Transform>(7));
                itemMObject.GetObjectFromList<Text>(1).text = item.Title;
                bool hasCompleted = ChallengeManager.Instance.HasCompletedChallenge(item.WorkshopItemID.ToString());
                itemMObject.GetObjectFromList<Transform>(2).gameObject.SetActive(hasCompleted);
                itemMObject.gameObject.AddComponent<ItemDisplay>().Initialize(this, item);

                itemMObject.gameObject.SetActive(true);
            }
            SetState(BrowserState.Idle);

            yield break;
        }

        /// <summary>
        /// Clears spawned items
        /// </summary>
        void clearExistingItems()
        {
            base.StopAllCoroutines();
            TransformUtils.DestroyAllChildren(MyModdedObject.GetObjectFromList<Transform>(7));
            _pages.SetPageViewActive(false);
        }

        /// <summary>
        /// Calls when ItemDisplay is created
        /// </summary>
        /// <param name="display"></param>
        public void AddItemToList(ItemDisplay display)
        {
            if (!_spawnedItems.Contains(display)) _spawnedItems.Add(display);
        }

        /// <summary>
        /// Calls when ItemDisplay is destroyed
        /// </summary>
        /// <param name="display"></param>
        public void RemoveItemFromList(ItemDisplay display)
        {
            if (_spawnedItems.Contains(display)) _spawnedItems.Remove(display);
        }

        /// <summary>
        /// ItemDisplay.SetSelected(false);
        /// </summary>
        public void DeselectAllItems()
        {
            foreach (ItemDisplay disp in _spawnedItems)
            {
                disp.SetSelected(false);
            }
        }

        /// <summary>
        /// Refereshes info about selected item when we click on it
        /// </summary>
        /// <param name="display"></param>
        public bool OnItemClicked(ItemDisplay display)
        {
            if (display == null || display.SteamItem == null || _itemDetailsUI == null)
            {
                return false;
            }
            _selectedItem = display.SteamItem;
            _itemDetailsUI.PopulateItemInfo(_selectedItem, display.ImageLoader.ImageComponent.sprite);
            return true;
        }

        /// <summary>
        /// This is used for workshop items in list
        /// </summary>
        public class ItemDisplay : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
        {
            private NewWorkshopBrowserUI _browserReference;
            public SteamWorkshopItem SteamItem;
            private ModdedObject _moddedObject;
            public ImageLoader ImageLoader;

            public void Initialize(NewWorkshopBrowserUI browser, SteamWorkshopItem item)
            {
                SteamItem = item;

                _browserReference = browser;
                _browserReference.AddItemToList(this);

                _moddedObject = base.GetComponent<ModdedObject>();

                ImageLoader = _moddedObject.GetObjectFromList<Transform>(0).gameObject.AddComponent<ImageLoader>();
                ImageLoader.LoadImage(item.PreviewURL);

                SetSelected(false);
            }

            void OnDestroy()
            {
                _browserReference.RemoveItemFromList(this);
            }

            void UnityEngine.EventSystems.IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
            {
                if (_browserReference.OnItemClicked(this))
                {
                    _browserReference.DeselectAllItems();
                    SetSelected(true);
                }
            }

            public void SetSelected(bool value)
            {
                _moddedObject.GetObjectFromList<Image>(3).enabled = value;
            }
        }

        /// <summary>
        /// Used for displaying item details, like description and author
        /// </summary>
        public class ItemDetailsUI : MonoBehaviour
        {
            public const string NoItemsSelectedColor = "#292A30";
            public const string ItemsSelectedColor = "#108424";

            NewWorkshopBrowserUI _browser;

            ImageLoader _imageLoader;
            ModdedObject _moddedObject;

            Transform _optionsMenuTransform;
            Button _steamPageButton;
            Button _resetProgress;

            Text _itemTitle;
            InputField _itemDesc;
            Text _itemMadeBy;

            Text _goToGameMode;

            Button _playButton;
            Button _subscribeButton;
            Button _optionsButton;
            Slider _downloadProgressSlider;

            Text _downloadState;

            private bool _isDownloading;
            private bool _showingOptions;
            private bool _selectedLevelHasSavedData;

            public void Initialize(NewWorkshopBrowserUI browser)
            {
                _browser = browser;

                base.GetComponent<Image>().color = BaseUtils.ColorFromHex(NoItemsSelectedColor);

                _moddedObject = base.GetComponent<ModdedObject>();
                _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(false);

                _imageLoader = _moddedObject.GetObjectFromList<ModdedObject>(5).gameObject.AddComponent<ImageLoader>();
                _optionsMenuTransform = _moddedObject.GetObjectFromList<Transform>(2);
                _optionsMenuTransform.gameObject.SetActive(false);

                _steamPageButton = _moddedObject.GetObjectFromList<Button>(16);
                _steamPageButton.onClick.AddListener(viewItemSteamPage);
                _resetProgress = _moddedObject.GetObjectFromList<Button>(15);
                _resetProgress.onClick.AddListener(resetProgress);

                _itemTitle = _moddedObject.GetObjectFromList<Text>(3);
                _itemMadeBy = _moddedObject.GetObjectFromList<Text>(4);
                _itemDesc = _moddedObject.GetObjectFromList<InputField>(6);

                _goToGameMode = _moddedObject.GetObjectFromList<Text>(10);

                _optionsButton = _moddedObject.GetObjectFromList<Button>(1);
                _optionsButton.onClick.AddListener(toggleOptionsMenu);

                _playButton = _moddedObject.GetObjectFromList<Button>(0);
                _playButton.onClick.AddListener(delegate
                {
                    if (_browser.GetCurrentLevelType() != 0 && _browser.GetCurrentLevelType() != 3) // If challange or adventure // TODO - transition
                    {
                        if (Singleton<WorkshopChallengeManager>.Instance.StartChallengeFromWorkshop(_browser.GetSelectedWorkshopItem()))
                        {
                            _browser.Hide();
                        }
                    }
                    else if (_browser.GetCurrentLevelType() == 0) // If endless level
                    {
                        _playButton.gameObject.SetActive(false);
                        _goToGameMode.gameObject.SetActive(true);
                        _goToGameMode.text = "Start Endless mode?";
                        _goToGameMode.GetComponent<Button>().onClick.RemoveAllListeners();
                        _goToGameMode.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            _browser.Hide();
                            GameFlowManager.Instance.StartEndlessModeGame();
                        });
                    }
                    else if (_browser.GetCurrentLevelType() == 3) // If last bot standing level
                    {
                        _playButton.gameObject.SetActive(false);
                        _goToGameMode.gameObject.SetActive(true);
                        _goToGameMode.text = "Start Last Bot Standing Match?";
                        _goToGameMode.GetComponent<Button>().onClick.RemoveAllListeners();
                        _goToGameMode.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            _browser.Hide();
                            GameUIRoot.Instance.TitleScreenUI.MultiplayerModeSelectScreen.Show();
                            GameUIRoot.Instance.TitleScreenUI.MultiplayerModeSelectScreen.GameModeData[2].ClickedCallback.Invoke();
                        });
                    }
                });

                _subscribeButton = _moddedObject.GetObjectFromList<Button>(12);
                _subscribeButton.onClick.AddListener(subscribeToItem);

                _downloadProgressSlider = _moddedObject.GetObjectFromList<Slider>(11);

                _downloadState = _moddedObject.GetObjectFromList<Text>(13);
            }

            public void PopulateItemInfo(SteamWorkshopItem item, Sprite sprite)
            {
                if (item == null)
                {
                    return;
                }

                _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(true);
                base.GetComponent<Image>().color = BaseUtils.ColorFromHex(ItemsSelectedColor);

                _itemTitle.text = item.Title;
                _itemDesc.text = item.Description;
                _itemMadeBy.text = item.CreatorName;
                if (sprite == null || sprite.name == "placeholderLoad")
                {
                    _imageLoader.LoadImage(item.PreviewURL);
                }
                else
                {
                    _imageLoader.SetImage(sprite);
                }

                _selectedLevelHasSavedData = false;
                if (_browser.GetCurrentLevelType() == 1 || _browser.GetCurrentLevelType() == 2)
                {
                    if (System.IO.File.Exists(DataRepository.Instance.GetFullPath("ChallengeData" + this._browser.GetSelectedWorkshopItem().WorkshopItemID + ".json", false)))
                    {
                        _selectedLevelHasSavedData = true;
                    }
                }

                RefreshPlayButtons();
                refreshIsDownloading();
                setOptionsVisible(false);
            }

            public void RefreshPlayButtons()
            {
                _goToGameMode.gameObject.SetActive(false);
                SteamWorkshopItem item = _browser.GetSelectedWorkshopItem();
                if (item == null)
                {
                    return;
                }

                bool isSubscribed = !string.IsNullOrEmpty(item.Folder);

                if (BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(item) != -1)
                {
                    _playButton.gameObject.SetActive(false);
                    _subscribeButton.gameObject.SetActive(false);
                    _downloadProgressSlider.gameObject.SetActive(true);
                    refreshIsDownloading(true);
                    return;
                }

                _playButton.gameObject.SetActive(isSubscribed);
                _subscribeButton.gameObject.SetActive(!isSubscribed);

                if (_browser.GetCurrentLevelType() == 0 && SettingsManager.Instance.GetWorkshopInEndlessPolicy() == WorkshopinEndlessPolicy.NoWorkshop)
                {
                    hideAllButtons();
                }

                _downloadProgressSlider.gameObject.SetActive(false);
            }
            private void hideAllButtons()
            {
                _playButton.gameObject.SetActive(false);
                _subscribeButton.gameObject.SetActive(false);
                _downloadProgressSlider.gameObject.SetActive(false);
                _goToGameMode.gameObject.SetActive(false);
            }

            private void refreshIsDownloading(bool isCalledDuringDownload = false)
            {
                if (!isCalledDuringDownload) _isDownloading = false;
                SteamWorkshopItem item = _browser.GetSelectedWorkshopItem();
                if (item == null)
                {
                    return;
                }
                float percentage = BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(item);
                if (!isCalledDuringDownload) _isDownloading = percentage != -1;

                if (!isCalledDuringDownload) if (!_isDownloading)
                    {
                        return;
                    }

                _playButton.gameObject.SetActive(false);
                _subscribeButton.gameObject.SetActive(false);
                _downloadProgressSlider.gameObject.SetActive(true);

                if ((_isDownloading || isCalledDuringDownload) && percentage != -1)
                {
                    _downloadProgressSlider.value = percentage;
                }

                if (percentage == -1)
                {
                    _downloadState.text = "Waiting download to start...";
                }
                else
                {
                    _downloadState.text = "Downloading";
                }
            }

            private void subscribeToItem()
            {
                SteamWorkshopItem item = _browser.GetSelectedWorkshopItem();
                if (item != null)
                {
                    SteamWorkshopManager.Instance.SubscribeToItem(item.WorkshopItemID, delegate (SteamWorkshopItem itemOut)
                    {
                        _browser.SetSelectedWorkshopItem(itemOut);
                        _isDownloading = false;
                        RefreshPlayButtons();
                        new Notifications.Notification().SetUp("Download success!", itemOut.Title + " was downloaded", 6f, Vector2.zero, Color.clear, null);
                    }, delegate (string error)
                    {
                        _isDownloading = false;
                        RefreshPlayButtons();
                        new Notifications.Notification().SetUp("Download error", error, 3f, new Vector2(450, 75), Color.red, null);
                    });

                    _isDownloading = true;
                    _downloadProgressSlider.value = 0;
                    _subscribeButton.gameObject.SetActive(false);
                    _downloadProgressSlider.gameObject.SetActive(true);
                }
            }

            private void toggleOptionsMenu()
            {
                setOptionsVisible(!_showingOptions);
            }
            private void setOptionsVisible(bool value)
            {
                _showingOptions = value;
                _optionsMenuTransform.gameObject.SetActive(_showingOptions);
                _resetProgress.interactable = _selectedLevelHasSavedData;
            }
            void viewItemSteamPage()
            {
                BaseUtils.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + this._browser.GetSelectedWorkshopItem().WorkshopItemID);
            }
            void resetProgress()
            {
                if (!_selectedLevelHasSavedData)
                {
                    return;
                }
                _selectedLevelHasSavedData = false;
                setOptionsVisible(true);
                System.IO.File.Delete(DataRepository.Instance.GetFullPath("ChallengeData" + this._browser.GetSelectedWorkshopItem().WorkshopItemID + ".json", false));
            }

            private void Update()
            {
                if (_isDownloading)
                {
                    refreshIsDownloading(true);
                }
            }
        }

        /// <summary>
        /// Page selector
        /// </summary>
        public class Pages : MonoBehaviour
        {
            NewWorkshopBrowserUI _browser;
            ModdedObject _moddedObject;
            public static Pages Initialize(NewWorkshopBrowserUI browser, ModdedObject moddedObject)
            {
                Pages pages = moddedObject.gameObject.AddComponent<Pages>();
                pages._browser = browser;
                pages._moddedObject = moddedObject;

                pages._nextPage = moddedObject.GetObjectFromList<Button>(1);
                pages._nextPage.onClick.AddListener(pages.NextPage);
                pages._prevPage = moddedObject.GetObjectFromList<Button>(0);
                pages._prevPage.onClick.AddListener(pages.PrevPage);
                pages._choosePageButton = moddedObject.GetObjectFromList<Button>(4);
                pages._choosePageButton.onClick.AddListener(pages.togglePageView);

                pages._firstPage = moddedObject.GetObjectFromList<Button>(2);
                pages._firstPageText = moddedObject.GetObjectFromList<Text>(2);
                pages._lastPage = moddedObject.GetObjectFromList<Button>(3);
                pages._lastPageText = moddedObject.GetObjectFromList<Text>(3);
                pages._currentPageText = moddedObject.GetObjectFromList<Text>(5);

                pages._pageViewMain = moddedObject.GetObjectFromList<Transform>(6);
                pages._pageViewContainer = moddedObject.GetObjectFromList<Transform>(7);
                pages._pageViewEntry = moddedObject.GetObjectFromList<Button>(8);



                return pages;
            }

            private int _query;
            private bool _pageViewActive;

            // Next and prev pages button
            Button _nextPage;
            Button _prevPage;
            Button _choosePageButton;

            // First and last pages config
            Button _firstPage;
            Text _firstPageText;
            Button _lastPage;
            Text _lastPageText;
            Text _currentPageText;

            // Page view
            Transform _pageViewMain;
            Button _pageViewEntry;
            Transform _pageViewContainer;

            /// <summary>
            /// Updates pages
            /// </summary>
            /// <param name="query"></param>
            public void UpdatePages()
            {
                this._query = SteamWorkshopManager.Instance.GetNumPagesForLastQuery();
                this.updateFirstAndLastPageButtons();
                this.updateNextAndPrevPageButtons();
                _currentPageText.text = _browser._currentPage.ToString();
            }
            public void SetPage(int page)
            {
                if (!_browser.AllowSwitching())
                {
                    return;
                }
                _browser.SetPage(page);
                this.updateNextAndPrevPageButtons();
                _browser.PopulateWorkshopItems();
                _currentPageText.text = page.ToString();
            }
            public void NextPage()
            {
                if (_browser._currentPage + 1 > _query)
                {
                    return;
                }
                SetPage(_browser._currentPage + 1);
            }
            public void PrevPage()
            {
                if (_browser._currentPage - 1 == 0)
                {
                    return;
                }
                SetPage(_browser._currentPage - 1);
            }
            private void updateFirstAndLastPageButtons()
            {
                _firstPageText.text = "1";
                PageButton.Initialize(_firstPage, SetPage, 1);
                _lastPageText.text = _query.ToString();
                PageButton.Initialize(_lastPage, SetPage, _query);
                _choosePageButton.interactable = _query > 2;
            }
            private void updateNextAndPrevPageButtons()
            {
                if (_query < 2)
                {
                    _nextPage.gameObject.SetActive(false);
                    _prevPage.gameObject.SetActive(false);
                    return;
                }
                _nextPage.gameObject.SetActive(_browser._currentPage < _query);
                _prevPage.gameObject.SetActive(_browser._currentPage != 1);
            }

            private void togglePageView()
            {
                SetPageViewActive(!_pageViewActive);
            }
            public void SetPageViewActive(bool value)
            {
                StopAllCoroutines();
                _pageViewActive = value;
                _pageViewMain.gameObject.SetActive(value);

                if (value)
                {
                    TransformUtils.DestroyAllChildren(_pageViewContainer);
                    StartCoroutine(spawnPages());
                }
            }
            IEnumerator spawnPages()
            {
                int timesToNextStop = 10;
                for (int i = 1; i < _query - 1; i++)
                {
                    timesToNextStop--;
                    if (timesToNextStop == 0)
                    {
                        timesToNextStop = 10;
                        yield return new WaitForEndOfFrame();
                    }
                    Button button = Instantiate<Button>(_pageViewEntry, _pageViewContainer);
                    button.gameObject.SetActive(true);
                    button.GetComponent<Text>().text = (i + 1).ToString();
                    PageButton.Initialize(button, SetPage, i + 1);
                }
                yield break;
            }

            public class PageButton
            {
                public static void Initialize(Button button, System.Action<int> onSelect, int myPage)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate
                    {
                        onSelect(myPage);
                    });
                }
            }
        }

        public enum BrowserState
        {
            Unknown,

            Idle,

            LoadingItems,

            InitializingItems
        }
    }
}
