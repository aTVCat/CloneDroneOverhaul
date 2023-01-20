using CloneDroneOverhaul.UI.Components;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CloneDroneOverhaul.V3.Notifications;

namespace CloneDroneOverhaul.V3.HUD
{

    public class UIWorkshopBrowser : V3_ModHUDBase
    {
        private void Start()
        {
            base.MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(new UnityAction(Hide));
            _itemDetailsUI = base.MyModdedObject.GetObjectFromList<Transform>(8).gameObject.AddComponent<UIWorkshopBrowser.ItemDetailsUI>();
            _itemDetailsUI.Initialize(this);
            _pages = UIWorkshopBrowser.Pages.Initialize(this, base.MyModdedObject.GetObjectFromList<ModdedObject>(13));
            _levelsView = base.MyModdedObject.GetObjectFromList<Transform>(7);
            _collectionsView = base.MyModdedObject.GetObjectFromList<Transform>(19);
            Hide();
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onLanguageChanged")
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
                _selectedTabID = UIWorkshopBrowser.TAB_IDS[2];
                _selectedLeveltype = 0;
                base.MyModdedObject.GetObjectFromList<Text>(1).text = "";
                SetPage(1);
                PopulateWorkshopItems();
            }
            base.MyModdedObject.GetObjectFromList<Text>(15).text = OverhaulMain.GetTranslatedString("WBUI_Title");
            _pages.GetComponent<ModdedObject>().GetObjectFromList<Text>(9).text = OverhaulMain.GetTranslatedString("WBUI_Pages_Header");
        }

        // Token: 0x060000EA RID: 234 RVA: 0x00007A31 File Offset: 0x00005C31
        public void Hide()
        {
            base.gameObject.SetActive(false);
            if (GameModeManager.IsOnTitleScreen())
            {
                V3.Gameplay.ArenaController.SetRootAndLogoVisible(true);
            }
        }

        // Token: 0x060000EB RID: 235 RVA: 0x00007A4C File Offset: 0x00005C4C
        public void SetState(UIWorkshopBrowser.BrowserState newState)
        {
            _currentState = newState;
            switch (newState)
            {
                case UIWorkshopBrowser.BrowserState.Unknown:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "";
                    return;
                case UIWorkshopBrowser.BrowserState.Idle:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "";
                    return;
                case UIWorkshopBrowser.BrowserState.LoadingItems:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "Loading...";
                    return;
                case UIWorkshopBrowser.BrowserState.InitializingItems:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "Finishing...";
                    return;
                default:
                    return;
            }
        }

