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
            this.GetWorkshopItems(this._selectedLeveltype, this._selectedTabID, this._currentPage, this.EUGCQueryByTabID(this._selectedTabID), delegate (List<SteamWorkshopItem> items)
            {
                this.SetState(NewWorkshopBrowserUI.BrowserState.InitializingItems);
                base.StartCoroutine(this.asyncPopulateWorkshopItems(items));
                this._pages.UpdatePages();
            });
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00008004 File Offset: 0x00006204
        private IEnumerator asyncPopulateWorkshopItems(List<SteamWorkshopItem> items)
        {
            foreach (SteamWorkshopItem item in items)
            {
                yield return new WaitForEndOfFrame();
                if (!this._canRereshItems)
                {
                    this.SetState(NewWorkshopBrowserUI.BrowserState.Idle);
                    yield break;
                }
                ModdedObject moddedObject = UnityEngine.Object.Instantiate<ModdedObject>(base.MyModdedObject.GetObjectFromList<ModdedObject>(6), base.MyModdedObject.GetObjectFromList<Transform>(7));
                moddedObject.GetObjectFromList<Text>(1).text = item.Title;
                bool active = Singleton<ChallengeManager>.Instance.HasCompletedChallenge(item.WorkshopItemID.ToString());
                moddedObject.GetObjectFromList<Transform>(2).gameObject.SetActive(active);
                moddedObject.gameObject.AddComponent<NewWorkshopBrowserUI.ItemDisplay>().Initialize(this, item);
                moddedObject.gameObject.SetActive(true);
            }
            List<SteamWorkshopItem>.Enumerator enumerator = default(List<SteamWorkshopItem>.Enumerator);
            this.SetState(NewWorkshopBrowserUI.BrowserState.Idle);
            yield break;
            yield break;
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x0000801A File Offset: 0x0000621A
        private void clearExistingItems()
        {
            base.StopAllCoroutines();
            TransformUtils.DestroyAllChildren(base.MyModdedObject.GetObjectFromList<Transform>(7));
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

        // Token: 0x0400009A RID: 154
        public static NewWorkshopBrowserUI Instance;

        // Token: 0x0400009B RID: 155
        private NewWorkshopBrowserUI.ItemDetailsUI _itemDetailsUI;

        // Token: 0x0400009C RID: 156
        private NewWorkshopBrowserUI.Pages _pages;

        // Token: 0x0400009D RID: 157
        private bool _hasPopulatedTabs;

        // Token: 0x0400009E RID: 158
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

        // Token: 0x0400009F RID: 159
        public static readonly string[] LEVEL_TYPES = new string[]
        {
            "Endless Level",
            "Challenge",
            "Adventure",
            "Last Bot Standing Level"
        };

        // Token: 0x040000A0 RID: 160
        public const string TAB_SELECTED_COLOR_HEX = "#3C9C63";

        // Token: 0x040000A1 RID: 161
        public const string TAB_DEFAULT_COLOR_HEX = "#45474F";

        // Token: 0x040000A2 RID: 162
        public const string TEMPORAL_PREFIX = "WorkshopBrowser_CDO_";

        // Token: 0x040000A3 RID: 163
        private bool _canRereshItems;

        // Token: 0x040000A4 RID: 164
        private int _selectedLeveltype;

        // Token: 0x040000A5 RID: 165
        private int _currentPage = 1;

        // Token: 0x040000A6 RID: 166
        private string _selectedTabID;

        // Token: 0x040000A7 RID: 167
        private List<NewWorkshopBrowserUI.ItemDisplay> _spawnedItems = new List<NewWorkshopBrowserUI.ItemDisplay>();

        // Token: 0x040000A8 RID: 168
        private List<string> _tabButtonImages_IDs = new List<string>();

        // Token: 0x040000A9 RID: 169
        private SteamWorkshopItem _selectedItem;

        // Token: 0x040000AA RID: 170
        private NewWorkshopBrowserUI.BrowserState _currentState = NewWorkshopBrowserUI.BrowserState.Idle;

        // Token: 0x040000AB RID: 171
        private bool _hasShownEarlier;

        // Token: 0x040000AC RID: 172
        private bool _hasToRefreshTabs = true;

        // Token: 0x0200008E RID: 142
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
                this._itemTitle = this._moddedObject.GetObjectFromList<Text>(3);
                this._itemMadeBy = this._moddedObject.GetObjectFromList<Text>(4);
                this._itemDesc = this._moddedObject.GetObjectFromList<InputField>(6);
                this._goToGameMode = this._moddedObject.GetObjectFromList<Text>(10);
                this._optionsButton = this._moddedObject.GetObjectFromList<Button>(1);
                this._optionsButton.onClick.AddListener(new UnityAction(this.toggleOptionsMenu));
                this._playButton = this._moddedObject.GetObjectFromList<Button>(0);
                this._playButton.onClick.AddListener(delegate ()
                {
                    if (this._browser.GetCurrentLevelType() != 0 && this._browser.GetCurrentLevelType() != 3)
                    {
                        if (Singleton<WorkshopChallengeManager>.Instance.StartChallengeFromWorkshop(this._browser.GetSelectedWorkshopItem()))
                        {
                            this._browser.Hide();
                            return;
                        }
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
                                Singleton<GameFlowManager>.Instance.StartEndlessModeGame();
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

            // Token: 0x060003AA RID: 938 RVA: 0x000143DC File Offset: 0x000125DC
            private void viewItemSteamPage()
            {
                string str = "https://steamcommunity.com/sharedfiles/filedetails/?id=";
                PublishedFileId_t workshopItemID = this._browser.GetSelectedWorkshopItem().WorkshopItemID;
                BaseUtils.OpenURL(str + workshopItemID.ToString());
            }

            // Token: 0x060003AB RID: 939 RVA: 0x00014418 File Offset: 0x00012618
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

            // Token: 0x060003AC RID: 940 RVA: 0x00014479 File Offset: 0x00012679
            private void Update()
            {
                if (this._isDownloading)
                {
                    this.refreshIsDownloading(true);
                }
            }

            // Token: 0x040002C3 RID: 707
            public const string NoItemsSelectedColor = "#292A30";

            // Token: 0x040002C4 RID: 708
            public const string ItemsSelectedColor = "#108424";

            // Token: 0x040002C5 RID: 709
            private NewWorkshopBrowserUI _browser;

            // Token: 0x040002C6 RID: 710
            private ImageLoader _imageLoader;

            // Token: 0x040002C7 RID: 711
            private ModdedObject _moddedObject;

            // Token: 0x040002C8 RID: 712
            private Transform _optionsMenuTransform;

            // Token: 0x040002C9 RID: 713
            private Button _steamPageButton;

            // Token: 0x040002CA RID: 714
            private Button _resetProgress;

            // Token: 0x040002CB RID: 715
            private Text _itemTitle;

            // Token: 0x040002CC RID: 716
            private InputField _itemDesc;

            // Token: 0x040002CD RID: 717
            private Text _itemMadeBy;

            // Token: 0x040002CE RID: 718
            private Text _goToGameMode;

            // Token: 0x040002CF RID: 719
            private Button _playButton;

            // Token: 0x040002D0 RID: 720
            private Button _subscribeButton;

            // Token: 0x040002D1 RID: 721
            private Button _optionsButton;

            // Token: 0x040002D2 RID: 722
            private Slider _downloadProgressSlider;

            // Token: 0x040002D3 RID: 723
            private Text _downloadState;

            // Token: 0x040002D4 RID: 724
            private bool _isDownloading;

            // Token: 0x040002D5 RID: 725
            private bool _showingOptions;

            // Token: 0x040002D6 RID: 726
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

            // Token: 0x060003B4 RID: 948 RVA: 0x000147D3 File Offset: 0x000129D3
            public void UpdatePages()
            {
                this._query = Singleton<SteamWorkshopManager>.Instance.GetNumPagesForLastQuery();
                this.updateFirstAndLastPageButtons();
                this.updateNextAndPrevPageButtons();
                this._currentPageText.text = this._browser._currentPage.ToString();
            }

            // Token: 0x060003B5 RID: 949 RVA: 0x0001480C File Offset: 0x00012A0C
            public void SetPage(int page)
            {
                if (!this._browser.AllowSwitching())
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

            // Token: 0x040002D7 RID: 727
            private NewWorkshopBrowserUI _browser;

            // Token: 0x040002D8 RID: 728
            private ModdedObject _moddedObject;

            // Token: 0x040002D9 RID: 729
            private int _query;

            // Token: 0x040002DA RID: 730
            private bool _pageViewActive;

            // Token: 0x040002DB RID: 731
            private Button _nextPage;

            // Token: 0x040002DC RID: 732
            private Button _prevPage;

            // Token: 0x040002DD RID: 733
            private Button _choosePageButton;

            // Token: 0x040002DE RID: 734
            private Button _firstPage;

            // Token: 0x040002DF RID: 735
            private Text _firstPageText;

            // Token: 0x040002E0 RID: 736
            private Button _lastPage;

            // Token: 0x040002E1 RID: 737
            private Text _lastPageText;

            // Token: 0x040002E2 RID: 738
            private Text _currentPageText;

            // Token: 0x040002E3 RID: 739
            private Transform _pageViewMain;

            // Token: 0x040002E4 RID: 740
            private Button _pageViewEntry;

            // Token: 0x040002E5 RID: 741
            private Transform _pageViewContainer;

            // Token: 0x020000D2 RID: 210
            public class PageButton
            {
                // Token: 0x060004DF RID: 1247 RVA: 0x000188A8 File Offset: 0x00016AA8
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
