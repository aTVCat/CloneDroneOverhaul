using CDOverhaul.HUD;
using CDOverhaul.NetworkAssets;
using ModLibrary;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.Workshop
{
    // Todo: Complete all OnDisposed methods
    public class OverhaulWorkshopBrowserUI : OverhaulUI
    {
        #region Settings

        [OverhaulSetting("Game interface.Network.New Workshop browser design", true)]
        public static bool UseThisUI;

        [OverhaulSetting("Game interface.Network.Cache images", false, false, null, null, null, "Game interface.Network.New Workshop browser design")]
        public static bool CacheImages;


        public static readonly List<Dropdown.OptionData> LevelTypes = new List<Dropdown.OptionData>
        {
            new DropdownStringOptionData
            {
                    text = "Endless Levels",
                    StringValue = "Endless Level"
            },
            new DropdownStringOptionData
            {
                    text = "Challenges",
                    StringValue = "Challenge"
            },
            new DropdownStringOptionData
            {
                    text = "Last Bot Standing Levels",
                    StringValue = "Last Bot Standing Level"
            },
            new DropdownStringOptionData
            {
                    text = "Adventures",
                    StringValue = "Adventure"
            },
        };

        public static string IconDirectory => OverhaulMod.Core.ModDirectory + "Assets/Workshop/Ico/";

        public static readonly EUGCQuery[] GenericRanks = new EUGCQuery[]
        {
            EUGCQuery.k_EUGCQuery_RankedByVote,
            EUGCQuery.k_EUGCQuery_RankedByTrend,
            EUGCQuery.k_EUGCQuery_RankedByPublicationDate,
            EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions
        };

        public static readonly EUGCQuery[] FriendRanks = new EUGCQuery[]
        {
            EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate,
            EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate,
            EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate,
        };

        public static readonly EUGCQuery[] UniqueRanks = new EUGCQuery[]
        {
            EUGCQuery.k_EUGCQuery_RankedByTextSearch,
            EUGCQuery.k_EUGCQuery_RankedByNumTimesReported,
        };

        #endregion

        #region Patches

        public bool TryShow()
        {
            if (OverhaulVersion.IsUpdate2Hotfix || !UseThisUI)
            {
                return false;
            }

            Show();
            return true;
        }

        #endregion

        public static bool BrowserIsNull => Instance == null;
        public static OverhaulWorkshopBrowserUI Instance;
        public static bool IsActive => !BrowserIsNull && Instance.gameObject.activeSelf;

        private static readonly List<Texture> m_LoadedTextures = new List<Texture>();
        private static ItemUIEntry[] m_SpawnedEntries;

        #region State

        public string LevelTypeRequiredTag
        {
            get;
            private set;
        }
        public int Page
        {
            get;
            private set;
        }
        public EUGCQuery RequiredRank
        {
            get;
            private set;
        }
        public OverhaulWorkshopItem ViewingWorkshopItem
        {
            get;
            private set;
        }

        public int PageCount;
        public int ItemSelectionIndex;

        public bool? CurrentItemVote;

        public OverhaulRequestProgressInfo CurrentRequestProgress;
        public OverhaulWorkshopRequestResult CurrentRequestResult;
        public bool IsPopulatingItems
        {
            get;
            private set;
        }

        private float m_UnscaledTimeClickedOnOption;
        private float m_TimeToAllowPressingReloadButton;
        private float m_TimeToAllowUsingArrowKeys;

        #endregion

        public bool ShouldShowManagementPanel()
        {
            return ViewingWorkshopItem != null;
        }

        public bool ShouldMakePlayButtonInteractable()
        {
            return LevelTypeRequiredTag == "Challenge" || LevelTypeRequiredTag == "Adventure";
        }

        public bool ShouldMakeEraseDataButtonInteractable()
        {
            if (ViewingWorkshopItem == null)
            {
                return false;
            }

            string path = Application.persistentDataPath + "/ChallengeData" + ViewingWorkshopItem.ItemID + ".json";
            return ShouldMakePlayButtonInteractable() && File.Exists(path);
        }

        public bool ShouldResetRequest()
        {
            return IsPopulatingItems && Time.unscaledTime >= m_UnscaledTimeClickedOnOption + 4f;
        }

        #region UI elements

        private Button m_ExitButton;

        private PrefabAndContainer m_WorkshopItemsContainer;
        private PrefabAndContainer m_LevelTypesContainer;
        private PrefabAndContainer m_RanksContainer;
        private PrefabAndContainer m_DropdownRanksContainer;
        private PrefabAndContainer m_RankSeparatorsContainer;
        private LoadingIndicator m_LoadingIndicator;

        private GameObject m_RanksDropdown;

        private GameObject m_QuickInfo;
        private Text m_QuickInfoLevelName;
        private Text m_QuickInfoLevelDesc;
        private Text m_QuickInfoLevelStars;
        private Text m_QuickInfoUserName;
        private RawImage m_QuickInfoUserAvatar;

        private GameObject m_ErrorWindow;
        private Button m_ErrorWindowRetryButton;

        private Transform m_ManagementButtonsContainer;
        private Transform m_ItemLoadingIndicatorTransform;
        private Button m_SubscribeButton;
        private Button m_UnsubscribeButton;
        private Button m_PlayButton;
        private Button m_EraseDataButton;
        private Button m_ReloadButton;

        private Button m_UpVoteButton;
        private Button m_DownVoteButton;
        private Button m_FavouriteButton;
        private Button m_ItemSteamPageButton;
        private Button m_ItemCreatorPageButton;
        private Button m_CopyItemLinkButton;

        private Button m_PageSelectionButton;
        private Text m_CurrentPageText;
        private Transform m_PageSelectionTransform;
        private PrefabAndContainer m_PageContainer;
        private Button m_ReloadPageButton;

        private PrefabAndContainer m_AdditionalPreviewsContainer;
        private LoadingIndicator m_ItemDownloadLI;

        #endregion

        #region Initilaizing & disposing

        public override void Initialize()
        {
            Instance = this;

            m_UpVoteButton = MyModdedObject.GetObject<Button>(27);
            m_UpVoteButton.onClick.AddListener(VoteUp);
            m_DownVoteButton = MyModdedObject.GetObject<Button>(28);
            m_DownVoteButton.onClick.AddListener(VoteDown);
            m_FavouriteButton = MyModdedObject.GetObject<Button>(29);
            m_FavouriteButton.onClick.AddListener(MarkItemAsFavourite);
            m_ItemSteamPageButton = MyModdedObject.GetObject<Button>(30);
            m_ItemSteamPageButton.onClick.AddListener(OpenItemSteamPage);
            m_ItemCreatorPageButton = MyModdedObject.GetObject<Button>(43);
            m_ItemCreatorPageButton.onClick.AddListener(OpenItemAuthorPage);
            m_CopyItemLinkButton = MyModdedObject.GetObject<Button>(44);
            m_CopyItemLinkButton.onClick.AddListener(CopyItemLink);
            m_ReloadButton = MyModdedObject.GetObject<Button>(45);
            m_ReloadButton.onClick.AddListener(ReloadItemView);

            m_WorkshopItemsContainer = new PrefabAndContainer(MyModdedObject, 1, 2);
            m_LevelTypesContainer = new PrefabAndContainer(MyModdedObject, 9, 10);
            m_RanksContainer = new PrefabAndContainer(MyModdedObject, 15, 16);
            m_DropdownRanksContainer = new PrefabAndContainer(MyModdedObject, 15, 19);
            m_RankSeparatorsContainer = new PrefabAndContainer(MyModdedObject, 17, 16);
            m_AdditionalPreviewsContainer = new PrefabAndContainer(MyModdedObject, 37, 38);
            m_LoadingIndicator = new LoadingIndicator(MyModdedObject, 3, 4);
            m_ItemDownloadLI = new LoadingIndicator(MyModdedObject, 34, 35);
            m_RanksDropdown = MyModdedObject.GetObject<Transform>(18).gameObject;
            m_RanksDropdown.gameObject.SetActive(false);
            m_ExitButton = MyModdedObject.GetObject<Button>(0);
            m_ExitButton.onClick.AddListener(Hide);
            m_QuickInfo = MyModdedObject.GetObject<Transform>(5).gameObject;
            m_QuickInfoLevelName = MyModdedObject.GetObject<Text>(6);
            m_QuickInfoLevelDesc = MyModdedObject.GetObject<Text>(7);
            m_QuickInfoLevelStars = MyModdedObject.GetObject<Text>(8);
            m_ErrorWindow = MyModdedObject.GetObject<Transform>(11).gameObject;
            m_ErrorWindow.SetActive(false);
            m_ErrorWindowRetryButton = MyModdedObject.GetObject<Button>(12);
            m_ErrorWindowRetryButton.onClick.AddListener(RetryRequest);
            m_QuickInfoUserName = MyModdedObject.GetObject<Text>(13);
            m_QuickInfoUserAvatar = MyModdedObject.GetObject<RawImage>(14);
            m_QuickInfoUserAvatar.gameObject.SetActive(false);

            m_ItemPageViewTransform = MyModdedObject.GetObject<Transform>(20);
            m_ItemPageViewTransform.gameObject.SetActive(false);
            m_PageTransform = MyModdedObject.GetObject<Transform>(21);
            _ = m_PageTransform.gameObject.AddComponent<ItemViewPageBehaviour>();
            m_ReloadPageButton = MyModdedObject.GetObject<Button>(51);
            m_ReloadPageButton.onClick.AddListener(RefreshLevelsList);

            m_PageSelectionButton = MyModdedObject.GetObject<Button>(49);
            m_PageSelectionButton.onClick.AddListener(TogglePageSelectionPanel);
            m_CurrentPageText = MyModdedObject.GetObject<Text>(50);
            m_CurrentPageText.text = "Page [1]";
            m_PageSelectionTransform = MyModdedObject.GetObject<Transform>(46);
            m_PageSelectionTransform.gameObject.SetActive(false);
            m_PageContainer = new PrefabAndContainer(MyModdedObject, 47, 48);
            Page = 1;

            m_ManagementButtonsContainer = MyModdedObject.GetObject<Transform>(41);
            m_ItemLoadingIndicatorTransform = MyModdedObject.GetObject<Transform>(34);
            m_SubscribeButton = MyModdedObject.GetObject<Button>(31);
            m_UnsubscribeButton = MyModdedObject.GetObject<Button>(42);
            m_UnsubscribeButton.onClick.AddListener(UnsubscribeFromItem);
            m_PlayButton = MyModdedObject.GetObject<Button>(32);
            m_PlayButton.onClick.AddListener(OnPlayButtonClicked);
            m_EraseDataButton = MyModdedObject.GetObject<Button>(33);
            m_EraseDataButton.onClick.AddListener(EraseLevelProgress);

            MyModdedObject.GetObject<Button>(39).onClick.AddListener(delegate
            {
                OverhaulUIImageViewer.SetActive(true, MyModdedObject.GetObject<RawImage>(39).texture);
            });
            MyModdedObject.GetObject<Button>(31).onClick.AddListener(SubscribeToItem);

            LevelTypeRequiredTag = "Adventure";
            RequiredRank = EUGCQuery.k_EUGCQuery_RankedByTrend;

            Hide(true);
        }

        public void PopulateLevelTypes()
        {
            m_LevelTypesContainer.ClearContainer();
            foreach (DropdownStringOptionData data in LevelTypes)
            {
                ModdedObject m = m_LevelTypesContainer.CreateNew();
                _ = LevelTypeUIEntry.CreateNew(m, data.StringValue);
                m.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(data.text);
            }
        }

        public void PopulateRanks()
        {
            m_RanksContainer.ClearContainer();
            populateRankArray(GenericRanks);
            populateRankArray(FriendRanks);
        }

        private void populateRankArray(EUGCQuery[] array)
        {
            if (array.IsNullOrEmpty())
            {
                return;
            }

            _ = m_RankSeparatorsContainer.CreateNew();
            int i = 0;
            do
            {
                if (i == 3 && array.Length > 4)
                {
                    ModdedObject moreButton = m_RanksContainer.CreateNew();
                    moreButton.GetObject<Text>(0).text = string.Format("More [{0}]", array.Length - i);
                    moreButton.GetObject<Transform>(2).gameObject.SetActive(false);
                    moreButton.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        ShowRanksDropdown(array, 3, moreButton.transform.position.y);
                    });

                    OverhaulNetworkDownloadHandler iconDownloadHandler = new OverhaulNetworkDownloadHandler();
                    iconDownloadHandler.DoneAction = delegate
                    {
                        if (moreButton != null && iconDownloadHandler != null && !iconDownloadHandler.Error)
                        {
                            RawImage ri = moreButton.GetObject<RawImage>(1);
                            ri.texture = iconDownloadHandler.DownloadedTexture;
                            ri.texture.filterMode = FilterMode.Point;
                            (ri.texture as Texture2D).Apply();
                            return;
                        }
                        if (iconDownloadHandler != null && iconDownloadHandler.DownloadedTexture) Destroy(iconDownloadHandler.DownloadedTexture);
                    };
                    OverhaulNetworkController.DownloadTexture(IconDirectory + "More.png", iconDownloadHandler);
                    return;
                }
                ModdedObject m = m_RanksContainer.CreateNew();
                m.GetObject<Text>(0).text = getRankString(array[i]);
                _ = RankUIEntry.CreateNew(m, array[i]);
                i++;
            } while (i < array.Length);
        }
        private string getRankString(EUGCQuery rank)
        {
            switch (rank)
            {
                case EUGCQuery.k_EUGCQuery_RankedByVote:
                    return "Popular";
                case EUGCQuery.k_EUGCQuery_RankedByTrend:
                    return "Trending";
                case EUGCQuery.k_EUGCQuery_RankedByPublicationDate:
                    return "Recent";
                case EUGCQuery.k_EUGCQuery_RankedByTotalVotesAsc:
                    return "Most rated";
                case EUGCQuery.k_EUGCQuery_NotYetRated:
                    return "Not rated";
                case EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions:
                    return "Most subscribers";
                case EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate:
                    return "Friends creations";
                case EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate:
                    return "Friends favourite";
                case EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate:
                    return "Your follows";
            }
            return rank.ToString();
        }

        public void ShowRanksDropdown(EUGCQuery[] array, int startIndex, float worldY)
        {
            if (m_RanksDropdown.gameObject.activeSelf)
            {
                m_RanksDropdown.gameObject.SetActive(false);
                return;
            }

            m_DropdownRanksContainer.ClearContainer();
            if (array.IsNullOrEmpty())
            {
                return;
            }

            int i = startIndex;
            do
            {
                ModdedObject m = m_DropdownRanksContainer.CreateNew();
                m.GetObject<Text>(0).text = getRankString(array[i]);
                _ = RankUIEntry.CreateNew(m, array[i]);
                i++;
            } while (i < array.Length);

            m_RanksDropdown.gameObject.SetActive(true);
            m_RanksDropdown.transform.position = new Vector3(m_RanksDropdown.transform.position.x, worldY, m_RanksDropdown.transform.position.z);
        }

        public void HideRanksDropdown()
        {
            m_RanksDropdown.gameObject.SetActive(false);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Instance = null;
        }

        #endregion

        #region Request parameters

        public void SetTag(string requiredTag, bool refreshLevelsList = true)
        {
            if (IsPopulatingItems || requiredTag == LevelTypeRequiredTag)
            {
                return;
            }

            LevelTypeRequiredTag = requiredTag;
            if (refreshLevelsList)
            {
                RefreshLevelsList();
            }
        }

        public void SetRank(EUGCQuery rank, bool refreshLevelsList = true)
        {
            if (IsPopulatingItems || rank == RequiredRank)
            {
                return;
            }

            HideRanksDropdown();

            RequiredRank = rank;
            if (refreshLevelsList)
            {
                RefreshLevelsList();
            }
        }

        #endregion

        #region Viewing the item

        private Animation m_Animator;
        private Transform m_ItemPageViewTransform;
        private Transform m_PageTransform;

        private readonly OverhaulRequestProgressInfo m_ProgressInfo = new OverhaulRequestProgressInfo();

        public void ViewItem(OverhaulWorkshopItem workshopItem)
        {
            ViewingWorkshopItem = workshopItem;

            // dispose textures first
            m_ItemPageViewTransform.gameObject.SetActive(false);
            RawImage ri = MyModdedObject.GetObject<RawImage>(39);
            if (ri.texture != null)
            {
                Destroy(ri.texture);
            }

            OverhaulRequestProgressInfo.SetProgress(m_ProgressInfo, 0f);
            LoadingIndicator.ResetIndicator(m_ItemDownloadLI);
            StaticCoroutineRunner.StopStaticCoroutine(waitUntilLevelIsDownloaded());
            m_ItemLoadingIndicatorTransform.gameObject.SetActive(false);

            bool itemIsNUll = workshopItem == null;
            if (itemIsNUll)
            {
                return;
            }

            RefreshManagementPanel();

            m_FavouriteButton.image.color = "#2E2E2E".ConvertHexToColor();
            m_FavouriteButton.interactable = true;
            m_ItemPageViewTransform.gameObject.SetActive(true);
            _ = m_Animator.Play("WorkshopUIViewItem");

            MyModdedObject.GetObject<Text>(23).text = workshopItem.ItemTitle;
            MyModdedObject.GetObject<Text>(24).text = workshopItem.CreatorNickname;
            MyModdedObject.GetObject<Text>(25).text = workshopItem.TimeCreated.ToShortDateString() + "\n" + workshopItem.TimeUpdated.ToShortDateString() + "\n" + workshopItem.ItemSizeString;
            MyModdedObject.GetObject<Text>(26).text = workshopItem.ItemLongDescription;

            OverhaulNetworkDownloadHandler hm = new OverhaulNetworkDownloadHandler();
            hm.DoneAction = delegate
            {
                if (hm != null && !hm.Error && ri != null)
                {
                    ri.texture = hm.DownloadedTexture;
                }
            };
            OverhaulNetworkController.DownloadTexture(workshopItem.PreviewURL, hm);

            int imagesCount = Mathf.Min(2, workshopItem.ItemAdditionalImages.Count);
            m_AdditionalPreviewsContainer.ClearContainer();
            if (imagesCount == 0)
            {
                return;
            }

            for (int i = 0; i < imagesCount; i++)
            {
                ModdedObject m = m_AdditionalPreviewsContainer.CreateNew();
                OverhaulNetworkDownloadHandler h = new OverhaulNetworkDownloadHandler();
                h.DoneAction = delegate
                {
                    if (hm != null && !hm.Error && m != null)
                    {
                        m.GetObject<RawImage>(0).texture = h.DownloadedTexture;
                        m_LoadedTextures.Add(h.DownloadedTexture);
                        m.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            if (m != null)
                            {
                                OverhaulUIImageViewer.SetActive(true, m.GetObject<RawImage>(0).texture);
                            }
                        });
                    }
                };
                OverhaulNetworkController.DownloadTexture(workshopItem.ItemAdditionalImages[i], h);
            }
        }

        public void ReloadItemView()
        {
            ViewItem(ViewingWorkshopItem);
        }

        public void SubscribeToItem()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            m_ItemLoadingIndicatorTransform.gameObject.SetActive(true);
            m_SubscribeButton.interactable = false;
            OverhaulSteamBrowser.SubscribeToItem(ViewingWorkshopItem.ItemID, delegate (PublishedFileId_t t, EResult r)
            {
                if (r == EResult.k_EResultOK)
                {
                    _ = StaticCoroutineRunner.StartStaticCoroutine(waitUntilLevelIsDownloaded());
                    return;
                }
                RefreshManagementPanel();
            });
        }

        private IEnumerator waitUntilLevelIsDownloaded()
        {
            while (true)
            {
                if (ViewingWorkshopItem == null)
                {
                    yield break;
                }

                EItemState itemState = OverhaulSteamBrowser.GetItemState(ViewingWorkshopItem.ItemID);
                if (itemState.HasFlag(EItemState.k_EItemStateInstalled) && itemState.HasFlag(EItemState.k_EItemStateSubscribed))
                {
                    DelegateScheduler.Instance.Schedule(delegate
                    {
                        if (ViewingWorkshopItem != null) RefreshManagementPanel();
                    }, 2f);
                    yield break;
                }
                yield return null;
            }
            yield break;
        }

        public void UnsubscribeFromItem()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            m_PlayButton.interactable = false;
            m_UnsubscribeButton.interactable = false;
            OverhaulSteamBrowser.UnsubscribeFromItem(ViewingWorkshopItem.ItemID, delegate (PublishedFileId_t t, EResult r)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    RefreshManagementPanel();
                }, 0.5f);
            });
        }

        public void MarkItemAsFavourite()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            m_FavouriteButton.image.color = "#2E2E2E".ConvertHexToColor();
            m_FavouriteButton.interactable = false;
            OverhaulSteamBrowser.MarkItemAsFavourite(ViewingWorkshopItem.ItemID, delegate (PublishedFileId_t t, EResult r, bool a)
            {
                m_FavouriteButton.image.color = "#FFDA5A".ConvertHexToColor();
            });
        }

        public void OpenItemSteamPage()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            string url = ViewingWorkshopItem.ItemURL;
            if (SteamManager.Instance != null && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage(url);
                return;
            }
            Application.OpenURL(url);
        }

        public void OpenItemAuthorPage()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            if (SteamManager.Instance != null && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToUser("steamid", ViewingWorkshopItem.CreatorID);
                return;
            }
            string url = "https://steamcommunity.com/profiles/" + ViewingWorkshopItem.CreatorID;
            Application.OpenURL(url);
        }

        public void CopyItemLink()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            TextEditor editor = new TextEditor
            {
                text = ViewingWorkshopItem.ItemURL
            };
            editor.SelectAll();
            editor.Copy();
        }

        public void EraseLevelProgress()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            m_EraseDataButton.interactable = false;
            string path = Application.persistentDataPath + "/ChallengeData" + ViewingWorkshopItem.ItemID + ".json";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                }
            }
        }

        public void VoteUp()
        {
            if (ViewingWorkshopItem == null || CurrentItemVote == true)
            {
                return;
            }

            m_UpVoteButton.interactable = false;
            m_DownVoteButton.interactable = false;

            OverhaulSteamBrowser.SetItemVote(ViewingWorkshopItem.ItemID, true, delegate (SetUserItemVoteResult_t t, bool fail)
            {
                RefreshVoteButtons();
            });
        }

        public void VoteDown()
        {
            if (ViewingWorkshopItem == null || CurrentItemVote == false)
            {
                return;
            }

            m_UpVoteButton.interactable = false;
            m_DownVoteButton.interactable = false;

            OverhaulSteamBrowser.SetItemVote(ViewingWorkshopItem.ItemID, false, delegate (SetUserItemVoteResult_t t, bool fail)
            {
                RefreshVoteButtons();
            });
        }

        public void OnPlayButtonClicked()
        {
            if (ViewingWorkshopItem == null || WorkshopChallengeManager.Instance == null)
            {
                return;
            }

            SteamWorkshopItem item = ViewingWorkshopItem.ToSteamWorkshopItem();
            if (!File.Exists(item.Folder + "\\ExportedChallengeData.json"))
            {
                RefreshManagementPanel();
                return;
            }

            Hide();
            _ = WorkshopChallengeManager.Instance.StartChallengeFromWorkshop(item);
        }

        public void RefreshManagementPanel()
        {
            bool shouldshow = ShouldShowManagementPanel();
            m_ManagementButtonsContainer.gameObject.SetActive(shouldshow);
            if (!shouldshow)
            {
                return;
            }

            PublishedFileId_t itemID = ViewingWorkshopItem.ItemID;
            bool isSubscribed = OverhaulSteamBrowser.IsSubscribedToItem(itemID);
            bool isInstalled = OverhaulSteamBrowser.IsItemInstalled(itemID);
            bool isDownloading = OverhaulSteamBrowser.IsDownloadingItemInAnyWay(itemID);

            m_SubscribeButton.gameObject.SetActive(!isSubscribed);
            m_SubscribeButton.interactable = !isSubscribed;
            m_UnsubscribeButton.gameObject.SetActive(isSubscribed);
            m_UnsubscribeButton.interactable = !isDownloading;
            m_EraseDataButton.interactable = ShouldMakeEraseDataButtonInteractable();
            m_ItemLoadingIndicatorTransform.gameObject.SetActive(isDownloading);

            ViewingWorkshopItem.RefreshAllInfos(delegate
            {
                if (ViewingWorkshopItem != null && MyModdedObject != null)
                {
                    MyModdedObject.GetObject<Text>(24).text = ViewingWorkshopItem.CreatorNickname;
                    m_ItemLoadingIndicatorTransform.gameObject.SetActive(OverhaulSteamBrowser.IsDownloadingItemInAnyWay(itemID));
                }
            });

            RefreshVoteButtons();
            refreshPlayButton(ViewingWorkshopItem);
        }

        private void refreshPlayButton(OverhaulWorkshopItem item)
        {
            m_PlayButton.interactable = false;
            if (item == null)
            {
                return;
            }

            DelegateScheduler.Instance.Schedule(delegate
            {
                bool isSub = OverhaulSteamBrowser.IsSubscribedToItem(item.ItemID);
                bool isIns = OverhaulSteamBrowser.IsItemInstalled(item.ItemID);
                bool isDow = OverhaulSteamBrowser.IsDownloadingItemInAnyWay(item.ItemID);
                m_PlayButton.interactable = ShouldMakePlayButtonInteractable() && isSub && isIns && !isDow && File.Exists(item.FolderPath + "\\ExportedChallengeData.json");
            }, 0.5f);
        }

        public void RefreshVoteButtons()
        {
            if (ViewingWorkshopItem == null)
            {
                return;
            }

            CurrentItemVote = null;
            m_UpVoteButton.interactable = false;
            m_UpVoteButton.image.color = "#414141".ConvertHexToColor();
            m_DownVoteButton.interactable = false;
            m_DownVoteButton.image.color = "#414141".ConvertHexToColor();

            OverhaulSteamBrowser.GetItemVoteInfo(ViewingWorkshopItem.ItemID, delegate (bool skip, bool up, bool down, bool fail)
            {
                if (fail)
                {
                    return;
                }

                m_UpVoteButton.interactable = true;
                if (up)
                {
                    m_UpVoteButton.image.color = "#2DC869".ConvertHexToColor();
                    CurrentItemVote = true;
                }
                m_DownVoteButton.interactable = true;
                if (down)
                {
                    m_DownVoteButton.image.color = "#EC5454".ConvertHexToColor();
                    CurrentItemVote = false;
                }
            });
        }

        #endregion

        #region Page Selection

        public void TogglePageSelectionPanel()
        {
            bool active = m_PageSelectionTransform.gameObject.activeSelf;
            m_PageSelectionTransform.gameObject.SetActive(!active);

            if (!active)
            {
                m_PageContainer.ClearContainer();

                int i = 1;
                do
                {
                    int pageIndex = i;
                    ModdedObject page = m_PageContainer.CreateNew();
                    page.GetObject<Text>(0).text = i.ToString();
                    page.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        Page = pageIndex;
                        RefreshLevelsList();
                    });
                    i++;
                } while (i < PageCount);
            }
        }

        #endregion

        #region Errors

        public void SetErrorWindowActive(bool value)
        {
            m_ErrorWindow.SetActive(value);
        }

        public void RetryRequest()
        {
            ResetRequest();
            RefreshLevelsList();
            SetErrorWindowActive(false);
        }

        #endregion

        public void Show()
        {
            base.gameObject.SetActive(true);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            OverhaulCanvasController.SetCanvasPixelPerfect(false);
            OverhaulUIDescriptionTooltip.SetActive(true, "Steam Workshop Browser", "Play and rate human levels!");
            RefreshLevelsList();
            PopulateLevelTypes();
            PopulateRanks();

            MyModdedObject.GetObject<Transform>(40).gameObject.SetActive(OverhaulVersion.IsDebugBuild);

            if (m_Animator == null)
            {
                m_Animator = GetComponent<Animation>();
                if (m_Animator != null)
                {
                    m_Animator.Stop();
                }
            }
        }

        public void Hide(bool hiddenBecauseLoading = false)
        {
            OverhaulUIDescriptionTooltip.SetActive(false);
            OverhaulCanvasController.SetCanvasPixelPerfect(true);
            base.gameObject.SetActive(false);
            if (!hiddenBecauseLoading) GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
        }
        public void Hide()
        {
            Hide(false);
        }

        private void Update()
        {
            m_ReloadPageButton.interactable = Time.unscaledTime >= m_TimeToAllowPressingReloadButton;

            LoadingIndicator.UpdateIndicator(m_LoadingIndicator, CurrentRequestProgress);
            if (ShouldResetRequest())
            {
                SetErrorWindowActive(true);
                ResetRequest();
            }

            if (ViewingWorkshopItem != null)
            {
                OverhaulSteamBrowser.UpdateItemDownloadInfo(ViewingWorkshopItem.ItemID, m_ProgressInfo);
                LoadingIndicator.UpdateIndicator(m_ItemDownloadLI, m_ProgressInfo);
            }

            if (OverhaulVersion.IsDebugBuild && Time.frameCount % 3 == 0 && ViewingWorkshopItem != null)
            {
                MyModdedObject.GetObject<Text>(40).text = string.Format("Item name: {0}\nState: {1}\nD_Progress: {2}", new object[] { ViewingWorkshopItem.ItemTitle, OverhaulSteamBrowser.GetItemState(ViewingWorkshopItem.ItemID).ToString(), OverhaulSteamBrowser.GetItemDownloadProgress(ViewingWorkshopItem.ItemID) });
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(ViewingWorkshopItem != null)
                {
                    ViewItem(null);
                    return;
                }
                Hide();
            }

            if (m_SpawnedEntries.IsNullOrEmpty())
            {
                return;
            }

            float time = Time.unscaledTime;
            if(time < m_TimeToAllowUsingArrowKeys) return;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                m_TimeToAllowUsingArrowKeys = time + 0.1f;
                if(ItemSelectionIndex < m_SpawnedEntries.Length - 1)
                {
                    ItemSelectionIndex++;
                }
                refreshScrollRectPosition(ItemSelectionIndex, m_SpawnedEntries.Length);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                m_TimeToAllowUsingArrowKeys = time + 0.1f;
                if (ItemSelectionIndex > 0)
                {
                    ItemSelectionIndex--;
                }
                refreshScrollRectPosition(ItemSelectionIndex, m_SpawnedEntries.Length);
            }
            else if ((ItemSelectionIndex != -1 && ItemSelectionIndex < m_SpawnedEntries.Length) && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            {
                ItemUIEntry entry = m_SpawnedEntries[ItemSelectionIndex];
                if(entry != null && entry.HasWorkshopItem())
                {
                    ViewItem(entry.GetWorkshopItem());
                    ItemSelectionIndex = -1;
                }
            }
        }

        private void refreshScrollRectPosition(int a, int total)
        {
            //MyModdedObject.GetObject<ScrollRect>(52).verticalNormalizedPosition = 1f - ((float)a / (float)total);
        }

        public void ResetRequest()
        {
            IsPopulatingItems = false;
            OverhaulRequestProgressInfo.SetProgress(CurrentRequestProgress, 1f);
            LoadingIndicator.ResetIndicator(m_LoadingIndicator);
            StaticCoroutineRunner.StopStaticCoroutine(populateItemsCoroutine());
            m_UnscaledTimeClickedOnOption = -1f;
            CurrentRequestResult = null;
            CurrentRequestProgress = null;
        }

        public void RefreshLevelsList()
        {
            if (IsPopulatingItems)
            {
                return;
            }

            IsPopulatingItems = true;
            m_CurrentPageText.text = string.Format("Page [{0}]", Page);
            m_WorkshopItemsContainer.ClearContainer();
            m_PageSelectionTransform.gameObject.SetActive(false);
            m_PageSelectionButton.interactable = false;
            m_UnscaledTimeClickedOnOption = Time.unscaledTime;
            m_TimeToAllowPressingReloadButton = m_UnscaledTimeClickedOnOption + 1f;
            CurrentRequestResult = null;
            CurrentRequestProgress = new OverhaulRequestProgressInfo();

            SetErrorWindowActive(false);
            LoadingIndicator.ResetIndicator(m_LoadingIndicator);
            StaticCoroutineRunner.StopStaticCoroutine(populateItemsCoroutine());
            OverhaulSteamBrowser.RequestItems(RequiredRank, Steamworks.EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, OnReceivedWorkshopResult, CurrentRequestProgress, LevelTypeRequiredTag, Page, true, true);
        }

        /// <summary>
        /// Called when our steam workshop request is done
        /// </summary>
        /// <param name="requestResult"></param>
        public void OnReceivedWorkshopResult(OverhaulWorkshopRequestResult requestResult)
        {
            IsPopulatingItems = false;
            CurrentRequestResult = requestResult;
            PageCount = Mathf.Clamp(requestResult.PageCount, 1, int.MaxValue);

            if (requestResult == null || requestResult.Error)
            {
                SetErrorWindowActive(true);
                return;
            }

            m_PageSelectionButton.interactable = true;
            _ = StaticCoroutineRunner.StartStaticCoroutine(populateItemsCoroutine());
        }
        private IEnumerator populateItemsCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            CurrentRequestProgress = null;

            if (!canPopulateItems(CurrentRequestResult))
            {
                yield break;
            }

            ItemUIEntry[] uiItems = new ItemUIEntry[CurrentRequestResult.ItemsReceived.Length];
            m_SpawnedEntries = uiItems;
            int itemIndex = 0;
            do
            {
                if(CurrentRequestResult.ItemsReceived[itemIndex] == null)
                {
                    itemIndex++;
                    continue;
                }

                ItemUIEntry entry = ItemUIEntry.CreateNew(m_WorkshopItemsContainer.CreateNew(), CurrentRequestResult.ItemsReceived[itemIndex], itemIndex);
                uiItems[itemIndex] = entry;
                itemIndex++;
                if (itemIndex % 10 == 0) yield return null;

            } while (canPopulateItems(CurrentRequestResult) && itemIndex < CurrentRequestResult.ItemsReceived.Length);

            if (uiItems.IsNullOrEmpty())
            {
                yield break;
            }

            itemIndex = 0;
            do
            {
                if (uiItems[itemIndex] != null)
                {
                    uiItems[itemIndex].ShowEntry();
                }
                itemIndex++;
                yield return new WaitForSecondsRealtime(0.024f);

            } while (canPopulateItems(CurrentRequestResult) && itemIndex < CurrentRequestResult.ItemsReceived.Length);

            yield break;
        }
        private bool canPopulateItems(OverhaulWorkshopRequestResult requestResult)
        {
            return requestResult != null && !requestResult.ItemsReceived.IsNullOrEmpty();
        }

        public void ShowQuickInfo(OverhaulWorkshopItem workshopItem)
        {
            if (workshopItem == null)
            {
                if (m_QuickInfoUserAvatar.texture != null)
                {
                    Destroy(m_QuickInfoUserAvatar.texture);
                }
                m_QuickInfo.SetActive(false);
                return;
            }
            string stars = workshopItem.Stars.ToString();

            m_QuickInfo.SetActive(true);
            m_QuickInfoLevelName.text = workshopItem.ItemTitle;
            m_QuickInfoLevelDesc.text = workshopItem.ItemLongDescription;
            m_QuickInfoLevelStars.text = stars.Length > 3 ? stars.Remove(3) : stars;
            m_QuickInfoUserName.text = workshopItem.CreatorNickname;
        }

        public class ItemViewPageBehaviour : OverhaulBehaviour
        {
            private void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GameUIRoot.Instance == null || BrowserIsNull || OverhaulUIImageViewer.IsActive)
                    {
                        return;
                    }

                    Graphic g = base.GetComponent<Graphic>();
                    if (g == null)
                    {
                        return;
                    }

                    GraphicRaycaster c = GameUIRoot.Instance.GetComponent<GraphicRaycaster>();
                    if (c == null)
                    {
                        return;
                    }

                    List<Graphic> list = c.GetPrivateField<List<Graphic>>("m_RaycastResults");
                    if (list.IsNullOrEmpty() || list.Contains(g))
                    {
                        return;
                    }

                    Instance.ViewItem(null);
                }
            }
        }

        public class RankUIEntry : OverhaulBehaviour
        {
            public static RankUIEntry CreateNew(ModdedObject moddedObject, EUGCQuery rank)
            {
                RankUIEntry entry = moddedObject.gameObject.AddComponent<RankUIEntry>();
                entry.m_Rank = rank;
                entry.m_SelectedFrame = moddedObject.GetObject<Transform>(2).gameObject;
                entry.m_SelectedFrame.SetActive(false);
                entry.m_RankIcon = moddedObject.GetObject<RawImage>(1);
                entry.getIcon();
                return entry;
            }

            private Button m_Button;
            private RawImage m_RankIcon;
            private GameObject m_SelectedFrame;
            private EUGCQuery m_Rank;

            private void Start()
            {
                m_Button = GetComponent<Button>();
                if (m_Button != null)
                {
                    m_Button.onClick.AddListener(onClicked);
                }
            }

            private void onClicked()
            {
                m_Button.OnDeselect(null);
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                {
                    return;
                }
                OverhaulWorkshopBrowserUI.Instance.Page = 1;
                OverhaulWorkshopBrowserUI.Instance.SetRank(m_Rank, true);
            }

            private void getIcon()
            {
                OverhaulNetworkDownloadHandler iconDownloadHandler = new OverhaulNetworkDownloadHandler();
                iconDownloadHandler.DoneAction = delegate
                {
                    if (iconDownloadHandler != null && !iconDownloadHandler.Error && m_RankIcon != null)
                    {
                        m_RankIcon.texture = iconDownloadHandler.DownloadedTexture;
                        m_RankIcon.texture.filterMode = FilterMode.Point;
                        (m_RankIcon.texture as Texture2D).Apply();
                        return;
                    }
                    if (iconDownloadHandler != null && iconDownloadHandler.DownloadedTexture) Destroy(iconDownloadHandler.DownloadedTexture);
                };
                OverhaulNetworkController.DownloadTexture(IconDirectory + m_Rank.ToString() + ".png", iconDownloadHandler);
            }

            private void Update()
            {
                if (!OverhaulWorkshopBrowserUI.BrowserIsNull && Time.frameCount % 3 == 0)
                {
                    m_SelectedFrame.SetActive(m_Rank == OverhaulWorkshopBrowserUI.Instance.RequiredRank);
                }
            }

            private void OnDestroy()
            {
                if(m_RankIcon && m_RankIcon.texture != null && m_RankIcon.texture.name != "Placeholder-16x16")
                {
                    Destroy(m_RankIcon.texture);
                }
            }
        }

        public class LevelTypeUIEntry : OverhaulBehaviour
        {
            public static LevelTypeUIEntry CreateNew(ModdedObject moddedObject, string tag)
            {
                LevelTypeUIEntry entry = moddedObject.gameObject.AddComponent<LevelTypeUIEntry>();
                entry.m_Tag = tag;
                entry.m_Text = moddedObject.GetObject<Text>(0);
                return entry;
            }

            public static readonly Color DeselectedColor = Color.gray;
            public static readonly Color SelectedColor = Color.white;

            private Button m_Button;
            private Text m_Text;
            private string m_Tag;

            private void Start()
            {
                m_Button = GetComponent<Button>();
                if (m_Button != null)
                {
                    m_Button.onClick.AddListener(onClicked);
                }
            }

            private void onClicked()
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull || string.IsNullOrEmpty(m_Tag))
                {
                    return;
                }
                OverhaulWorkshopBrowserUI.Instance.SetTag(m_Tag, true);
                m_Button.OnDeselect(null);
            }

            private void Update()
            {
                if (!OverhaulWorkshopBrowserUI.BrowserIsNull && Time.frameCount % 3 == 0)
                {
                    m_Text.color = m_Tag == OverhaulWorkshopBrowserUI.Instance.LevelTypeRequiredTag ? SelectedColor : DeselectedColor;
                }
            }
        }

        public class ItemUIEntry : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
        {
            public static ItemUIEntry CreateNew(ModdedObject moddedObject, OverhaulWorkshopItem workshopItem, int index)
            {
                if (workshopItem == null)
                {
                    if (moddedObject != null)
                    {
                        moddedObject.gameObject.SetActive(false);
                    }
                    return null;
                }

                ItemUIEntry entry = moddedObject.gameObject.AddComponent<ItemUIEntry>();
                entry.m_WorkshopItem = workshopItem;
                entry.m_CanvasRenderer = moddedObject.GetObject<CanvasRenderer>(1);
                entry.m_CanvasGroup = entry.GetComponent<CanvasGroup>();
                entry.m_CanvasGroup.alpha = 0f;
                entry.m_TitleText = moddedObject.GetObject<Text>(0);
                entry.m_ThumbnailImage = moddedObject.GetObject<RawImage>(2);
                entry.m_ThumbnailProgressBar = moddedObject.GetObject<Image>(3);
                entry.m_HoverOutline = moddedObject.GetObject<Transform>(5).gameObject;
                entry.m_HoverOutline.SetActive(false);
                entry.m_Index = index;
                entry.SetTitleText(workshopItem.ItemTitle);
                entry.LoadPreview();
                return entry;
            }

            public bool CanWorkWithImage()
            {
                return this != null && base.gameObject != null && base.gameObject.activeInHierarchy && !IsDisposedOrDestroyed() && m_ThumbnailImage != null && m_ThumbnailProgressBar != null;
            }

            public OverhaulWorkshopItem GetWorkshopItem() => m_WorkshopItem;
            public bool HasWorkshopItem() => m_WorkshopItem != null;

            private CanvasRenderer m_CanvasRenderer;
            private CanvasGroup m_CanvasGroup;
            private OverhaulWorkshopItem m_WorkshopItem;

            private Text m_TitleText;

            private RawImage m_ThumbnailImage;
            private Image m_ThumbnailProgressBar;

            private GameObject m_HoverOutline;

            public bool Show;
            private int m_Index;

            public void SetTitleText(string titleText)
            {
                m_TitleText.text = titleText;
            }

            public void LoadPreview()
            {
                if (!base.enabled || !base.gameObject.activeInHierarchy || string.IsNullOrEmpty(m_WorkshopItem.PreviewURL))
                {
                    return;
                }
                _ = StaticCoroutineRunner.StartStaticCoroutine(loadImageCoroutine());
            }

            private IEnumerator loadImageCoroutine()
            {
                if (!CanWorkWithImage())
                {
                    yield break;
                }

                m_ThumbnailProgressBar.fillAmount = 0f;
                m_ThumbnailProgressBar.gameObject.SetActive(true);
                m_ThumbnailImage.enabled = false;
                string texturePath = m_WorkshopItem.PreviewURL;

                if (CacheImages && File.Exists(OverhaulNetworkController.DownloadFolder + "Steam/" + m_WorkshopItem.ItemID.m_PublishedFileId + ".png"))
                {
                    texturePath = "file://" + OverhaulNetworkController.DownloadFolder + "Steam/" + m_WorkshopItem.ItemID.m_PublishedFileId + ".png";
                }

                OverhaulNetworkDownloadHandler handler = new OverhaulNetworkDownloadHandler();
                handler.DoneAction = delegate
                {
                    if (handler == null || !CanWorkWithImage())
                    {
                        return;
                    }

                    m_ThumbnailProgressBar.gameObject.SetActive(false);
                    m_ThumbnailImage.enabled = !handler.Error;
                    if (handler.Error)
                    {
                        return;
                    }

                    m_ThumbnailImage.texture = handler.DownloadedTexture;
                    if (CacheImages) _ = StaticCoroutineRunner.StartStaticCoroutine(saveImageCoroutine(handler.DownloadedData, m_WorkshopItem.ItemID.m_PublishedFileId.ToString()));
                };
                OverhaulNetworkController.DownloadTexture(texturePath, handler);
                yield return null;
                while (handler.DonePercentage != 1f && CanWorkWithImage())
                {
                    m_ThumbnailProgressBar.fillAmount = handler.DonePercentage;
                    yield return null;
                }

                yield break;
            }

            private IEnumerator saveImageCoroutine(byte[] image, string itemID)
            {
                string path = NetworkAssets.OverhaulNetworkController.DownloadFolder + "Steam/";
                if (!Directory.Exists(path) || File.Exists(path + itemID + ".png"))
                {
                    yield break;
                }

                using (FileStream s = File.OpenWrite(path + itemID + ".png"))
                {
                    yield return s.WriteAsync(image, 0, image.Length);
                }
                yield break;
            }

            public void ShowEntry()
            {
                Show = true;
            }

            private void Update()
            {
                m_CanvasGroup.alpha += (Show && !m_CanvasRenderer.cull ? 1f : 0f - m_CanvasGroup.alpha) * Time.unscaledDeltaTime * 5f;
                m_HoverOutline.SetActive(OverhaulWorkshopBrowserUI.Instance != null ? OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex == m_Index : false);
            }

            private void OnDisable()
            {
                StaticCoroutineRunner.StopStaticCoroutine(loadImageCoroutine());
                if (m_ThumbnailImage.texture != null)
                {
                    Destroy(m_ThumbnailImage.texture);
                }
            }

            private void OnDestroy()
            {
                StaticCoroutineRunner.StopStaticCoroutine(loadImageCoroutine());
                if (m_ThumbnailImage.texture != null)
                {
                    Destroy(m_ThumbnailImage.texture);
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                {
                    return;
                }
                OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex = m_Index;
                OverhaulWorkshopBrowserUI.Instance.ShowQuickInfo(m_WorkshopItem);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                {
                    return;
                }
                if(OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex == m_Index) OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex = -1;
                OverhaulWorkshopBrowserUI.Instance.ShowQuickInfo(null);
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                {
                    return;
                }
                OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex = -1;
                OverhaulWorkshopBrowserUI.Instance.ViewItem(m_WorkshopItem);
            }
        }
    }
}
