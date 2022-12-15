using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.UI.Components;
using CloneDroneOverhaul.UI.Notifications;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    // Token: 0x02000021 RID: 33
    public class NewWorkshopBrowserUI : ModGUIBase
    {
        // Token: 0x060000E7 RID: 231 RVA: 0x000078E8 File Offset: 0x00005AE8
        public override void OnInstanceStart()
        {
            NewWorkshopBrowserUI.Instance = this;
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            base.MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(new UnityAction(this.Hide));
            this._itemDetailsUI = base.MyModdedObject.GetObjectFromList<Transform>(8).gameObject.AddComponent<NewWorkshopBrowserUI.ItemDetailsUI>();
            this._itemDetailsUI.Initialize(this);
            this._pages = NewWorkshopBrowserUI.Pages.Initialize(this, base.MyModdedObject.GetObjectFromList<ModdedObject>(13));
            this._levelsView = base.MyModdedObject.GetObjectFromList<Transform>(7);
            this._collectionsView = base.MyModdedObject.GetObjectFromList<Transform>(19);
            this.Hide();
        }

        // Token: 0x060000E8 RID: 232 RVA: 0x00007970 File Offset: 0x00005B70
        public override void RunFunction(string name, object[] arguments)
        {
            if (name == "onLanguageChanged")
            {
                this._hasToRefreshTabs = true;
            }
        }

        // Token: 0x060000E9 RID: 233 RVA: 0x00007988 File Offset: 0x00005B88
        public void Show()
        {
            base.gameObject.SetActive(true);
            this.populateTabsAndLevelTypeDropdowns();
            this._canRereshItems = true;
            if (!this._hasShownEarlier)
            {
                this._hasShownEarlier = true;
                this._selectedTabID = NewWorkshopBrowserUI.TAB_IDS[2];
                this._selectedLeveltype = 0;
                base.MyModdedObject.GetObjectFromList<Text>(1).text = "";
                this.SetPage(1);
                this.PopulateWorkshopItems();
            }
            base.MyModdedObject.GetObjectFromList<Text>(15).text = OverhaulMain.GetTranslatedString("WBUI_Title");
            this._pages.GetComponent<ModdedObject>().GetObjectFromList<Text>(9).text = OverhaulMain.GetTranslatedString("WBUI_Pages_Header");
        }

        // Token: 0x060000EA RID: 234 RVA: 0x00007A31 File Offset: 0x00005C31
        public void Hide()
        {
            base.gameObject.SetActive(false);
            if (GameModeManager.IsOnTitleScreen())
            {
                ArenaManager.SetRootAndLogoVisible(true);
            }
        }

        // Token: 0x060000EB RID: 235 RVA: 0x00007A4C File Offset: 0x00005C4C
        public void SetState(NewWorkshopBrowserUI.BrowserState newState)
        {
            this._currentState = newState;
            switch (newState)
            {
                case NewWorkshopBrowserUI.BrowserState.Unknown:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "";
                    return;
                case NewWorkshopBrowserUI.BrowserState.Idle:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "";
                    return;
                case NewWorkshopBrowserUI.BrowserState.LoadingItems:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "Loading...";
                    return;
                case NewWorkshopBrowserUI.BrowserState.InitializingItems:
                    base.MyModdedObject.GetObjectFromList<Text>(14).text = "Finishing...";
                    return;
                default:
                    return;
            }
        }

        // Token: 0x060000EC RID: 236 RVA: 0x00007AD6 File Offset: 0x00005CD6
        public SteamWorkshopItem GetSelectedWorkshopItem()
        {
            return this._selectedItem;
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00007ADE File Offset: 0x00005CDE
        public int GetCurrentLevelType()
        {
            return this._selectedLeveltype;
        }

        // Token: 0x060000EE RID: 238 RVA: 0x00007AE6 File Offset: 0x00005CE6
        public bool AllowSwitching()
        {
            return this._canRereshItems && this._currentState != NewWorkshopBrowserUI.BrowserState.LoadingItems && this._currentState != NewWorkshopBrowserUI.BrowserState.InitializingItems;
        }

        // Token: 0x060000EF RID: 239 RVA: 0x00007B07 File Offset: 0x00005D07
        public void SetSelectedWorkshopItem(SteamWorkshopItem item)
        {
            this._selectedItem = item;
        }

        // Token: 0x060000F0 RID: 240 RVA: 0x00007B10 File Offset: 0x00005D10
        public void SetPage(int pageNum)
        {
            this._currentPage = pageNum;
        }

        // Token: 0x060000F1 RID: 241 RVA: 0x00007B1C File Offset: 0x00005D1C
        private void populateTabsAndLevelTypeDropdowns()
        {
            if (this._hasPopulatedTabs && !this._hasToRefreshTabs)
            {
                return;
            }
            this._canRereshItems = false;
            this._hasPopulatedTabs = true;
            this._hasToRefreshTabs = false;
            if (this._tabButtonImages_IDs.Count > 0)
            {
                foreach (string text in this._tabButtonImages_IDs)
                {
                    OverhaulCacheManager.RemoveTemporalObject(text);
                    OverhaulCacheManager.RemoveTemporalObject(text.Replace("ImageComp_", string.Empty));
                }
            }
            this._tabButtonImages_IDs.Clear();
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(3));
            foreach (string text2 in NewWorkshopBrowserUI.TAB_IDS)
            {
                ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(2), base.MyModdedObject.GetObjectFromList<Transform>(3));
                moddedObject.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("WBUI_Tab_" + text2);
                ReferenceOnClick.CallOnClickWithReference(moddedObject.gameObject, new Action<MonoBehaviour>(this.onTabSelected));
                string name = "WorkshopBrowser_CDO_" + moddedObject.gameObject.GetInstanceID().ToString();
                OverhaulCacheManager.AddTemporalObject<string>(text2, name);
                string text3 = "WorkshopBrowser_CDO_ImageComp_" + moddedObject.gameObject.GetInstanceID().ToString();
                OverhaulCacheManager.AddTemporalObject<Image>(moddedObject.GetComponent<Image>(), text3);
                moddedObject.GetComponent<Image>().color = BaseUtils.ColorFromHex("#45474F");
                this._tabButtonImages_IDs.Add(text3);
                moddedObject.gameObject.SetActive(true);
            }
            base.MyModdedObject.GetObjectFromList<Dropdown>(4).options.Clear();
            foreach (string text4 in NewWorkshopBrowserUI.LEVEL_TYPES)
            {
                base.MyModdedObject.GetObjectFromList<Dropdown>(4).options.Add(new Dropdown.OptionData(text4));
            }
            base.MyModdedObject.GetObjectFromList<Dropdown>(4).onValueChanged.AddListener(new UnityAction<int>(this.onSelectedLevelType));
            this._canRereshItems = true;
        }

        // Token: 0x060000F2 RID: 242 RVA: 0x00007D50 File Offset: 0x00005F50
        private void onSelectedLevelType(int value)
        {
            if (!this.AllowSwitching())
            {
                return;
            }
            this.SetPage(1);
            this._selectedLeveltype = value;
            this.PopulateWorkshopItems();
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
            if (!this.AllowSwitching())
            {
                return;
            }
            this.SetPage(1);
            foreach (string name in this._tabButtonImages_IDs)
            {
                Image temporalObject = OverhaulCacheManager.GetTemporalObject<Image>(name);
                temporalObject.color = BaseUtils.ColorFromHex("#45474F");
            }
            this._selectedTabID = OverhaulCacheManager.GetTemporalObject<string>("WorkshopBrowser_CDO_" + tab.gameObject.GetInstanceID().ToString());
            OverhaulCacheManager.GetTemporalObject<Image>("WorkshopBrowser_CDO_ImageComp_" + tab.gameObject.GetInstanceID().ToString()).color = BaseUtils.ColorFromHex("#3C9C63");
            this.PopulateWorkshopItems();
        }


        // Token: 0x060000F4 RID: 244 RVA: 0x00007EB0 File Offset: 0x000060B0
        public List<SteamWorkshopItem> GetWorkshopItems(int selectLevelType, string tabID, int page, EUGCQuery queryType, Action<List<SteamWorkshopItem>> onReceive = null)
        {
            List<SteamWorkshopItem> result = new List<SteamWorkshopItem>();
            if (tabID == NewWorkshopBrowserUI.TAB_IDS[5])
            {
                Singleton<SteamWorkshopManager>.Instance.GetWorkshopItems(NewWorkshopBrowserUI.LEVEL_TYPES[selectLevelType], page, delegate (List<SteamWorkshopItem> items)
                {
                    result = items;
                    if (onReceive != null)
                    {
                        onReceive(items);
                    }
                });
                return result;
            }
            if (tabID == NewWorkshopBrowserUI.TAB_IDS[0])
            {
                Singleton<SteamWorkshopManager>.Instance.GetCollectionChildren(new PublishedFileId_t(2652995786), new Action<List<SteamWorkshopItem>>(onReceive));
                return result;
            }
            Singleton<SteamWorkshopManager>.Instance.GetAllNewWorkshopItems(NewWorkshopBrowserUI.LEVEL_TYPES[selectLevelType], queryType, page, delegate (List<SteamWorkshopItem> items)
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
            if (tabID == NewWorkshopBrowserUI.TAB_IDS[1])
            {
                result = EUGCQuery.k_EUGCQuery_RankedByVote;
            }
            else if (tabID == NewWorkshopBrowserUI.TAB_IDS[2])
            {
                result = EUGCQuery.k_EUGCQuery_RankedByTrend;
            }
            else if (tabID == NewWorkshopBrowserUI.TAB_IDS[3])
            {
                result = EUGCQuery.k_EUGCQuery_RankedByPublicationDate;
            }
            else if (tabID == NewWorkshopBrowserUI.TAB_IDS[4])
            {
                result = EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate;
            }
            else if (tabID == NewWorkshopBrowserUI.TAB_IDS[6])
            {
                result = EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate;
            }
            else if (tabID == NewWorkshopBrowserUI.TAB_IDS[7])
            {
                result = EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate;
            }
            return result;
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00007FB0 File Offset: 0x000061B0
        public void PopulateWorkshopItems()
        {
            if (!this._canRereshItems)
            {
                return;
            }
            this.clearExistingItems();
            this.SetState(NewWorkshopBrowserUI.BrowserState.LoadingItems);

            bool viewCollections = this._selectedTabID == TAB_IDS[0];
            _isViewingCollectionChildren = viewCollections;
            base.MyModdedObject.GetObjectFromList<Transform>(21).gameObject.SetActive(viewCollections);

            base.MyModdedObject.GetObjectFromList<Dropdown>(4).interactable = !viewCollections;

            _levelsView.gameObject.SetActive(!viewCollections);
            _collectionsView.gameObject.SetActive(viewCollections);

            if (viewCollections)
            {
                this.GetWorkshopItems(this._selectedLeveltype, this._selectedTabID, this._currentPage, this.EUGCQueryByTabID(this._selectedTabID), delegate (List<SteamWorkshopItem> items)
                {
                    this.SetState(NewWorkshopBrowserUI.BrowserState.InitializingItems);
                    base.StartCoroutine(this.asyncPopulateWorkshopItems(items, true));
                    this._pages.SetupForCollectionsView();
                });
                return;
            }
            this.GetWorkshopItems(this._selectedLeveltype, this._selectedTabID, this._currentPage, this.EUGCQueryByTabID(this._selectedTabID), delegate (List<SteamWorkshopItem> items)
            {
                this.SetState(NewWorkshopBrowserUI.BrowserState.InitializingItems);
                base.StartCoroutine(this.asyncPopulateWorkshopItems(items));
                this._pages.UpdatePages();
            });
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00008004 File Offset: 0x00006204
        private IEnumerator asyncPopulateWorkshopItems(List<SteamWorkshopItem> items, bool populateCollections = false)
        {
            foreach (SteamWorkshopItem item in items)
            {
                yield return new WaitForEndOfFrame();
                if (!this._canRereshItems)
                {
                    this.SetState(NewWorkshopBrowserUI.BrowserState.Idle);
                    yield break;
                }

                if (!populateCollections)
                {
                    ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(6), _levelsView);
                    moddedObject.GetObjectFromList<Text>(1).text = item.Title;
                    bool active = Singleton<ChallengeManager>.Instance.HasCompletedChallenge(item.WorkshopItemID.ToString());
                    moddedObject.GetObjectFromList<Transform>(2).gameObject.SetActive(active);
                    moddedObject.gameObject.AddComponent<NewWorkshopBrowserUI.ItemDisplay>().Initialize(this, item);
                    moddedObject.gameObject.SetActive(true);
                }
                else
                {
                    ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(20), _collectionsView);
                    moddedObject.GetObjectFromList<Text>(1).text = item.Title;
                    moddedObject.GetObjectFromList<Text>(2).text = item.Description;
                    moddedObject.GetObjectFromList<ModdedObject>(0).gameObject.AddComponent<ImageLoader>().LoadImage(item.PreviewURL);
                    moddedObject.gameObject.SetActive(true);
                }
            }
            this.SetState(NewWorkshopBrowserUI.BrowserState.Idle);
            yield break;
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x0000801A File Offset: 0x0000621A
        private void clearExistingItems()
        {
            base.StopAllCoroutines();
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(7));
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(19));
            _itemDetailsUI.PopulateItemInfo(null, null);
            this._pages.SetPageViewActive(false);
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x0000803F File Offset: 0x0000623F
        public void AddItemToList(NewWorkshopBrowserUI.ItemDisplay display)
        {
            if (!this._spawnedItems.Contains(display))
            {
                this._spawnedItems.Add(display);
            }
        }

        // Token: 0x060000FA RID: 250 RVA: 0x0000805B File Offset: 0x0000625B
        public void RemoveItemFromList(NewWorkshopBrowserUI.ItemDisplay display)
        {
            if (this._spawnedItems.Contains(display))
            {
                this._spawnedItems.Remove(display);
            }
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00008078 File Offset: 0x00006278
        public void DeselectAllItems()
        {
            foreach (NewWorkshopBrowserUI.ItemDisplay itemDisplay in this._spawnedItems)
            {
                itemDisplay.SetSelected(false);
            }
        }

        // Token: 0x060000FC RID: 252 RVA: 0x000080CC File Offset: 0x000062CC
        public bool OnItemClicked(NewWorkshopBrowserUI.ItemDisplay display)
        {
            if (display == null || display.SteamItem == null || this._itemDetailsUI == null)
            {
                return false;
            }
            this._selectedItem = display.SteamItem;
            this._itemDetailsUI.PopulateItemInfo(this._selectedItem, display.ImageLoader.ImageComponent.sprite);
            return true;
        }

        public static NewWorkshopBrowserUI Instance;

        private NewWorkshopBrowserUI.ItemDetailsUI _itemDetailsUI;

        private NewWorkshopBrowserUI.Pages _pages;

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

        private List<NewWorkshopBrowserUI.ItemDisplay> _spawnedItems = new List<NewWorkshopBrowserUI.ItemDisplay>();

        private List<string> _tabButtonImages_IDs = new List<string>();

        private SteamWorkshopItem _selectedItem;

        private NewWorkshopBrowserUI.BrowserState _currentState = NewWorkshopBrowserUI.BrowserState.Idle;

        private bool _hasShownEarlier;

        private bool _hasToRefreshTabs = true;

        private Transform _levelsView;

        private Transform _collectionsView;

        private bool _isViewingCollectionChildren;

        public class ItemDisplay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
        {
            // Token: 0x0600039D RID: 925 RVA: 0x00013D64 File Offset: 0x00011F64
            public void Initialize(NewWorkshopBrowserUI browser, SteamWorkshopItem item)
            {
                this.SteamItem = item;
                this._browserReference = browser;
                this._browserReference.AddItemToList(this);
                this._moddedObject = base.GetComponent<ModdedObject>();
                this.ImageLoader = this._moddedObject.GetObjectFromList<Transform>(0).gameObject.AddComponent<ImageLoader>();
                this.ImageLoader.LoadImage(item.PreviewURL);
                this.SetSelected(false);
            }

            // Token: 0x0600039E RID: 926 RVA: 0x00013DCB File Offset: 0x00011FCB
            private void OnDestroy()
            {
                this._browserReference.RemoveItemFromList(this);
            }

            // Token: 0x0600039F RID: 927 RVA: 0x00013DD9 File Offset: 0x00011FD9
            void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
            {
                if (this._browserReference.OnItemClicked(this))
                {
                    this._browserReference.DeselectAllItems();
                    this.SetSelected(true);
                }
            }

            // Token: 0x060003A0 RID: 928 RVA: 0x00013DFB File Offset: 0x00011FFB
            public void SetSelected(bool value)
            {
                this._moddedObject.GetObjectFromList<Image>(3).enabled = value;
            }

            // Token: 0x040002BF RID: 703
            private NewWorkshopBrowserUI _browserReference;

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
            public void Initialize(NewWorkshopBrowserUI browser)
            {
                this._browser = browser;
                base.GetComponent<Image>().color = BaseUtils.ColorFromHex("#292A30");
                this._moddedObject = base.GetComponent<ModdedObject>();
                this._moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(false);
                this._imageLoader = this._moddedObject.GetObjectFromList<Transform>(5).gameObject.AddComponent<ImageLoader>();
                this._optionsMenuTransform = this._moddedObject.GetObjectFromList<Transform>(2);
                this._optionsMenuTransform.gameObject.SetActive(false);
                this._steamPageButton = this._moddedObject.GetObjectFromList<Button>(16);
                this._steamPageButton.onClick.AddListener(new UnityAction(this.viewItemSteamPage));
                this._resetProgress = this._moddedObject.GetObjectFromList<Button>(15);
                this._resetProgress.onClick.AddListener(new UnityAction(this.resetProgress));
                this._unsubscribeFromItem = this._moddedObject.GetObjectFromList<Button>(17);
                this._unsubscribeFromItem.onClick.AddListener(new UnityAction(this.unsubscribeFromItem));
                this._itemTitle = this._moddedObject.GetObjectFromList<Text>(3);
                this._itemMadeBy = this._moddedObject.GetObjectFromList<Text>(4);
                this._itemDesc = this._moddedObject.GetObjectFromList<Text>(6);
                this._goToGameMode = this._moddedObject.GetObjectFromList<Text>(10);
                this._optionsButton = this._moddedObject.GetObjectFromList<Button>(1);
                this._optionsButton.onClick.AddListener(new UnityAction(this.toggleOptionsMenu));
                this._playButton = this._moddedObject.GetObjectFromList<Button>(0);
                this._playButton.onClick.AddListener(delegate ()
                {
                    if (this._browser.GetCurrentLevelType() != 0 && this._browser.GetCurrentLevelType() != 3)
                    {
                        V3Tests.Base.TransitionAction act = new V3Tests.Base.TransitionAction();
                        act.Type = V3Tests.Base.TranstionType.Method;
                        act.Action = delegate
                        {
                            if (Singleton<WorkshopChallengeManager>.Instance.StartChallengeFromWorkshop(this._browser.GetSelectedWorkshopItem()))
                            {
                                this._browser.Hide();
                                return;
                            }
                        };
                        act.HideOnComplete = true;
                        V3Tests.Base.SceneTransitionController.StartTranstion(act, "Spawning level...", string.Empty, false);
                        return;
                    }
                    else
                    {
                        if (this._browser.GetCurrentLevelType() == 0)
                        {
                            this._playButton.gameObject.SetActive(false);
                            this._goToGameMode.gameObject.SetActive(true);
                            this._goToGameMode.text = "Start Endless mode?";
                            this._goToGameMode.GetComponent<Button>().onClick.RemoveAllListeners();
                            this._goToGameMode.GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                this._browser.Hide();
                                V3Tests.Base.TransitionAction act = new V3Tests.Base.TransitionAction();
                                act.Type = V3Tests.Base.TranstionType.Method;
                                act.Action = delegate { Singleton<GameFlowManager>.Instance.StartEndlessModeGame(); };
                                act.HideOnComplete = true;
                                V3Tests.Base.SceneTransitionController.StartTranstion(act, "Starting Endless mode...", string.Empty, false);
                            });

                            return;
                        }
                        if (this._browser.GetCurrentLevelType() == 3)
                        {
                            this._playButton.gameObject.SetActive(false);
                            this._goToGameMode.gameObject.SetActive(true);
                            this._goToGameMode.text = "Start Last Bot Standing Match?";
                            this._goToGameMode.GetComponent<Button>().onClick.RemoveAllListeners();
                            this._goToGameMode.GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                this._browser.Hide();
                                Singleton<GameUIRoot>.Instance.TitleScreenUI.MultiplayerModeSelectScreen.Show();
                                Singleton<GameUIRoot>.Instance.TitleScreenUI.MultiplayerModeSelectScreen.GameModeData[2].ClickedCallback.Invoke();
                            });
                        }
                    }
                });
                this._subscribeButton = this._moddedObject.GetObjectFromList<Button>(12);
                this._subscribeButton.onClick.AddListener(new UnityAction(this.subscribeToItem));
                this._downloadProgressSlider = this._moddedObject.GetObjectFromList<Slider>(11);
                this._downloadState = this._moddedObject.GetObjectFromList<Text>(13);
            }

            // Token: 0x060003A3 RID: 931 RVA: 0x00013FFC File Offset: 0x000121FC
            public void PopulateItemInfo(SteamWorkshopItem item, Sprite sprite)
            {
                if (item == null)
                {
                    this._moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(false);
                    base.GetComponent<Image>().color = BaseUtils.ColorFromHex("#1D1D1D");
                    this._imageLoader.SetImage(null);
                    return;
                }
                this._moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(true);
                base.GetComponent<Image>().color = BaseUtils.ColorFromHex("#108424");
                this._itemTitle.text = item.Title;
                this._itemDesc.text = item.Description;
                this._itemMadeBy.text = item.CreatorName;
                if (sprite == null || sprite.name == "placeholderLoad")
                {
                    this._imageLoader.LoadImage(item.PreviewURL);
                }
                else
                {
                    this._imageLoader.SetImage(sprite);
                }
                this._selectedLevelHasSavedData = false;
                if (this._browser.GetCurrentLevelType() == 1 || this._browser.GetCurrentLevelType() == 2)
                {
                    DataRepository instance = Singleton<DataRepository>.Instance;
                    string str = "ChallengeData";
                    PublishedFileId_t workshopItemID = this._browser.GetSelectedWorkshopItem().WorkshopItemID;
                    if (File.Exists(instance.GetFullPath(str + workshopItemID.ToString() + ".json", false)))
                    {
                        this._selectedLevelHasSavedData = true;
                    }
                }
                this.RefreshPlayButtons();
                this.refreshIsDownloading(false);
                this.setOptionsVisible(false);
            }

            // Token: 0x060003A4 RID: 932 RVA: 0x00014124 File Offset: 0x00012324
            public void RefreshPlayButtons()
            {
                this._goToGameMode.gameObject.SetActive(false);
                SteamWorkshopItem selectedWorkshopItem = this._browser.GetSelectedWorkshopItem();
                if (selectedWorkshopItem == null)
                {
                    return;
                }
                bool flag = !string.IsNullOrEmpty(selectedWorkshopItem.Folder);
                if (BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(selectedWorkshopItem) != -1f)
                {
                    this._playButton.gameObject.SetActive(false);
                    this._subscribeButton.gameObject.SetActive(false);
                    this._downloadProgressSlider.gameObject.SetActive(true);
                    this.refreshIsDownloading(true);
                    return;
                }
                this._playButton.gameObject.SetActive(flag);
                this._subscribeButton.gameObject.SetActive(!flag);
                if (this._browser.GetCurrentLevelType() == 0 && Singleton<SettingsManager>.Instance.GetWorkshopInEndlessPolicy() == WorkshopinEndlessPolicy.NoWorkshop)
                {
                    this.hideAllButtons();
                }
                this._downloadProgressSlider.gameObject.SetActive(false);
            }

            // Token: 0x060003A5 RID: 933 RVA: 0x00014200 File Offset: 0x00012400
            private void hideAllButtons()
            {
                this._playButton.gameObject.SetActive(false);
                this._subscribeButton.gameObject.SetActive(false);
                this._downloadProgressSlider.gameObject.SetActive(false);
                this._goToGameMode.gameObject.SetActive(false);
            }

            // Token: 0x060003A6 RID: 934 RVA: 0x00014254 File Offset: 0x00012454
            private void refreshIsDownloading(bool isCalledDuringDownload = false)
            {
                if (!isCalledDuringDownload)
                {
                    this._isDownloading = false;
                }
                SteamWorkshopItem selectedWorkshopItem = this._browser.GetSelectedWorkshopItem();
                if (selectedWorkshopItem == null)
                {
                    return;
                }
                float itemLoadProgress = BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(selectedWorkshopItem);
                if (!isCalledDuringDownload)
                {
                    this._isDownloading = (itemLoadProgress != -1f);
                }
                if (!isCalledDuringDownload && !this._isDownloading)
                {
                    return;
                }
                this._playButton.gameObject.SetActive(false);
                this._subscribeButton.gameObject.SetActive(false);
                this._downloadProgressSlider.gameObject.SetActive(true);
                if ((this._isDownloading || isCalledDuringDownload) && itemLoadProgress != -1f)
                {
                    this._downloadProgressSlider.value = itemLoadProgress;
                }
                if (itemLoadProgress == -1f)
                {
                    this._downloadState.text = "Waiting download to start...";
                    return;
                }
                this._downloadState.text = "Downloading";
            }

            // Token: 0x060003A7 RID: 935 RVA: 0x0001431C File Offset: 0x0001251C
            private void subscribeToItem()
            {
                SteamWorkshopItem selectedWorkshopItem = this._browser.GetSelectedWorkshopItem();
                if (selectedWorkshopItem != null)
                {
                    Singleton<SteamWorkshopManager>.Instance.SubscribeToItem(selectedWorkshopItem.WorkshopItemID, delegate (SteamWorkshopItem itemOut)
                    {
                        this._browser.SetSelectedWorkshopItem(itemOut);
                        this._isDownloading = false;
                        this.RefreshPlayButtons();
                        new Notification().SetUp("Download success!", itemOut.Title + " was downloaded", 6f, Vector2.zero, Color.clear, null, false);
                    }, delegate (string error)
                    {
                        this._isDownloading = false;
                        this.RefreshPlayButtons();
                        new Notification().SetUp("Download error", error, 3f, new Vector2(450f, 75f), Color.red, null, false);
                    });
                    this._isDownloading = true;
                    this._downloadProgressSlider.value = 0f;
                    this._subscribeButton.gameObject.SetActive(false);
                    this._downloadProgressSlider.gameObject.SetActive(true);
                }
            }

            // Token: 0x060003A8 RID: 936 RVA: 0x00014399 File Offset: 0x00012599
            private void toggleOptionsMenu()
            {
                this.setOptionsVisible(!this._showingOptions);
            }

            // Token: 0x060003A9 RID: 937 RVA: 0x000143AA File Offset: 0x000125AA
            private void setOptionsVisible(bool value)
            {
                this._showingOptions = value;
                this._optionsMenuTransform.gameObject.SetActive(this._showingOptions);
                this._resetProgress.interactable = this._selectedLevelHasSavedData;
            }

            private void viewItemSteamPage()
            {
                string str = "https://steamcommunity.com/sharedfiles/filedetails/?id=";
                PublishedFileId_t workshopItemID = this._browser.GetSelectedWorkshopItem().WorkshopItemID;
                BaseUtils.OpenURL(str + workshopItemID.ToString());
            }

            private void resetProgress()
            {
                if (!this._selectedLevelHasSavedData)
                {
                    return;
                }
                this._selectedLevelHasSavedData = false;
                this.setOptionsVisible(true);
                DataRepository instance = Singleton<DataRepository>.Instance;
                string str = "ChallengeData";
                PublishedFileId_t workshopItemID = this._browser.GetSelectedWorkshopItem().WorkshopItemID;
                File.Delete(instance.GetFullPath(str + workshopItemID.ToString() + ".json", false));
            }

            private void unsubscribeFromItem()
            {
                SteamWorkshopItem item = this._browser.GetSelectedWorkshopItem();
                SteamWorkshopManager.Instance.UnsubscribeFromItem(item.WorkshopItemID);
                item.Folder = string.Empty;
                RefreshPlayButtons();
            }

            private void Update()
            {
                if (this._isDownloading)
                {
                    this.refreshIsDownloading(true);
                }
            }

            public const string NoItemsSelectedColor = "#292A30";

            public const string ItemsSelectedColor = "#108424";

            private NewWorkshopBrowserUI _browser;

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
            public static NewWorkshopBrowserUI.Pages Initialize(NewWorkshopBrowserUI browser, ModdedObject moddedObject)
            {
                NewWorkshopBrowserUI.Pages pages = moddedObject.gameObject.AddComponent<NewWorkshopBrowserUI.Pages>();
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
                this._query = Singleton<SteamWorkshopManager>.Instance.GetNumPagesForLastQuery();
                this.updateFirstAndLastPageButtons();
                this.updateNextAndPrevPageButtons();
                this._currentPageText.text = this._browser._currentPage.ToString();
            }

            public void SetupForCollectionsView()
            {
                this._query = 0;
                this._currentPageText.text = string.Empty;
                this.updateFirstAndLastPageButtons();
                this.updateNextAndPrevPageButtons();
            }

            public void SetPage(int page)
            {
                if (!this._browser.AllowSwitching() || this._browser._isViewingCollectionChildren)
                {
                    return;
                }
                this._browser.SetPage(page);
                this.updateNextAndPrevPageButtons();
                this._browser.PopulateWorkshopItems();
                this._currentPageText.text = page.ToString();
            }

            // Token: 0x060003B6 RID: 950 RVA: 0x0001484B File Offset: 0x00012A4B
            public void NextPage()
            {
                if (this._browser._currentPage + 1 > this._query)
                {
                    return;
                }
                this.SetPage(this._browser._currentPage + 1);
            }

            // Token: 0x060003B7 RID: 951 RVA: 0x00014876 File Offset: 0x00012A76
            public void PrevPage()
            {
                if (this._browser._currentPage - 1 == 0)
                {
                    return;
                }
                this.SetPage(this._browser._currentPage - 1);
            }

            // Token: 0x060003B8 RID: 952 RVA: 0x0001489C File Offset: 0x00012A9C
            private void updateFirstAndLastPageButtons()
            {
                if (this._query == 0)
                {
                    this._choosePageButton.interactable = false;
                    return;
                }
                this._firstPageText.text = "1";
                NewWorkshopBrowserUI.Pages.PageButton.Initialize(this._firstPage, new Action<int>(this.SetPage), 1);
                this._lastPageText.text = this._query.ToString();
                NewWorkshopBrowserUI.Pages.PageButton.Initialize(this._lastPage, new Action<int>(this.SetPage), this._query);
                this._choosePageButton.interactable = (this._query > 2);
            }

            // Token: 0x060003B9 RID: 953 RVA: 0x00014918 File Offset: 0x00012B18
            private void updateNextAndPrevPageButtons()
            {
                if (this._query < 2)
                {
                    this._nextPage.gameObject.SetActive(false);
                    this._prevPage.gameObject.SetActive(false);
                    return;
                }
                this._nextPage.gameObject.SetActive(this._browser._currentPage < this._query);
                this._prevPage.gameObject.SetActive(this._browser._currentPage != 1);
            }

            // Token: 0x060003BA RID: 954 RVA: 0x00014995 File Offset: 0x00012B95
            private void togglePageView()
            {
                this.SetPageViewActive(!this._pageViewActive);
            }

            // Token: 0x060003BB RID: 955 RVA: 0x000149A6 File Offset: 0x00012BA6
            public void SetPageViewActive(bool value)
            {
                base.StopAllCoroutines();
                this._pageViewActive = value;
                this._pageViewMain.gameObject.SetActive(value);
                if (value)
                {
                    TransformUtils.DestroyAllChildren(this._pageViewContainer);
                    base.StartCoroutine(this.spawnPages());
                }
            }

            // Token: 0x060003BC RID: 956 RVA: 0x000149E1 File Offset: 0x00012BE1
            private IEnumerator spawnPages()
            {
                int timesToNextStop = 10;
                int num;
                for (int i = 1; i < this._query - 1; i = num + 1)
                {
                    num = timesToNextStop;
                    timesToNextStop = num - 1;
                    if (timesToNextStop == 0)
                    {
                        timesToNextStop = 10;
                        yield return new WaitForEndOfFrame();
                    }
                    Button button = UnityEngine.Object.Instantiate<Button>(this._pageViewEntry, this._pageViewContainer);
                    button.gameObject.SetActive(true);
                    button.GetComponent<Text>().text = (i + 1).ToString();
                    NewWorkshopBrowserUI.Pages.PageButton.Initialize(button, new Action<int>(this.SetPage), i + 1);
                    num = i;
                }
                yield break;
            }

            private NewWorkshopBrowserUI _browser;

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
