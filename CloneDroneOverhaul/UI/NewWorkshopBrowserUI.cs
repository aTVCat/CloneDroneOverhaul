using CloneDroneOverhaul.Modules;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using CloneDroneOverhaul.UI.Components;
using Steamworks;

namespace CloneDroneOverhaul.UI
{
    public class NewWorkshopBrowserUI : ModGUIBase
    {
        public static NewWorkshopBrowserUI Instance;
        private ItemDetailsUI _itemDetailsUI;
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

        public const string TEMPORAL_PREFIX = "WorkshopBrowser_CDO_";

        private bool _canRereshItems;
        private int _selectedLeveltype;
        private int _currentPage = 1;
        private string _selectedTabID;

        private List<ItemDisplay> _spawnedItems = new List<ItemDisplay>();

        private SteamWorkshopItem _selectedItem;

        public override void OnInstanceStart()
        {
            Instance = this;
            base.MyModdedObject = base.GetComponent<ModdedObject>();

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(Hide);

            _itemDetailsUI = MyModdedObject.GetObjectFromList<ModdedObject>(8).gameObject.AddComponent<ItemDetailsUI>();
            _itemDetailsUI.Initialize(this);

            Hide();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            populateTabsAndLevelTypeDropdowns();

            _canRereshItems = true;
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);