        // Token: 0x060000EC RID: 236 RVA: 0x00007AD6 File Offset: 0x00005CD6
        public SteamWorkshopItem GetSelectedWorkshopItem()
        {
            return _selectedItem;
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00007ADE File Offset: 0x00005CDE
        public int GetCurrentLevelType()
        {
            return _selectedLeveltype;
        }

        // Token: 0x060000EE RID: 238 RVA: 0x00007AE6 File Offset: 0x00005CE6
        public bool AllowSwitching()
        {
            return _canRereshItems && _currentState != UIWorkshopBrowser.BrowserState.LoadingItems && _currentState != UIWorkshopBrowser.BrowserState.InitializingItems;
        }

        // Token: 0x060000EF RID: 239 RVA: 0x00007B07 File Offset: 0x00005D07
        public void SetSelectedWorkshopItem(SteamWorkshopItem item)
        {
            _selectedItem = item;
        }

        // Token: 0x060000F0 RID: 240 RVA: 0x00007B10 File Offset: 0x00005D10
        public void SetPage(int pageNum)
        {
            _currentPage = pageNum;
        }

        // Token: 0x060000F1 RID: 241 RVA: 0x00007B1C File Offset: 0x00005D1C
        private void populateTabsAndLevelTypeDropdowns()
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
                foreach (string text in _tabButtonImages_IDs)
                {
                    OverhaulCacheAndGarbageController.RemoveTemporalObject(text);
                    OverhaulCacheAndGarbageController.RemoveTemporalObject(text.Replace("ImageComp_", string.Empty));
                }
            }
            _tabButtonImages_IDs.Clear();
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(3));
            foreach (string text2 in UIWorkshopBrowser.TAB_IDS)
            {
                ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(2), base.MyModdedObject.GetObjectFromList<Transform>(3));
                moddedObject.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("WBUI_Tab_" + text2);
                ReferenceOnClick.CallOnClickWithReference(moddedObject.gameObject, new Action<MonoBehaviour>(onTabSelected));
                string name = "WorkshopBrowser_CDO_" + moddedObject.gameObject.GetInstanceID().ToString();
                OverhaulCacheAndGarbageController.AddTemporalObject<string>(text2, name);
                string text3 = "WorkshopBrowser_CDO_ImageComp_" + moddedObject.gameObject.GetInstanceID().ToString();
                OverhaulCacheAndGarbageController.AddTemporalObject<Image>(moddedObject.GetComponent<Image>(), text3);
                moddedObject.GetComponent<Image>().color = BaseUtils.ColorFromHex("#45474F");
                _tabButtonImages_IDs.Add(text3);
                moddedObject.gameObject.SetActive(true);
            }
            base.MyModdedObject.GetObjectFromList<Dropdown>(4).options.Clear();
            foreach (string text4 in UIWorkshopBrowser.LEVEL_TYPES)
            {
                base.MyModdedObject.GetObjectFromList<Dropdown>(4).options.Add(new Dropdown.OptionData(text4));
            }
            base.MyModdedObject.GetObjectFromList<Dropdown>(4).onValueChanged.AddListener(new UnityAction<int>(onSelectedLevelType));
            _canRereshItems = true;
        }

        // Token: 0x060000F2 RID: 242 RVA: 0x00007D50 File Offset: 0x00005F50
        private void onSelectedLevelType(int value)
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
                base.MyModdedObject.GetObjectFromList<Text>(1).text = Singleton<LocalizationManager>.Instance.GetTranslatedString("endlessLevelsDescription", -1);
                return;
            }
            if (value == 3)
            {
                base.MyModdedObject.GetObjectFromList<Text>(1).text = Singleton<LocalizationManager>.Instance.GetTranslatedString("lbsLevelsDescription", -1);
                return;
            }
            base.MyModdedObject.GetObjectFromList<Text>(1).text = "";
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x00007DDC File Offset: 0x00005FDC
        private void onTabSelected(MonoBehaviour tab)
        {
            if (!AllowSwitching())
            {
                return;
            }
            SetPage(1);
            foreach (string name in _tabButtonImages_IDs)
            {
                Image temporalObject = OverhaulCacheAndGarbageController.GetTemporalObject<Image>(name);
                temporalObject.color = BaseUtils.ColorFromHex("#45474F");
            }
            _selectedTabID = OverhaulCacheAndGarbageController.GetTemporalObject<string>("WorkshopBrowser_CDO_" + tab.gameObject.GetInstanceID().ToString());
            OverhaulCacheAndGarbageController.GetTemporalObject<Image>("WorkshopBrowser_CDO_ImageComp_" + tab.gameObject.GetInstanceID().ToString()).color = BaseUtils.ColorFromHex("#3C9C63");
            PopulateWorkshopItems();
        }


        // Token: 0x060000F4 RID: 244 RVA: 0x00007EB0 File Offset: 0x000060B0
        public List<SteamWorkshopItem> GetWorkshopItems(int selectLevelType, string tabID, int page, EUGCQuery queryType, Action<List<SteamWorkshopItem>> onReceive = null)
        {
            List<SteamWorkshopItem> result = new List<SteamWorkshopItem>();
            if (tabID == UIWorkshopBrowser.TAB_IDS[5])
            {
                Singleton<SteamWorkshopManager>.Instance.GetWorkshopItems(UIWorkshopBrowser.LEVEL_TYPES[selectLevelType], page, delegate (List<SteamWorkshopItem> items)
                {
                    result = items;
                    if (onReceive != null)
                    {
                        onReceive(items);
                    }
                });
                return result;
            }
            if (tabID == UIWorkshopBrowser.TAB_IDS[0])
            {
                Singleton<SteamWorkshopManager>.Instance.GetCollectionChildren(new PublishedFileId_t(2652995786), new Action<List<SteamWorkshopItem>>(onReceive));
                return result;
            }
            Singleton<SteamWorkshopManager>.Instance.GetAllNewWorkshopItems(UIWorkshopBrowser.LEVEL_TYPES[selectLevelType], queryType, page, delegate (List<SteamWorkshopItem> items)
            {
                result = items;
                if (onReceive != null)
                {
                    onReceive(items);
                }
            });
            return result;
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x00007F30 File Offset: 0x00006130
        public EUGCQuery EUGCQueryByTabID(string tabID)
        {
            EUGCQuery result = EUGCQuery.k_EUGCQuery_RankedByTrend;
            if (tabID == UIWorkshopBrowser.TAB_IDS[1])
            {
                result = EUGCQuery.k_EUGCQuery_RankedByVote;
            }
            else if (tabID == UIWorkshopBrowser.TAB_IDS[2])
            {
                result = EUGCQuery.k_EUGCQuery_RankedByTrend;
            }
            else if (tabID == UIWorkshopBrowser.TAB_IDS[3])
            {
                result = EUGCQuery.k_EUGCQuery_RankedByPublicationDate;
            }
            else if (tabID == UIWorkshopBrowser.TAB_IDS[4])
            {
                result = EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate;
            }
            else if (tabID == UIWorkshopBrowser.TAB_IDS[6])
            {
                result = EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate;
            }
            else if (tabID == UIWorkshopBrowser.TAB_IDS[7])
            {
                result = EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate;
            }
            return result;
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00007FB0 File Offset: 0x000061B0
        public void PopulateWorkshopItems()
        {
            if (!_canRereshItems)
            {
                return;
            }
            clearExistingItems();
            SetState(UIWorkshopBrowser.BrowserState.LoadingItems);

            bool viewCollections = _selectedTabID == TAB_IDS[0];
            _isViewingCollectionChildren = viewCollections;
            base.MyModdedObject.GetObjectFromList<Transform>(21).gameObject.SetActive(viewCollections);

            base.MyModdedObject.GetObjectFromList<Dropdown>(4).interactable = !viewCollections;

            _levelsView.gameObject.SetActive(!viewCollections);
            _collectionsView.gameObject.SetActive(viewCollections);

            if (viewCollections)
            {
                GetWorkshopItems(_selectedLeveltype, _selectedTabID, _currentPage, EUGCQueryByTabID(_selectedTabID), delegate (List<SteamWorkshopItem> items)
                {
                    SetState(UIWorkshopBrowser.BrowserState.InitializingItems);
                    base.StartCoroutine(asyncPopulateWorkshopItems(items, true));
                    _pages.SetupForCollectionsView();
                });
                return;
            }
            GetWorkshopItems(_selectedLeveltype, _selectedTabID, _currentPage, EUGCQueryByTabID(_selectedTabID), delegate (List<SteamWorkshopItem> items)
            {
                SetState(UIWorkshopBrowser.BrowserState.InitializingItems);
                base.StartCoroutine(asyncPopulateWorkshopItems(items));
                _pages.UpdatePages();
            });
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00008004 File Offset: 0x00006204
        private IEnumerator asyncPopulateWorkshopItems(List<SteamWorkshopItem> items, bool populateCollections = false)
        {
            foreach (SteamWorkshopItem item in items)
            {
                yield return new WaitForEndOfFrame();
                if (!_canRereshItems)
                {
                    SetState(UIWorkshopBrowser.BrowserState.Idle);
                    yield break;
                }

                if (!populateCollections)
                {
                    ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(6), _levelsView);
                    moddedObject.GetObjectFromList<Text>(1).text = item.Title;
                    bool active = Singleton<ChallengeManager>.Instance.HasCompletedChallenge(item.WorkshopItemID.ToString());
                    moddedObject.GetObjectFromList<Transform>(2).gameObject.SetActive(active);
                    moddedObject.gameObject.AddComponent<UIWorkshopBrowser.ItemDisplay>().Initialize(this, item);
                    moddedObject.gameObject.SetActive(true);
                }
                else
                {
                    ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(20), _collectionsView);
                    moddedObject.GetObjectFromList<Text>(1).text = item.Title;
                    moddedObject.GetObjectFromList<Text>(2).text = item.Description;
                    moddedObject.GetObjectFromList<ModdedObject>(0).gameObject.AddComponent<ImageLoader>().LoadImage(item);
                    moddedObject.gameObject.SetActive(true);
                }
            }
            SetState(UIWorkshopBrowser.BrowserState.Idle);
            yield break;
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x0000801A File Offset: 0x0000621A
        private void clearExistingItems()
        {
            base.StopAllCoroutines();
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(7));
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(19));
            _itemDetailsUI.PopulateItemInfo(null, null);
            _pages.SetPageViewActive(false);
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x0000803F File Offset: 0x0000623F
        public void AddItemToList(UIWorkshopBrowser.ItemDisplay display)
        {
            if (!_spawnedItems.Contains(display))
            {
                _spawnedItems.Add(display);
            }
        }

        // Token: 0x060000FA RID: 250 RVA: 0x0000805B File Offset: 0x0000625B
        public void RemoveItemFromList(UIWorkshopBrowser.ItemDisplay display)
        {
            if (_spawnedItems.Contains(display))
            {
                _spawnedItems.Remove(display);
            }
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00008078 File Offset: 0x00006278
        public void DeselectAllItems()
        {
            foreach (UIWorkshopBrowser.ItemDisplay itemDisplay in _spawnedItems)
            {
                itemDisplay.SetSelected(false);
            }
        }

        // Token: 0x060000FC RID: 252 RVA: 0x000080CC File Offset: 0x000062CC
        public bool OnItemClicked(UIWorkshopBrowser.ItemDisplay display)
        {
            if (display == null || display.SteamItem == null || _itemDetailsUI == null)
            {
                return false;
            }
            _selectedItem = display.SteamItem;
            _itemDetailsUI.PopulateItemInfo(_selectedItem, display.ImageLoader.ImageComponent.sprite);
            return true;
        }

        private UIWorkshopBrowser.ItemDetailsUI _itemDetailsUI;

        private UIWorkshopBrowser.Pages _pages;

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
            "Friends favourite"
        };

        public static readonly string[] LEVEL_TYPES = new string[]
        {
            "Endless Level",
            "Challenge",
            "Adventure",
            "Last Bot Standing Level"
        };

        public const string TAB_SELECTED_COLOR_HEX = "#3C9C63";

        public const string TAB_DEFAULT_COLOR_HEX = "#45474F";

        public const string TEMPORAL_PREFIX = "WorkshopBrowser_CDO_";

        private bool _canRereshItems;

        private int _selectedLeveltype;

        private int _currentPage = 1;

        private string _selectedTabID;

        private List<UIWorkshopBrowser.ItemDisplay> _spawnedItems = new List<UIWorkshopBrowser.ItemDisplay>();

        private List<string> _tabButtonImages_IDs = new List<string>();

        private SteamWorkshopItem _selectedItem;

        private UIWorkshopBrowser.BrowserState _currentState = UIWorkshopBrowser.BrowserState.Idle;

        private bool _hasShownEarlier;

        private bool _hasToRefreshTabs = true;

        private Transform _levelsView;

        private Transform _collectionsView;

        private bool _isViewingCollectionChildren;

        public class ItemDisplay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
        {
            // Token: 0x0600039D RID: 925 RVA: 0x00013D64 File Offset: 0x00011F64
            public void Initialize(UIWorkshopBrowser browser, SteamWorkshopItem item)
            {
                SteamItem = item;
                _browserReference = browser;
                _browserReference.AddItemToList(this);
                _moddedObject = base.GetComponent<ModdedObject>();
                ImageLoader = _moddedObject.GetObjectFromList<Transform>(0).gameObject.AddComponent<ImageLoader>();
                ImageLoader.LoadImage(item);
                SetSelected(false);
            }

            // Token: 0x0600039E RID: 926 RVA: 0x00013DCB File Offset: 0x00011FCB
            private void OnDestroy()
            {
                _browserReference.RemoveItemFromList(this);
            }

            // Token: 0x0600039F RID: 927 RVA: 0x00013DD9 File Offset: 0x00011FD9
            void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
            {
                if (_browserReference.OnItemClicked(this))
                {
                    _browserReference.DeselectAllItems();
                    SetSelected(true);
                }
            }

            // Token: 0x060003A0 RID: 928 RVA: 0x00013DFB File Offset: 0x00011FFB
            public void SetSelected(bool value)
            {
                _moddedObject.GetObjectFromList<Image>(3).enabled = value;
            }

            // Token: 0x040002BF RID: 703
            private UIWorkshopBrowser _browserReference;

            // Token: 0x040002C0 RID: 704
            public SteamWorkshopItem SteamItem;

            // Token: 0x040002C1 RID: 705
            private ModdedObject _moddedObject;

            // Token: 0x040002C2 RID: 706
            public ImageLoader ImageLoader;
        }

        // Token: 0x0200008F RID: 143
        public class ItemDetailsUI : MonoBehaviour
        {
            // Token: 0x060003A2 RID: 930 RVA: 0x00013E18 File Offset: 0x00012018
            public void Initialize(UIWorkshopBrowser browser)
            {
                _browser = browser;
                base.GetComponent<Image>().color = BaseUtils.ColorFromHex("#292A30");
                _moddedObject = base.GetComponent<ModdedObject>();
                _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(false);
                _imageLoader = _moddedObject.GetObjectFromList<Transform>(5).gameObject.AddComponent<ImageLoader>();
                _optionsMenuTransform = _moddedObject.GetObjectFromList<Transform>(2);
                _optionsMenuTransform.gameObject.SetActive(false);
                _steamPageButton = _moddedObject.GetObjectFromList<Button>(16);
                _steamPageButton.onClick.AddListener(new UnityAction(viewItemSteamPage));
                _resetProgress = _moddedObject.GetObjectFromList<Button>(15);
                _resetProgress.onClick.AddListener(new UnityAction(resetProgress));
                _unsubscribeFromItem = _moddedObject.GetObjectFromList<Button>(17);
                _unsubscribeFromItem.onClick.AddListener(new UnityAction(unsubscribeFromItem));
                _itemTitle = _moddedObject.GetObjectFromList<Text>(3);
                _itemMadeBy = _moddedObject.GetObjectFromList<Text>(4);
                _itemDesc = _moddedObject.GetObjectFromList<Text>(6);
                _goToGameMode = _moddedObject.GetObjectFromList<Text>(10);
                _optionsButton = _moddedObject.GetObjectFromList<Button>(1);
                _optionsButton.onClick.AddListener(new UnityAction(toggleOptionsMenu));
                _playButton = _moddedObject.GetObjectFromList<Button>(0);
                _playButton.onClick.AddListener(delegate ()
                {
                    if (_browser.GetCurrentLevelType() != 0 && _browser.GetCurrentLevelType() != 3)
                    {
                        V3.HUD.TransitionAction act = new V3.HUD.TransitionAction
                        {
                            Type = V3.HUD.ETranstionType.Method,
                            Action = delegate
                            {
                                if (Singleton<WorkshopChallengeManager>.Instance.StartChallengeFromWorkshop(_browser.GetSelectedWorkshopItem()))
                                {
                                    _browser.Hide();
                                    return;
                                }
                            },
                            HideOnComplete = true
                        };
                        V3.HUD.SceneTransitionController.StartTranstion(act, "Spawning level...", string.Empty, false);
                        return;
                    }
                    else
                    {
                        if (_browser.GetCurrentLevelType() == 0)
                        {
                            _playButton.gameObject.SetActive(false);
                            _goToGameMode.gameObject.SetActive(true);
                            _goToGameMode.text = "Start Endless mode?";
                            _goToGameMode.GetComponent<Button>().onClick.RemoveAllListeners();
                            _goToGameMode.GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                _browser.Hide();
                                V3.HUD.TransitionAction act = new V3.HUD.TransitionAction
                                {
                                    Type = V3.HUD.ETranstionType.Method,
                                    Action = delegate { Singleton<GameFlowManager>.Instance.StartEndlessModeGame(); },
                                    HideOnComplete = true
                                };
                                V3.HUD.SceneTransitionController.StartTranstion(act, "Starting Endless mode...", string.Empty, false);
                            });

                            return;
                        }
                        if (_browser.GetCurrentLevelType() == 3)
                        {
                            _playButton.gameObject.SetActive(false);
                            _goToGameMode.gameObject.SetActive(true);
                            _goToGameMode.text = "Start Last Bot Standing Match?";
                            _goToGameMode.GetComponent<Button>().onClick.RemoveAllListeners();
                            _goToGameMode.GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                _browser.Hide();
                                Singleton<GameUIRoot>.Instance.TitleScreenUI.MultiplayerModeSelectScreen.Show();
                                Singleton<GameUIRoot>.Instance.TitleScreenUI.MultiplayerModeSelectScreen.GameModeData[2].ClickedCallback.Invoke();
                            });
                        }
                    }
                });
                _subscribeButton = _moddedObject.GetObjectFromList<Button>(12);
                _subscribeButton.onClick.AddListener(new UnityAction(subscribeToItem));
                _downloadProgressSlider = _moddedObject.GetObjectFromList<Slider>(11);
                _downloadState = _moddedObject.GetObjectFromList<Text>(13);
            }

            // Token: 0x060003A3 RID: 931 RVA: 0x00013FFC File Offset: 0x000121FC
            public void PopulateItemInfo(SteamWorkshopItem item, Sprite sprite)
            {
                if (item == null)
                {
                    _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(false);
                    base.GetComponent<Image>().color = BaseUtils.ColorFromHex("#1D1D1D");
                    _imageLoader.SetImage(null);
                    return;
                }
                _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(true);
                base.GetComponent<Image>().color = BaseUtils.ColorFromHex("#108424");
                _itemTitle.text = item.Title;
                _itemDesc.text = item.Description;
                _itemMadeBy.text = item.CreatorName;
                if (sprite == null || sprite.name == "placeholderLoad")
                {
                    _imageLoader.LoadImage(item);
                }
                else
                {
                    _imageLoader.SetImage(sprite);
                }
                _selectedLevelHasSavedData = false;
                if (_browser.GetCurrentLevelType() == 1 || _browser.GetCurrentLevelType() == 2)
                {
                    DataRepository instance = Singleton<DataRepository>.Instance;
                    string str = "ChallengeData";
                    PublishedFileId_t workshopItemID = _browser.GetSelectedWorkshopItem().WorkshopItemID;
                    if (File.Exists(instance.GetFullPath(str + workshopItemID.ToString() + ".json", false)))
                    {
                        _selectedLevelHasSavedData = true;
                    }
                }
                RefreshPlayButtons();
                refreshIsDownloading(false);
                setOptionsVisible(false);
            }

            // Token: 0x060003A4 RID: 932 RVA: 0x00014124 File Offset: 0x00012324
            public void RefreshPlayButtons()
            {
                _goToGameMode.gameObject.SetActive(false);
                SteamWorkshopItem selectedWorkshopItem = _browser.GetSelectedWorkshopItem();
                if (selectedWorkshopItem == null)
                {
                    return;
                }
                bool flag = !string.IsNullOrEmpty(selectedWorkshopItem.Folder);
                if (BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(selectedWorkshopItem) != -1f)
                {
                    _playButton.gameObject.SetActive(false);
                    _subscribeButton.gameObject.SetActive(false);
                    _downloadProgressSlider.gameObject.SetActive(true);
                    refreshIsDownloading(true);
                    return;
                }
                _playButton.gameObject.SetActive(flag);
                _subscribeButton.gameObject.SetActive(!flag);
                if (_browser.GetCurrentLevelType() == 0 && Singleton<SettingsManager>.Instance.GetWorkshopInEndlessPolicy() == WorkshopinEndlessPolicy.NoWorkshop)
                {
                    hideAllButtons();
                }
                _downloadProgressSlider.gameObject.SetActive(false);
            }

            // Token: 0x060003A5 RID: 933 RVA: 0x00014200 File Offset: 0x00012400
            private void hideAllButtons()
            {
                _playButton.gameObject.SetActive(false);
                _subscribeButton.gameObject.SetActive(false);
                _downloadProgressSlider.gameObject.SetActive(false);
                _goToGameMode.gameObject.SetActive(false);
            }

            // Token: 0x060003A6 RID: 934 RVA: 0x00014254 File Offset: 0x00012454
            private void refreshIsDownloading(bool isCalledDuringDownload = false)
            {
                if (!isCalledDuringDownload)
                {
                    _isDownloading = false;
                }
                SteamWorkshopItem selectedWorkshopItem = _browser.GetSelectedWorkshopItem();
                if (selectedWorkshopItem == null)
                {
                    return;
                }
                float itemLoadProgress = BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(selectedWorkshopItem);
                if (!isCalledDuringDownload)
                {
                    _isDownloading = (itemLoadProgress != -1f);
                }
                if (!isCalledDuringDownload && !_isDownloading)
                {
                    return;
                }
                _playButton.gameObject.SetActive(false);
                _subscribeButton.gameObject.SetActive(false);
                _downloadProgressSlider.gameObject.SetActive(true);
                if ((_isDownloading || isCalledDuringDownload) && itemLoadProgress != -1f)
                {
                    _downloadProgressSlider.value = itemLoadProgress;
                }
                if (itemLoadProgress == -1f)
                {
                    _downloadState.text = "Waiting download to start...";
                    return;
                }
                _downloadState.text = "Downloading";
            }

            // Token: 0x060003A7 RID: 935 RVA: 0x0001431C File Offset: 0x0001251C
            private void subscribeToItem()
            {
                SteamWorkshopItem selectedWorkshopItem = _browser.GetSelectedWorkshopItem();
                if (selectedWorkshopItem != null)
                {
                    Singleton<SteamWorkshopManager>.Instance.SubscribeToItem(selectedWorkshopItem.WorkshopItemID, delegate (SteamWorkshopItem itemOut)
                    {
                        _browser.SetSelectedWorkshopItem(itemOut);
                        _isDownloading = false;
                        RefreshPlayButtons();
                        SNotification notif = new SNotification("Download success!", itemOut.Title + " was downloaded", 6f, UINotifications.NotificationSize_Default, null);
                        notif.Send();
                    }, delegate (string error)
                    {
                        _isDownloading = false;
                        RefreshPlayButtons();
                        SNotification notif = new SNotification("Download error", error, 6f, UINotifications.NotificationSize_Default, null);
                        notif.Send();
                    });
                    _isDownloading = true;
                    _downloadProgressSlider.value = 0f;
                    _subscribeButton.gameObject.SetActive(false);
                    _downloadProgressSlider.gameObject.SetActive(true);
                }
            }

            // Token: 0x060003A8 RID: 936 RVA: 0x00014399 File Offset: 0x00012599
            private void toggleOptionsMenu()
            {
                setOptionsVisible(!_showingOptions);
            }

            // Token: 0x060003A9 RID: 937 RVA: 0x000143AA File Offset: 0x000125AA
            private void setOptionsVisible(bool value)
            {
                _showingOptions = value;
                _optionsMenuTransform.gameObject.SetActive(_showingOptions);
                _resetProgress.interactable = _selectedLevelHasSavedData;
            }

            private void viewItemSteamPage()
            {
                string str = "https://steamcommunity.com/sharedfiles/filedetails/?id=";
                PublishedFileId_t workshopItemID = _browser.GetSelectedWorkshopItem().WorkshopItemID;
                BaseUtils.OpenURL(str + workshopItemID.ToString());
            }

            private void resetProgress()
            {
                if (!_selectedLevelHasSavedData)
                {
                    return;
                }
                _selectedLevelHasSavedData = false;
                setOptionsVisible(true);
                DataRepository instance = Singleton<DataRepository>.Instance;
                string str = "ChallengeData";
                PublishedFileId_t workshopItemID = _browser.GetSelectedWorkshopItem().WorkshopItemID;
                File.Delete(instance.GetFullPath(str + workshopItemID.ToString() + ".json", false));
            }

            private void unsubscribeFromItem()
            {
                SteamWorkshopItem item = _browser.GetSelectedWorkshopItem();
                SteamWorkshopManager.Instance.UnsubscribeFromItem(item.WorkshopItemID);
                item.Folder = string.Empty;
                RefreshPlayButtons();
            }

            private void Update()
            {
                if (_isDownloading)
                {
                    refreshIsDownloading(true);
                }
            }

            public const string NoItemsSelectedColor = "#292A30";

            public const string ItemsSelectedColor = "#108424";

            private UIWorkshopBrowser _browser;

            private ImageLoader _imageLoader;

            private ModdedObject _moddedObject;

            private Transform _optionsMenuTransform;

            private Button _steamPageButton;

            private Button _resetProgress;

            private Button _unsubscribeFromItem;

            private Text _itemTitle;

            private Text _itemDesc;

            private Text _itemMadeBy;

            private Text _goToGameMode;

            private Button _playButton;

            private Button _subscribeButton;

            private Button _optionsButton;

            private Slider _downloadProgressSlider;

            private Text _downloadState;

            private bool _isDownloading;

            private bool _showingOptions;

            private bool _selectedLevelHasSavedData;
        }

        // Token: 0x02000090 RID: 144
        public class Pages : MonoBehaviour
        {
            // Token: 0x060003B3 RID: 947 RVA: 0x000146C8 File Offset: 0x000128C8
            public static UIWorkshopBrowser.Pages Initialize(UIWorkshopBrowser browser, ModdedObject moddedObject)
            {
                UIWorkshopBrowser.Pages pages = moddedObject.gameObject.AddComponent<UIWorkshopBrowser.Pages>();
                pages._browser = browser;
                pages._moddedObject = moddedObject;
                pages._nextPage = moddedObject.GetObjectFromList<Button>(1);
                pages._nextPage.onClick.AddListener(new UnityAction(pages.NextPage));
                pages._prevPage = moddedObject.GetObjectFromList<Button>(0);
                pages._prevPage.onClick.AddListener(new UnityAction(pages.PrevPage));
                pages._choosePageButton = moddedObject.GetObjectFromList<Button>(4);
                pages._choosePageButton.onClick.AddListener(new UnityAction(pages.togglePageView));
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

            public void UpdatePages()
            {
                _query = Singleton<SteamWorkshopManager>.Instance.GetNumPagesForLastQuery();
                updateFirstAndLastPageButtons();
                updateNextAndPrevPageButtons();
                _currentPageText.text = _browser._currentPage.ToString();
            }

            public void SetupForCollectionsView()
            {
                _query = 0;
                _currentPageText.text = string.Empty;
                updateFirstAndLastPageButtons();
                updateNextAndPrevPageButtons();
            }

            public void SetPage(int page)
            {
                if (!_browser.AllowSwitching() || _browser._isViewingCollectionChildren)
                {
                    return;
                }
                _browser.SetPage(page);
                updateNextAndPrevPageButtons();
                _browser.PopulateWorkshopItems();
                _currentPageText.text = page.ToString();
            }

            // Token: 0x060003B6 RID: 950 RVA: 0x0001484B File Offset: 0x00012A4B
            public void NextPage()
            {
                if (_browser._currentPage + 1 > _query)
                {
                    return;
                }
                SetPage(_browser._currentPage + 1);
            }

            // Token: 0x060003B7 RID: 951 RVA: 0x00014876 File Offset: 0x00012A76
            public void PrevPage()
            {
                if (_browser._currentPage - 1 == 0)
                {
                    return;
                }
                SetPage(_browser._currentPage - 1);
            }

            // Token: 0x060003B8 RID: 952 RVA: 0x0001489C File Offset: 0x00012A9C
            private void updateFirstAndLastPageButtons()
            {
                if (_query == 0)
                {
                    _choosePageButton.interactable = false;
                    return;
                }
                _firstPageText.text = "1";
                UIWorkshopBrowser.Pages.PageButton.Initialize(_firstPage, new Action<int>(SetPage), 1);
                _lastPageText.text = _query.ToString();
                UIWorkshopBrowser.Pages.PageButton.Initialize(_lastPage, new Action<int>(SetPage), _query);
                _choosePageButton.interactable = (_query > 2);
            }

            // Token: 0x060003B9 RID: 953 RVA: 0x00014918 File Offset: 0x00012B18
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

            // Token: 0x060003BA RID: 954 RVA: 0x00014995 File Offset: 0x00012B95
            private void togglePageView()
            {
                SetPageViewActive(!_pageViewActive);
            }

            // Token: 0x060003BB RID: 955 RVA: 0x000149A6 File Offset: 0x00012BA6
            public void SetPageViewActive(bool value)
            {
                base.StopAllCoroutines();
                _pageViewActive = value;
                _pageViewMain.gameObject.SetActive(value);
                if (value)
                {
                    TransformUtils.DestroyAllChildren(_pageViewContainer);
                    base.StartCoroutine(spawnPages());
                }
            }

            // Token: 0x060003BC RID: 956 RVA: 0x000149E1 File Offset: 0x00012BE1
            private IEnumerator spawnPages()
            {
                int timesToNextStop = 10;
                int num;
                for (int i = 1; i < _query - 1; i = num + 1)
                {
                    num = timesToNextStop;
                    timesToNextStop = num - 1;
                    if (timesToNextStop == 0)
                    {
                        timesToNextStop = 10;
                        yield return new WaitForEndOfFrame();
                    }
                    Button button = UnityEngine.Object.Instantiate<Button>(_pageViewEntry, _pageViewContainer);
                    button.gameObject.SetActive(true);
                    button.GetComponent<Text>().text = (i + 1).ToString();
                    UIWorkshopBrowser.Pages.PageButton.Initialize(button, new Action<int>(SetPage), i + 1);
                    num = i;
                }
                yield break;
            }

            private UIWorkshopBrowser _browser;

            private ModdedObject _moddedObject;

            private int _query;

            private bool _pageViewActive;

            private Button _nextPage;

            private Button _prevPage;

            private Button _choosePageButton;

            private Button _firstPage;

            private Text _firstPageText;

            private Button _lastPage;

            private Text _lastPageText;

            private Text _currentPageText;

            private Transform _pageViewMain;

            private Button _pageViewEntry;

            private Transform _pageViewContainer;

            public class PageButton
            {
                public static void Initialize(Button button, Action<int> onSelect, int myPage)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate ()
                    {
                        onSelect(myPage);
                    });
                }
            }
        }

        // Token: 0x02000091 RID: 145
        public enum BrowserState
        {
            // Token: 0x040002E7 RID: 743
            Unknown,
            // Token: 0x040002E8 RID: 744
            Idle,
            // Token: 0x040002E9 RID: 745
            LoadingItems,
            // Token: 0x040002EA RID: 746
            InitializingItems
        }
    }
}
