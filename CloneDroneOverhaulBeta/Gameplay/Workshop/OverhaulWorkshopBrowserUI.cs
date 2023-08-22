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
    public class OverhaulWorkshopBrowserUI : OverhaulUIController
    {
        #region Settings

        [OverhaulSetting("Game interface.Network.New Workshop browser design", true)]
        public static bool UseThisUI;

        [OverhaulSetting("Game interface.Network.Cache images", false, false, null, "Game interface.Network.New Workshop browser design")]
        public static bool CacheImages;

        public static readonly List<Dropdown.OptionData> Tags = new List<Dropdown.OptionData>
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

        #endregion

        #region UI elements

        [ActionReference(nameof(Hide))]
        [ObjectReference("CloseWorkshopBrowser")]
        private readonly Button m_ExitButton;

        private PrefabAndContainer m_WorkshopItemsContainer;
        private PrefabAndContainer m_LevelTypesContainer;
        private PrefabAndContainer m_RanksContainer;
        private PrefabAndContainer m_DropdownRanksContainer;
        private PrefabAndContainer m_RankSeparatorsContainer;
        private LoadingIndicator m_LoadingIndicator;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("RanksDropdown")]
        private readonly GameObject m_RanksDropdown;

        [ObjectReference("QuickInfoNew")]
        private readonly GameObject m_QuickInfo;
        [ObjectReference("QuickInfoName")]
        private readonly Text m_QuickInfoLevelName;
        [ObjectReference("QuickInfoDesc")]
        private readonly Text m_QuickInfoLevelDesc;
        [ObjectReference("QuickInfoStars")]
        private readonly Text m_QuickInfoLevelStars;
        [ObjectReference("QuickInfoCreatorName")]
        private readonly Text m_QuickInfoCreatorName;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("ErrorWindow")]
        private readonly GameObject m_ErrorWindow;
        [ActionReference(nameof(RetryRequest))]
        [ObjectReference("RetryButton")]
        private readonly Button m_ErrorWindowRetryButton;

        [ObjectReference("ManagementPanel")]
        private readonly Transform m_ManagementButtonsContainer;
        [ObjectReference("ItemLoadProgressBar")]
        private readonly Transform m_ItemLoadingIndicatorTransform;
        [ActionReference(nameof(SubscribeToItem))]
        [ObjectReference("Subscribe")]
        private readonly Button m_SubscribeButton;
        [ActionReference(nameof(UnsubscribeFromItem))]
        [ObjectReference("UnSubscribe")]
        private readonly Button m_UnsubscribeButton;
        [ActionReference(nameof(OnPlayButtonClicked))]
        [ObjectReference("PlayButton")]
        private readonly Button m_PlayButton;
        [ActionReference(nameof(EraseLevelProgress))]
        [ObjectReference("EraseProgress")]
        private readonly Button m_EraseDataButton;
        [ActionReference(nameof(ReloadItemView))]
        [ObjectReference("ReloadPanelButton")]
        private readonly Button m_ReloadItemViewButton;

        [ActionReference(nameof(VoteUp))]
        [ObjectReference("UpVote")]
        private readonly Button m_UpVoteButton;
        [ActionReference(nameof(VoteDown))]
        [ObjectReference("DownVote")]
        private readonly Button m_DownVoteButton;
        [ActionReference(nameof(MarkItemAsFavourite))]
        [ObjectReference("Favourite")]
        private readonly Button m_FavoriteButton;
        [ActionReference(nameof(OpenItemSteamPage))]
        [ObjectReference("SteamPage")]
        private readonly Button m_ItemSteamPageButton;
        [ActionReference(nameof(OpenItemAuthorProfilePage))]
        [ObjectReference("CreatorProfile")]
        private readonly Button m_ItemCreatorProfileButton;
        [ActionReference(nameof(ViewAuthorItems))]
        [ObjectReference("CreatorItems")]
        private readonly Button m_ViewCreatorItemsButton;
        [ActionReference(nameof(CopyItemLink))]
        [ObjectReference("CopyLink")]
        private readonly Button m_CopyItemLinkButton;

        [ActionReference(nameof(TogglePageSelectionPanel))]
        [ObjectReference("ButtonPage")]
        private readonly Button m_PageSelectionButton;
        [ObjectReference("CurrentPageText")]
        private readonly Text m_CurrentPageText;
        [ObjectDefaultVisibility(false)]
        [ObjectReference("PageSelection")]
        private readonly Transform m_PageSelectionTransform;
        [ActionReference(nameof(RefreshLevelsList))]
        [ObjectReference("ButtonReloadPage")]
        private readonly Button m_ReloadPageButton;
        [ObjectReference("ButtonBrowseInstalledItems")]
        private readonly Button m_BrowseInstalledButton;
        [ObjectReference("ButtonBrowseWorkshopItems")]
        private readonly Button m_BrowseWorkshopButton;
        [ObjectReference("ButtonBrowserLocalUserItems")]
        private readonly Button m_BrowsePublishedButton;
        private PrefabAndContainer m_PageContainer;

        [ObjectReference("ButtonSearch")]
        private readonly Button m_SearchButton;
        [ObjectDefaultVisibility(false)]
        [ObjectReference("SearchPanel")]
        private readonly GameObject m_SearchPanel;
        [ObjectReference("SearchBox")]
        private readonly InputField m_SearchBox;
        [ObjectReference("ButtonStartSearching")]
        private readonly Button m_StartSearchingButton;

        private PrefabAndContainer m_AdditionalPreviewsContainer;
        private LoadingIndicator m_ItemDownloadLI;

        #endregion

        #region State

        private float m_UnscaledTimeClickedOnOption;
        private float m_TimeToAllowPressingReloadButton;
        private float m_TimeToAllowUsingArrowKeys;

        public int PageCount;
        public int ItemSelectionIndex;

        public bool? CurrentItemVote;

        public ProgressInformation CurrentRequestProgress;
        public OverhaulWorkshopRequestResult CurrentRequestResult;

        public static OverhaulWorkshopBrowserUI Instance;

        private static readonly List<Texture> m_LoadedTextures = new List<Texture>();
        private static ItemUIEntry[] m_SpawnedEntries;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("ItemPageFullscreen")]
        private readonly Transform m_ItemPageViewTransform;
        [ObjectReference("Page")]
        private readonly Transform m_PageTransform;

        private readonly ProgressInformation m_ProgressInfo = new ProgressInformation();

        public static string IconsDirectory => OverhaulMod.Core.ModDirectory + "Assets/Workshop/Ico/";
        public static bool BrowserIsNull => Instance == null;
        public static bool IsActive => !BrowserIsNull && Instance.gameObject.activeSelf;
        public bool IsViewingItemsByLocalUser => TargetAccount == SteamUser.GetSteamID();

        public bool IsViewingInstalledItems
        {
            get;
            private set;
        }
        public CSteamID TargetAccount
        {
            get;
            private set;
        } = default;

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
        public string SearchText
        {
            get;
            private set;
        }

        public OverhaulWorkshopItem ViewingWorkshopItem
        {
            get;
            private set;
        }

        public bool IsPopulatingItems
        {
            get;
            private set;
        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();
            Instance = this;

            m_BrowseInstalledButton.onClick.AddListener(delegate
            {
                if (IsPopulatingItems)
                    return;

                Page = 1;
                TargetAccount = default;
                IsViewingInstalledItems = true;
                m_BrowseInstalledButton.interactable = false;
                m_BrowseWorkshopButton.interactable = true;
                m_BrowsePublishedButton.interactable = true;
                m_SearchButton.interactable = false;
                RefreshLevelsList();
            });
            m_BrowseInstalledButton.interactable = true;
            m_BrowseWorkshopButton.onClick.AddListener(delegate
            {
                if (IsPopulatingItems)
                    return;

                Page = 1;
                TargetAccount = default;
                IsViewingInstalledItems = false;
                m_BrowseInstalledButton.interactable = true;
                m_BrowseWorkshopButton.interactable = false;
                m_BrowsePublishedButton.interactable = true;
                m_SearchButton.interactable = true;
                RefreshLevelsList();
            });
            m_BrowseWorkshopButton.interactable = false;
            m_BrowsePublishedButton.onClick.AddListener(delegate
            {
                if (IsPopulatingItems)
                    return;

                Page = 1;
                TargetAccount = SteamUser.GetSteamID();
                IsViewingInstalledItems = false;
                m_BrowseInstalledButton.interactable = true;
                m_BrowseWorkshopButton.interactable = true;
                m_BrowsePublishedButton.interactable = false;
                m_SearchButton.interactable = false;
                RefreshLevelsList();
            });
            m_BrowsePublishedButton.interactable = true;
            m_SearchButton.onClick.AddListener(delegate
            {
                m_SearchPanel.SetActive(!m_SearchPanel.activeSelf);
                m_SearchBox.text = string.Empty;
            });
            m_StartSearchingButton.onClick.AddListener(delegate
            {
                if (IsPopulatingItems)
                    return;

                Page = 1;
                m_SearchPanel.SetActive(false);
                SearchText = m_SearchBox.text;
                RefreshLevelsList();
            });

            m_WorkshopItemsContainer = new PrefabAndContainer(MyModdedObject, 1, 2);
            m_LevelTypesContainer = new PrefabAndContainer(MyModdedObject, 9, 10);
            m_RanksContainer = new PrefabAndContainer(MyModdedObject, 15, 16);
            m_DropdownRanksContainer = new PrefabAndContainer(MyModdedObject, 15, 16);
            m_RankSeparatorsContainer = new PrefabAndContainer(MyModdedObject, 17, 16);
            m_AdditionalPreviewsContainer = new PrefabAndContainer(MyModdedObject, 37, 38);
            m_PageContainer = new PrefabAndContainer(MyModdedObject, 47, 48);
            m_LoadingIndicator = new LoadingIndicator(MyModdedObject, 3, 4);
            m_ItemDownloadLI = new LoadingIndicator(MyModdedObject, 34, 35);
            m_CurrentPageText.text = "Page [1]";

            _ = m_PageTransform.gameObject.AddComponent<ItemViewPageBehaviour>();
            OverhaulUIPanelScaler scaler = m_PageTransform.gameObject.AddComponent<OverhaulUIPanelScaler>();
            scaler.Multiplier = 15f;
            scaler.StartScale = Vector3.one * 0.6f;
            scaler.StopForFrames = 3;

            AssignActionToButton(MyModdedObject, "Subscribe", SubscribeToItem);
            AssignActionToButton(MyModdedObject, "Thumbnail", delegate
            {
                OverhaulUIImageViewer.SetActive(true, MyModdedObject.GetObject<RawImage>(39).texture);
            });

            Page = 1;
            LevelTypeRequiredTag = "Adventure";
            RequiredRank = EUGCQuery.k_EUGCQuery_RankedByTrend;
        }

        public override void Show()
        {
            base.Show();
            OverhaulCanvasController.SetCanvasPixelPerfect(false);
            showDefaultInfo();
            RefreshLevelsList();
            PopulateLevelTypes();
            PopulateRanks();
            HideTitleScreenButtons();

            MyModdedObject.GetObject<Transform>(40).gameObject.SetActive(OverhaulVersion.IsDebugBuild);
        }

        public override void Hide()
        {
            HideBrowser(false);
        }

        public void HideBrowser(bool hiddenBecauseLoading = false)
        {
            base.Hide();
            OverhaulUIDescriptionTooltip.SetActive(false);
            OverhaulCanvasController.SetCanvasPixelPerfect(true);
            if (!hiddenBecauseLoading)
                ShowTitleScreenButtons();
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
                if (ViewingWorkshopItem != null)
                {
                    ViewItem(null);
                    return;
                }
                HideBrowser();
            }

            if (m_SpawnedEntries.IsNullOrEmpty())
                return;

            float time = Time.unscaledTime;
            if (time < m_TimeToAllowUsingArrowKeys) return;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                m_TimeToAllowUsingArrowKeys = time + 0.1f;
                if (ItemSelectionIndex < m_SpawnedEntries.Length - 1)
                {
                    ItemSelectionIndex++;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                m_TimeToAllowUsingArrowKeys = time + 0.1f;
                if (ItemSelectionIndex > 0)
                {
                    ItemSelectionIndex--;
                }
            }
            else if (ItemSelectionIndex != -1 && ItemSelectionIndex < m_SpawnedEntries.Length && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            {
                ItemUIEntry entry = m_SpawnedEntries[ItemSelectionIndex];
                if (entry != null && entry.HasWorkshopItem())
                {
                    ViewItem(entry.GetWorkshopItem());
                    ItemSelectionIndex = -1;
                }
            }
        }

        public void PopulateLevelTypes()
        {
            m_LevelTypesContainer.ClearContainer();
            foreach (DropdownStringOptionData data in Tags)
            {
                ModdedObject m = m_LevelTypesContainer.CreateNew();
                _ = LevelTypeUIEntry.CreateNew(m, data.StringValue);
                m.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(data.text);
            }
        }

        public void PopulateRanks()
        {
            m_RanksContainer.ClearContainer();
            populateRankArray(GenericRanks, "Rank by:");
            populateRankArray(FriendRanks, "Other:");
        }

        private void populateRankArray(EUGCQuery[] array, string header)
        {
            if (array.IsNullOrEmpty())
                return;

            ModdedObject separator = m_RankSeparatorsContainer.CreateNew();
            separator.GetObject<Text>(0).text = OverhaulLocalizationManager.GetTranslation(header);
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

                    OverhaulDownloadInfo iconDownloadHandler = new OverhaulDownloadInfo();
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
                    OverhaulNetworkAssetsController.DownloadTexture(IconsDirectory + "More.png", iconDownloadHandler);
                    return;
                }
                ModdedObject m = m_RanksContainer.CreateNew();
                m.GetObject<Text>(0).text = OverhaulLocalizationManager.GetTranslation(getRankString(array[i]));
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
                    return "Friends favorited";
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
                return;

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

        #region Request parameters

        public void SetTag(string requiredTag, bool refreshLevelsList = true)
        {
            if (IsPopulatingItems || requiredTag == LevelTypeRequiredTag)
            {
                return;
            }

            Page = 1;
            LevelTypeRequiredTag = requiredTag;
            if (refreshLevelsList)
            {
                RefreshLevelsList();
            }
        }

        public void SetRank(EUGCQuery rank, bool refreshLevelsList = true)
        {
            if (IsPopulatingItems || rank == RequiredRank)
                return;

            HideRanksDropdown();

            Page = 1;
            TargetAccount = default;
            RequiredRank = rank;
            if (refreshLevelsList)
            {
                RefreshLevelsList();
            }
        }

        #endregion

        #region Viewing the item

        public void ViewItem(OverhaulWorkshopItem workshopItem)
        {
            RawImage ri = MyModdedObject.GetObject<RawImage>(39);
            if (ri.texture)
                Destroy(ri.texture);

            foreach (Texture texture in m_LoadedTextures)
                if (texture)
                    Destroy(texture);

            ViewingWorkshopItem = workshopItem;
            if (workshopItem == null)
            {
                m_ItemPageViewTransform.gameObject.SetActive(false);
                return;
            }
            m_ItemPageViewTransform.gameObject.SetActive(true);

            ProgressInformation.SetProgress(m_ProgressInfo, 0f);
            LoadingIndicator.ResetIndicator(m_ItemDownloadLI);
            StaticCoroutineRunner.StopStaticCoroutine(waitUntilLevelIsDownloaded());
            m_ItemLoadingIndicatorTransform.gameObject.SetActive(false);

            RefreshManagementPanel();

            m_FavoriteButton.image.color = "#2E2E2E".ToColor();
            m_FavoriteButton.interactable = true;
            m_PageTransform.gameObject.SetActive(true);

            MyModdedObject.GetObject<Text>(23).text = workshopItem.ItemTitle;
            MyModdedObject.GetObject<Text>(24).text = workshopItem.CreatorNickname;
            MyModdedObject.GetObject<Text>(25).text = workshopItem.TimeCreated.ToShortDateString() + "\n" + workshopItem.TimeUpdated.ToShortDateString() + "\n" + workshopItem.ItemSizeString;
            MyModdedObject.GetObject<Text>(26).text = workshopItem.ItemLongDescription;

            OverhaulDownloadInfo hm = new OverhaulDownloadInfo();
            hm.DoneAction = delegate
            {
                if (hm != null && !hm.Error && ri != null)
                    ri.texture = hm.DownloadedTexture;
            };
            OverhaulNetworkAssetsController.DownloadTexture(workshopItem.PreviewURL, hm);

            int imagesCount = Mathf.Min(2, workshopItem.ItemAdditionalImages.Count);
            m_AdditionalPreviewsContainer.ClearContainer();
            if (imagesCount == 0)
                return;

            for (int i = 0; i < imagesCount; i++)
            {
                ModdedObject m = m_AdditionalPreviewsContainer.CreateNew();
                OverhaulDownloadInfo h = new OverhaulDownloadInfo();
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
                OverhaulNetworkAssetsController.DownloadTexture(workshopItem.ItemAdditionalImages[i], h);
            }
        }

        public void ReloadItemView()
        {
            ViewItem(ViewingWorkshopItem);
        }

        public void SubscribeToItem()
        {
            if (ViewingWorkshopItem == null)
                return;

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
                    yield break;

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
                return;

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
                return;

            m_FavoriteButton.image.color = "#2E2E2E".ToColor();
            m_FavoriteButton.interactable = false;
            OverhaulSteamBrowser.MarkItemAsFavourite(ViewingWorkshopItem.ItemID, delegate (PublishedFileId_t t, EResult r, bool a)
            {
                m_FavoriteButton.image.color = "#FFDA5A".ToColor();
            });
        }

        public void OpenItemSteamPage()
        {
            if (ViewingWorkshopItem == null)
                return;

            string url = ViewingWorkshopItem.ItemURL;
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage(url);
                return;
            }
            Application.OpenURL(url);
        }

        public void OpenItemAuthorProfilePage()
        {
            if (ViewingWorkshopItem == null)
                return;

            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToUser("steamid", ViewingWorkshopItem.CreatorID);
                return;
            }
            string url = "https://steamcommunity.com/profiles/" + ViewingWorkshopItem.CreatorID;
            Application.OpenURL(url);
        }

        public void ViewAuthorItems()
        {
            if (ViewingWorkshopItem == null)
                return;

            Page = 1;
            IsViewingInstalledItems = false;
            m_BrowseInstalledButton.interactable = true;
            m_BrowseWorkshopButton.interactable = false;
            m_BrowsePublishedButton.interactable = true;
            TargetAccount = ViewingWorkshopItem.CreatorID;
            RefreshLevelsList();
            ViewItem(null);
        }

        public void CopyItemLink()
        {
            if (ViewingWorkshopItem == null)
                return;

            ViewingWorkshopItem.ItemURL.CopyToClipboard();
        }

        public void EraseLevelProgress()
        {
            if (ViewingWorkshopItem == null)
                return;

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

            string id = ViewingWorkshopItem.ItemID.ToString();
            if (ChallengeManager.Instance._challengeDataDictionary.ContainsKey(id))
            {
                _ = ChallengeManager.Instance._challengeDataDictionary.Remove(id);
            }
        }

        public void VoteUp()
        {
            if (ViewingWorkshopItem == null || CurrentItemVote == true)
                return;

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
                return;

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
                return;

            SteamWorkshopItem item = ViewingWorkshopItem.ToSteamWorkshopItem();
            if (!File.Exists(item.Folder + "\\ExportedChallengeData.json"))
            {
                RefreshManagementPanel();
                return;
            }

            HideBrowser();
            _ = WorkshopChallengeManager.Instance.StartChallengeFromWorkshop(item);
        }

        public void RefreshManagementPanel()
        {
            bool shouldshow = ShouldShowManagementPanel();
            m_ManagementButtonsContainer.gameObject.SetActive(shouldshow);
            if (!shouldshow)
                return;

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
                return;

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
                return;

            CurrentItemVote = null;
            m_UpVoteButton.interactable = false;
            m_UpVoteButton.image.color = "#414141".ToColor();
            m_DownVoteButton.interactable = false;
            m_DownVoteButton.image.color = "#414141".ToColor();

            OverhaulSteamBrowser.GetItemVoteInfo(ViewingWorkshopItem.ItemID, delegate (bool skip, bool up, bool down, bool fail)
            {
                if (fail)
                    return;

                m_UpVoteButton.interactable = true;
                if (up)
                {
                    m_UpVoteButton.image.color = "#2DC869".ToColor();
                    CurrentItemVote = true;
                }
                m_DownVoteButton.interactable = true;
                if (down)
                {
                    m_DownVoteButton.image.color = "#EC5454".ToColor();
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

        private void showDefaultInfo()
        {
            OverhaulUIDescriptionTooltip.SetActive(true, OverhaulLocalizationManager.GetTranslation("Steam workshop browser"), OverhaulLocalizationManager.GetTranslation("Play and rate human levels!"));
        }

        public void ResetRequest()
        {
            IsPopulatingItems = false;
            ProgressInformation.SetProgress(CurrentRequestProgress, 1f);
            LoadingIndicator.ResetIndicator(m_LoadingIndicator);
            StaticCoroutineRunner.StopStaticCoroutine(populateItemsCoroutine());
            m_UnscaledTimeClickedOnOption = -1f;
            CurrentRequestResult = null;
            CurrentRequestProgress = null;
        }

        public void RefreshLevelsList()
        {
            if (IsPopulatingItems)
                return;

            IsPopulatingItems = true;
            m_CurrentPageText.text = string.Format(OverhaulLocalizationManager.GetTranslation("Page number"), Page);
            m_WorkshopItemsContainer.ClearContainer();
            m_PageSelectionTransform.gameObject.SetActive(false);
            m_PageSelectionButton.interactable = false;
            m_UnscaledTimeClickedOnOption = Time.unscaledTime;
            m_TimeToAllowPressingReloadButton = m_UnscaledTimeClickedOnOption + 1f;
            CurrentRequestResult = null;
            CurrentRequestProgress = new ProgressInformation();

            if (TargetAccount != default)
            {
                string persona = SteamFriends.GetFriendPersonaName(TargetAccount);
                OverhaulUIDescriptionTooltip.SetActive(true, string.Format(OverhaulLocalizationManager.GetTranslation("LevelsBy"), persona), OverhaulLocalizationManager.GetTranslation("View levels made by this user"));
            }
            else
            {
                showDefaultInfo();
            }

            SetErrorWindowActive(false);
            LoadingIndicator.ResetIndicator(m_LoadingIndicator);
            StaticCoroutineRunner.StopStaticCoroutine(populateItemsCoroutine());
            OverhaulSteamBrowser.RequestItems(RequiredRank, Steamworks.EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, OnGetWorkshopItems, CurrentRequestProgress, SearchText, LevelTypeRequiredTag, Page, false, true, IsViewingInstalledItems, TargetAccount.GetAccountID());
        }

        /// <summary>
        /// Called when our steam workshop request is done
        /// </summary>
        /// <param name="requestResult"></param>
        public void OnGetWorkshopItems(OverhaulWorkshopRequestResult requestResult)
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
                yield break;

            ItemUIEntry[] uiItems = new ItemUIEntry[CurrentRequestResult.ItemsReceived.Length];
            m_SpawnedEntries = uiItems;
            int itemIndex = 0;
            do
            {
                if (CurrentRequestResult.ItemsReceived[itemIndex] == null)
                {
                    itemIndex++;
                    continue;
                }

                ItemUIEntry entry = ItemUIEntry.CreateNew(m_WorkshopItemsContainer.CreateNew(), CurrentRequestResult.ItemsReceived[itemIndex], itemIndex);
                uiItems[itemIndex] = entry;
                itemIndex++;

                if (itemIndex % 10 == 0)
                    yield return null;

            } while (canPopulateItems(CurrentRequestResult) && itemIndex < uiItems.Length);

            if (uiItems.IsNullOrEmpty())
                yield break;

            itemIndex = 0;
            do
            {
                if (uiItems[itemIndex] != null)
                {
                    uiItems[itemIndex].ShowEntry();
                }
                itemIndex++;
                yield return new WaitForSecondsRealtime(0.024f);

            } while (canPopulateItems(CurrentRequestResult) && itemIndex < uiItems.Length);

            yield break;
        }

        private bool canPopulateItems(OverhaulWorkshopRequestResult requestResult)
        {
            return requestResult != null && !requestResult.ItemsReceived.IsNullOrEmpty();
        }

        public void ShowQuickInfo(OverhaulWorkshopItem workshopItem)
        {
            if (workshopItem == null)
                return;

            string stars = workshopItem.Stars.ToString();
            m_QuickInfoLevelStars.text = stars == "1" ? OverhaulLocalizationManager.GetTranslation("Not rated") : stars.Length > 3 ? stars.Remove(3) : stars;

            m_QuickInfo.SetActive(true);
            m_QuickInfoLevelName.text = workshopItem.ItemTitle;
            m_QuickInfoLevelDesc.text = workshopItem.ItemLongDescription;
            m_QuickInfoCreatorName.text = workshopItem.CreatorNickname;
        }

        public bool ShouldShowManagementPanel() => ViewingWorkshopItem != null;
        public bool ShouldMakePlayButtonInteractable() => LevelTypeRequiredTag == "Challenge" || LevelTypeRequiredTag == "Adventure";

        public bool ShouldMakeEraseDataButtonInteractable()
        {
            if (ViewingWorkshopItem == null)
                return false;

            string path = Application.persistentDataPath + "/ChallengeData" + ViewingWorkshopItem.ItemID + ".json";
            return ShouldMakePlayButtonInteractable() && File.Exists(path);
        }

        public bool ShouldResetRequest() => IsPopulatingItems && Time.unscaledTime >= m_UnscaledTimeClickedOnOption + 10f;

        #region Patches

        public bool TryShow()
        {
            if (!UseThisUI)
                return false;

            Show();
            return true;
        }

        #endregion

        public class ItemViewPageBehaviour : OverhaulBehaviour
        {
            private void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GameUIRoot.Instance == null || BrowserIsNull || OverhaulUIImageViewer.IsActive)
                        return;

                    Graphic g = base.GetComponent<Graphic>();
                    if (g == null)
                        return;

                    GraphicRaycaster c = GameUIRoot.Instance.GetComponent<GraphicRaycaster>();
                    if (c == null)
                        return;

                    List<Graphic> list = c.GetPrivateField<List<Graphic>>("m_RaycastResults");
                    if (list.IsNullOrEmpty() || list.Contains(g))
                        return;

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
                if (m_Button)
                    m_Button.onClick.AddListener(onClicked);
            }

            private void onClicked()
            {
                m_Button.OnDeselect(null);
                if (OverhaulWorkshopBrowserUI.BrowserIsNull || OverhaulWorkshopBrowserUI.Instance.IsViewingInstalledItems || OverhaulWorkshopBrowserUI.Instance.IsViewingItemsByLocalUser)
                    return;

                OverhaulWorkshopBrowserUI.Instance.Page = 1;
                OverhaulWorkshopBrowserUI.Instance.SetRank(m_Rank, true);
            }

            private void getIcon()
            {
                OverhaulDownloadInfo iconDownloadHandler = new OverhaulDownloadInfo();
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
                OverhaulNetworkAssetsController.DownloadTexture(IconsDirectory + m_Rank.ToString() + ".png", iconDownloadHandler);
            }

            private void Update()
            {
                if (!OverhaulWorkshopBrowserUI.BrowserIsNull && Time.frameCount % 3 == 0)
                    m_SelectedFrame.SetActive(!OverhaulWorkshopBrowserUI.Instance.IsViewingInstalledItems && !OverhaulWorkshopBrowserUI.Instance.IsViewingItemsByLocalUser && m_Rank == OverhaulWorkshopBrowserUI.Instance.RequiredRank);
            }

            private void OnDestroy()
            {
                if (m_RankIcon && m_RankIcon.texture != null && m_RankIcon.texture.name != "Placeholder-16x16")
                    Destroy(m_RankIcon.texture);
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
                    m_Button.onClick.AddListener(onClicked);
            }

            private void onClicked()
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull || string.IsNullOrEmpty(m_Tag))
                    return;

                OverhaulWorkshopBrowserUI.Instance.SetTag(m_Tag, true);
                m_Button.OnDeselect(null);
            }

            private void Update()
            {
                if (!OverhaulWorkshopBrowserUI.BrowserIsNull && Time.frameCount % 3 == 0)
                    m_Text.color = m_Tag == OverhaulWorkshopBrowserUI.Instance.LevelTypeRequiredTag ? SelectedColor : DeselectedColor;
            }
        }

        public class ItemUIEntry : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
        {
            public static ItemUIEntry CreateNew(ModdedObject moddedObject, OverhaulWorkshopItem workshopItem, int index)
            {
                if (workshopItem == null)
                {
                    if (moddedObject != null)
                        moddedObject.gameObject.SetActive(false);

                    return null;
                }

                workshopItem.RefreshCreatorInfo();

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
                entry.m_BeatenIndicator = moddedObject.GetObject<Transform>(6).gameObject;
                entry.m_BeatenIndicator.SetActive(ChallengeManager.Instance.HasCompletedChallenge(workshopItem.ItemID.ToString()));
                entry.m_Index = index;
                entry.SetTitleText(workshopItem.ItemTitle);
                entry.LoadPreview();
                return entry;
            }

            public void CanWorkWithImage(out bool can)
            {
                try
                {
                    can = !IsDisposedOrDestroyed() && base.gameObject.activeInHierarchy && m_ThumbnailImage && m_ThumbnailProgressBar;
                }
                catch
                {
                    can = false;
                }
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
            private GameObject m_BeatenIndicator;

            public bool Show;
            private int m_Index;

            public void SetTitleText(string titleText)
            {
                m_TitleText.text = titleText;
            }

            public void LoadPreview()
            {
                if (!base.enabled || !base.gameObject.activeInHierarchy || string.IsNullOrEmpty(m_WorkshopItem.PreviewURL))
                    return;

                _ = StaticCoroutineRunner.StartStaticCoroutine(loadImageCoroutine());
            }

            private IEnumerator loadImageCoroutine()
            {
                CanWorkWithImage(out bool can);
                if (!can)
                    yield break;

                m_ThumbnailProgressBar.gameObject.SetActive(false);
                m_ThumbnailImage.enabled = false;
                string texturePath = m_WorkshopItem.PreviewURL;

                string path = OverhaulMod.Core.ModDirectory + "Assets/Workshop/DownloadedPreviews/" + m_WorkshopItem.ItemID.m_PublishedFileId + ".png";
                if (CacheImages && File.Exists(path))
                    texturePath = "file://" + path;

                OverhaulDownloadInfo handler = new OverhaulDownloadInfo();
                handler.DoneAction = delegate
                {
                    CanWorkWithImage(out bool can2);
                    if (!can2)
                        return;

                    if (!this || IsDisposedOrDestroyed() || handler == null)
                        return;

                    m_ThumbnailImage.enabled = !handler.Error;
                    if (handler.Error)
                        return;

                    m_ThumbnailImage.texture = handler.DownloadedTexture;
                    if (CacheImages) _ = StaticCoroutineRunner.StartStaticCoroutine(saveImageCoroutine(handler.DownloadedData, m_WorkshopItem.ItemID.m_PublishedFileId.ToString()));
                };
                OverhaulNetworkAssetsController.DownloadTexture(texturePath, handler);
                yield return null;

                yield break;
            }

            private IEnumerator saveImageCoroutine(byte[] image, string itemID)
            {
                string path = OverhaulMod.Core.ModDirectory + "Assets/Workshop/DownloadedPreviews/";
                if (!Directory.Exists(path) || File.Exists(path + itemID + ".png"))
                    yield break;

                using (FileStream s = File.OpenWrite(path + itemID + ".png"))
                    yield return s.WriteAsync(image, 0, image.Length);
                yield break;
            }

            public void ShowEntry()
            {
                Show = true;
            }

            private void Update()
            {
                m_CanvasGroup.alpha += (Show && !m_CanvasRenderer.cull ? 1f : 0f - m_CanvasGroup.alpha) * Time.unscaledDeltaTime * 5f;
                m_HoverOutline.SetActive(OverhaulWorkshopBrowserUI.Instance != null && OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex == m_Index);
            }

            private void OnDisable()
            {
                StaticCoroutineRunner.StopStaticCoroutine(loadImageCoroutine());
                if (m_ThumbnailImage.texture != null)
                    Destroy(m_ThumbnailImage.texture);
            }

            private void OnDestroy()
            {
                StaticCoroutineRunner.StopStaticCoroutine(loadImageCoroutine());
                if (m_ThumbnailImage.texture != null)
                    Destroy(m_ThumbnailImage.texture);
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                    return;

                OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex = m_Index;
                OverhaulWorkshopBrowserUI.Instance.ShowQuickInfo(m_WorkshopItem);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                    return;

                if (OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex == m_Index) OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex = -1;
                OverhaulWorkshopBrowserUI.Instance.ShowQuickInfo(null);
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                    return;

                OverhaulWorkshopBrowserUI.Instance.ItemSelectionIndex = -1;
                OverhaulWorkshopBrowserUI.Instance.ViewItem(m_WorkshopItem);
            }
        }
    }
}