            if (GameModeManager.IsOnTitleScreen())
            {
                ArenaManager.SetRootAndLogoVisible(true);
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

        public void SetSelectedWorkshopItem(SteamWorkshopItem item)
        {
            _selectedItem = item;
        }

        /// <summary>
        /// Populates all tabs which decide, what items we want from
        /// </summary>
        void populateTabsAndLevelTypeDropdowns()
        {
            if (_hasPopulatedTabs)
            {
                return;
            }
            _canRereshItems = false;
            _hasPopulatedTabs = true;

            foreach (string tabName in TAB_IDS)
            {
                ModdedObject mObj = Instantiate<ModdedObject>(MyModdedObject.GetObjectFromList<ModdedObject>(2), MyModdedObject.GetObjectFromList<Transform>(3));
                mObj.GetObjectFromList<Text>(0).text = tabName;

                ReferenceOnClick.CallOnClickWithReference(mObj.gameObject, onTabSelected);

                OverhaulCacheManager.AddTemporalObject<string>(tabName, TEMPORAL_PREFIX + mObj.gameObject.GetInstanceID());

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
            _selectedLeveltype = value;
            populateWorkshopItems();
        }

        /// <summary>
        /// Calls when we click tab button
        /// </summary>
        /// <param name="tabText"></param>
        void onTabSelected(MonoBehaviour tab)
        {
            _selectedTabID = OverhaulCacheManager.GetTemporalObject<string>(TEMPORAL_PREFIX + tab.gameObject.GetInstanceID());
            populateWorkshopItems();
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
            if(tabID == TAB_IDS[5])
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
            Singleton<SteamWorkshopManager>.Instance.GetAllNewWorkshopItems(LEVEL_TYPES[selectLevelType], queryType, page, delegate(List<SteamWorkshopItem> items)
            {
                result = items;
                if(onReceive != null)
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

            if(tabID == TAB_IDS[1])
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
        void populateWorkshopItems()
        {
            if (!_canRereshItems)
            {
                return;
            }

            clearExistingItems();

            GetWorkshopItems(_selectedLeveltype, _selectedTabID, _currentPage, EUGCQueryByTabID(_selectedTabID), delegate(List<SteamWorkshopItem> items)
            {
                base.StartCoroutine(asyncPopulateWorkshopItems(items));
            });
        }

        IEnumerator asyncPopulateWorkshopItems(List<SteamWorkshopItem> items)
        {
            foreach(SteamWorkshopItem item in items)
            {
                yield return new WaitForEndOfFrame();

                if (!_canRereshItems)
                {
                    yield break;
                }

                ModdedObject itemMObject = Instantiate<ModdedObject>(MyModdedObject.GetObjectFromList<ModdedObject>(6), MyModdedObject.GetObjectFromList<Transform>(7));
                itemMObject.GetObjectFromList<Text>(1).text = item.Title;
                bool hasCompleted = ChallengeManager.Instance.HasCompletedChallenge(item.WorkshopItemID.ToString());
                itemMObject.GetObjectFromList<Transform>(2).gameObject.SetActive(hasCompleted);
                itemMObject.gameObject.AddComponent<ItemDisplay>().Initialize(this, item);

                itemMObject.gameObject.SetActive(true);
            }

            yield break;
        }

        /// <summary>
        /// Clears spawned items
        /// </summary>
        void clearExistingItems()
        {
            base.StopAllCoroutines();
            TransformUtils.DestroyAllChildren(MyModdedObject.GetObjectFromList<Transform>(7));
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
            if(_spawnedItems.Contains(display)) _spawnedItems.Remove(display);
        }

        /// <summary>
        /// ItemDisplay.SetSelected(false);
        /// </summary>
        public void DeselectAllItems()
        {
            foreach(ItemDisplay disp in _spawnedItems)
            {
                disp.SetSelected(false);
            }
        }

        /// <summary>
        /// Refereshes info about selected item when we click on it
        /// </summary>
        /// <param name="display"></param>
        public void OnItemClicked(ItemDisplay display)
        {
            _selectedItem = display.SteamItem;
            _itemDetailsUI.PopulateItemInfo(_selectedItem, display.ImageLoader.ImageComponent.sprite);
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
                _browserReference.OnItemClicked(this);
                _browserReference.DeselectAllItems();
                SetSelected(true);
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

            Text _itemTitle;
            InputField _itemDesc;
            Text _itemMadeBy;

            Text _goToGameMode;

            Button _playButton;
            Button _subscribeButton;
            Slider _downloadProgressSlider;

            Text _downloadState;

            private bool _isDownloading;

            public void Initialize(NewWorkshopBrowserUI browser)
            {
                _browser = browser;

                base.GetComponent<Image>().color = BaseUtils.ColorFromHex(NoItemsSelectedColor);

                _moddedObject = base.GetComponent<ModdedObject>();
                _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(false);

                _imageLoader = _moddedObject.GetObjectFromList<ModdedObject>(5).gameObject.AddComponent<ImageLoader>();
                _optionsMenuTransform = _moddedObject.GetObjectFromList<Transform>(2);
                _optionsMenuTransform.gameObject.SetActive(false);

                _itemTitle = _moddedObject.GetObjectFromList<Text>(3);
                _itemMadeBy = _moddedObject.GetObjectFromList<Text>(4);
                _itemDesc = _moddedObject.GetObjectFromList<InputField>(6);

                _goToGameMode = _moddedObject.GetObjectFromList<Text>(10);

                _playButton = _moddedObject.GetObjectFromList<Button>(0);
                _playButton.onClick.AddListener(delegate
                {
                    if(_browser.GetCurrentLevelType() != 0 && _browser.GetCurrentLevelType() != 3) // If challange or adventure // TODO - transition
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
                _moddedObject.GetObjectFromList<Transform>(9).gameObject.SetActive(true);
                base.GetComponent<Image>().color = BaseUtils.ColorFromHex(ItemsSelectedColor);

                _itemTitle.text = item.Title;
                _itemDesc.text = item.Description;
                _itemMadeBy.text = item.CreatorName;
                if(sprite == null || sprite.name == "placeholderLoad")
                {
                    _imageLoader.LoadImage(item.PreviewURL);
                }
                else
                {
                    _imageLoader.SetImage(sprite);
                }

                RefreshPlayButtons();
                refreshIsDownloading();
            }

            public void RefreshPlayButtons()
            {
                _goToGameMode.gameObject.SetActive(false);
                SteamWorkshopItem item = _browser.GetSelectedWorkshopItem();
                if(item == null)
                {
                    return;
                }

                bool isSubscribed = !string.IsNullOrEmpty(item.Folder);

                if(BaseUtils.SteamWorkshopUtils.GetItemLoadProgress(item) != -1)
                {
                    _playButton.gameObject.SetActive(false);
                    _subscribeButton.gameObject.SetActive(false);
                    _downloadProgressSlider.gameObject.SetActive(true);
                    refreshIsDownloading(true);
                    return;
                }

                _playButton.gameObject.SetActive(isSubscribed);
                _subscribeButton.gameObject.SetActive(!isSubscribed);

                _downloadProgressSlider.gameObject.SetActive(false);
            }

            private void refreshIsDownloading(bool isCalledDuringDownload = false)
            {
                if(!isCalledDuringDownload) _isDownloading = false;
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

                if(percentage == -1)
                {
                    _downloadState.text = "Waiting download to start...";
                }
                else
                {
                    _downloadState.text = "Downloading";
                }
            }

            void subscribeToItem()
            {
                SteamWorkshopItem item = _browser.GetSelectedWorkshopItem();
                if (item != null)
                {
                    SteamWorkshopManager.Instance.SubscribeToItem(item.WorkshopItemID, delegate(SteamWorkshopItem itemOut) 
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

            void Update()
            {
                if(_isDownloading)
                {
                    refreshIsDownloading(true);
                }
            }
        }
    }
}
