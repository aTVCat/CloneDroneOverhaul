using OverhaulMod.Content;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIWorkshopItemPageWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnRefreshButtonClicked))]
        [UIElement("RefreshPageButton")]
        private readonly Button _refreshButton;

        [UIElementAction(nameof(OnAuthorProfileButtonClicked))]
        [UIElement("AuthorProfileButton")]
        private readonly Button _authorProfileButton;

        [UIElementAction(nameof(OnAuthorLevelsButtonClicked))]
        [UIElement("AuthorLevelsButton")]
        private readonly Button _authorLevelsButton;

        [UIElementAction(nameof(OnShareButtonClicked))]
        [UIElement("ShareButton")]
        private readonly Button _shareButton;

        [UIElementAction(nameof(OnSteamPageButtonClicked))]
        [UIElement("SteamPageButton")]
        private readonly Button _steamPageButton;

        [UIElement("ItemTitle")]
        private readonly Text _itemTitleText;

        [UIElement("ItemDescription")]
        private readonly Text _itemDescriptionText;

        [UIElement("ItemAuthor")]
        private readonly Text _itemAuthorText;

        [UIElement("ItemMainPreview")]
        private readonly RawImage _itemPreviewImage;

        [UIElementAction(nameof(OnPreviewClicked))]
        [UIElement("ItemMainPreview")]
        private readonly Button _itemPreviewButton;

        [UIElementAction(nameof(OnVoteUpButtonClicked))]
        [UIElement("UpVoteButton")]
        private readonly Button _voteUpButton;

        [UIElement("UpVoteButtonText")]
        private readonly Text _upVoteButtonText;

        [UIElementAction(nameof(OnVoteDownButtonClicked))]
        [UIElement("DownVoteButton")]
        private readonly Button _voteDownButton;

        [UIElement("DownVoteButtonText")]
        private readonly Text _downVoteButtonText;

        [UIElementAction(nameof(OnFavoriteButtonClicked))]
        [UIElement("AddToFavouritesButton")]
        private readonly Button _favoriteButton;

        [UIElementAction(nameof(OnSubscribeButtonClicked))]
        [UIElement("SubscribeButton")]
        private readonly Button _subscribeButton;

        [UIElementAction(nameof(OnUnsubscribeButtonClicked))]
        [UIElement("UnsubscribeButton")]
        private readonly Button _unsubscribeButton;

        [UIElementAction(nameof(OnPlayButtonClicked))]
        [UIElement("PlayButton")]
        private readonly Button _playButton;

        [UIElementAction(nameof(OnPlayOptionsClicked))]
        [UIElement("AdvancedPlayOptionsButton")]
        private readonly Button _playOptionsButton;

        [ShowTooltipOnHighLight("erase progress", 1.5f, true)]
        [UIElementAction(nameof(OnEraseProgressButtonClicked))]
        [UIElement("DeleteProgressButton")]
        private readonly Button _eraseProgressButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton")]
        private readonly Button _updateButton;

        [UIElement("LBSMessage", false)]
        private readonly GameObject _battleRoyaleMessage;

        [UIElement("EndlessMessage", false)]
        private readonly GameObject _endlessModeMessage;

        [UIElement("FavouriteGlow", false)]
        private readonly GameObject _favoriteButtonGlowObject;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicatorObject;

        [UIElement("NotEnoughRatingsText", false)]
        private readonly GameObject _notEnoughRatingsTextObject;

        [UIElement("Stars", false)]
        private readonly GameObject _starsObject;

        [UIElement("CompletedIndicator", false)]
        private readonly GameObject _completedIndicator;

        [UIElement("LoadingIndicatorText")]
        private readonly Text _loadingIndicatorText;

        [UIElement("ItemAuthorAvatar")]
        private readonly RawImage _authorAvatarImage;

        [UIElement("StarsFill")]
        private readonly Image _ratingFillImage;

        [UIElement("UniqueVisitorsDetailText")]
        private readonly Text _visitorsText;

        [UIElement("SubscribersDetailText")]
        private readonly Text _subscribersText;

        [UIElement("FavouritesDetailText")]
        private readonly Text _favoritesText;

        [UIElement("FileSizeDetailText")]
        private readonly Text _fileSizeText;

        [UIElement("PostTimeDetailText")]
        private readonly Text _postTimeText;

        [UIElement("UpdateTimeDetailText")]
        private readonly Text _updateTimeText;

        [UIElement("TagsText")]
        private readonly Text _tagsText;

        [UIElement("ItemImageDisplay", false)]
        private readonly ModdedObject _additionalPreviewDisplayPrefab;

        [UIElement("ItemImageContainer")]
        private readonly Transform _additionalPreviewDisplayContainer;

        [UIElement("NamePanel")]
        private readonly RectTransform _namePanel;

        [UIElement("Name")]
        private readonly RectTransform _nameHolder;

        [UIElement("PlaceholderVoteButtons")]
        private readonly GameObject _placeholderVoteButtons;

        [UIElement("DetailsPanel")]
        private readonly LayoutElement _detailsPanel;

        [UIElement("AdditionalPreviewsScrollRect")]
        private readonly GameObject _additionalPreviewsScrollRectObject;

        /*
        [UIElement("Panel", typeof(UIElementMouseEventsComponent))]
        private readonly UIElementMouseEventsComponent _panel;*/

        private string _authorProfileLink, _itemLink;
        private CSteamID _authorId;

        private string _previewLink;
        private Texture2D _previewTexture, _authorAvatarTexture;

        private UnityWebRequest _webRequest;

        private UIElementShowTooltipOnHightLight _tooltipOnHightLight;

        private WorkshopItem _workshopItem;

        private bool _refreshDisplaysNextFrame;

        private float _makeButtonsInteractableInTime;

        private float _timeLeftToRefreshDisplays;

        private float _timeLeftToResumeTicker;
        private float _tickerProgress;
        private bool _tickerIsGoingLeft;

        public float EaseMultiplier;

        public UIWorkshopBrowser browserUI
        {
            get;
            set;
        }

        public bool isImageViewerShown
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            UIElementShowTooltipOnHightLight tooltipOnHightLight = _itemTitleText.gameObject.AddComponent<UIElementShowTooltipOnHightLight>();
            tooltipOnHightLight.tooltipShowDuration = 2f;
            tooltipOnHightLight.InitializeElement();
            _tooltipOnHightLight = tooltipOnHightLight;

            _upVoteButtonText.gameObject.AddComponent<BetterOutline>().effectColor = Color.black;
            _downVoteButtonText.gameObject.AddComponent<BetterOutline>().effectColor = Color.black;

            EaseMultiplier = 50f;
        }

        public override void Show()
        {
            base.Show();
            _tickerProgress = 0f;
            _tickerIsGoingLeft = false;
            _timeLeftToResumeTicker = 1f;

            RectTransform nameHolder = _nameHolder;
            Vector2 vector = nameHolder.anchoredPosition;
            vector.x = 0f;
            nameHolder.anchoredPosition = vector;
        }

        public override void Update()
        {
            base.Update();

            float d = Time.unscaledDeltaTime;
            if (_timeLeftToResumeTicker > 0f)
                _timeLeftToResumeTicker -= d;

            if (_refreshDisplaysNextFrame)
            {
                _refreshDisplaysNextFrame = false;

                refreshManagementDisplays(_workshopItem);
            }

            if (_makeButtonsInteractableInTime > 0f)
                _makeButtonsInteractableInTime -= d;

            if (_makeButtonsInteractableInTime <= 0f)
            {
                _makeButtonsInteractableInTime = -1f;
                _subscribeButton.interactable = true;
                _unsubscribeButton.interactable = true;
                _updateButton.interactable = true;
            }

            _timeLeftToRefreshDisplays -= d;
            if (_timeLeftToRefreshDisplays <= 0f)
            {
                _refreshDisplaysNextFrame = true;
                _timeLeftToRefreshDisplays = 0.1f;
            }

            RectTransform namePanel = _namePanel;
            RectTransform nameHolder = _nameHolder;
            float preferredWidth = LayoutUtility.GetPreferredWidth(_itemTitleText.rectTransform);
            float xa = 0f;
            float xb = Mathf.Min(namePanel.rect.width - preferredWidth + 35f, 0f);

            if (xb != 0f && _timeLeftToResumeTicker <= 0f)
            {
                float xbPositive = -xb;
                float toAdd = d * (1f / Mathf.Clamp(xbPositive, 100f, 600f)) * EaseMultiplier;

                if (_tickerIsGoingLeft)
                {
                    _tickerProgress -= toAdd;
                    if (_tickerProgress <= 0f)
                    {
                        _tickerIsGoingLeft = false;
                        _tickerProgress = 0f;
                    }
                }
                else
                {
                    _tickerProgress += toAdd;
                    if (_tickerProgress >= 1f)
                    {
                        _tickerIsGoingLeft = true;
                        _tickerProgress = 1f;
                    }
                }

                Vector2 vector = nameHolder.anchoredPosition;
                vector.x = Mathf.Lerp(xa, xb, NumberUtils.EaseInOutCubic(0f, 1f, _tickerProgress));
                nameHolder.anchoredPosition = vector;
            }
            else
            {
                Vector2 vector = nameHolder.anchoredPosition;
                vector.x = 0f;
                nameHolder.anchoredPosition = vector;
            }

            /*
            if (Input.GetMouseButtonDown(0) && !_panel.isMouseOverElement && !isImageViewerShown)
            {
                Hide();
            }*/
        }

        public override void OnDisable()
        {
            dispose();
        }

        public void Populate(WorkshopItem workshopItem)
        {
            dispose();
            if (workshopItem == null || workshopItem.IsDisposed())
                return;

            bool isChallengeOrAdventure = workshopItem.IsChallengeOrAdventure();
            string path = DataRepository.Instance.GetFullPath($"ChallengeData{workshopItem.ItemID}", false);
            _workshopItem = workshopItem;

            _playButton.interactable = true;

            _eraseProgressButton.gameObject.SetActive(isChallengeOrAdventure);
            _eraseProgressButton.interactable = isChallengeOrAdventure && File.Exists(path);

            _itemTitleText.text = workshopItem.Name;
            _itemDescriptionText.text = workshopItem.Description;

            _completedIndicator.SetActive(workshopItem.IsChallengeOrAdventure() && ChallengeManager.Instance.HasCompletedChallenge(workshopItem.ItemID.ToString()));

            if (!workshopItem.Author.IsNullOrEmpty() && workshopItem.Author != "[unknown]")
                _itemAuthorText.text = $"{LocalizationManager.Instance.GetTranslatedString("workshop_leveldetails_author")} {workshopItem.Author.AddColor(Color.white)}";
            else
                _itemAuthorText.text = $"{LocalizationManager.Instance.GetTranslatedString("workshop_leveldetails_author")} {workshopItem.AuthorID.ToString().AddColor(Color.white)}";

            _battleRoyaleMessage.SetActive(workshopItem.IsLastBotStandingLevel());
            _endlessModeMessage.SetActive(workshopItem.IsEndlessLevel());

            _itemLink = $"https://steamcommunity.com/sharedfiles/filedetails/?id={workshopItem.ItemID}";
            _authorProfileLink = $"https://steamcommunity.com/profiles/{workshopItem.AuthorID}";
            _authorId = workshopItem.AuthorID;

            _tooltipOnHightLight.tooltipText = workshopItem.Name;

            getMainPreview(workshopItem);
            getAuthorAvatar(workshopItem);
            populateDetails(workshopItem);
            populateAdditionalPreviews(workshopItem);
            refreshManagementDisplays(workshopItem);
            refreshUserVote(workshopItem);
            refreshUserVoteCounters(workshopItem);
        }

        public void SetFavoriteButtonInteractable(bool value)
        {
            _favoriteButton.interactable = value;
            _favoriteButtonGlowObject.SetActive(!value);
        }

        private void getMainPreview(WorkshopItem workshopItem)
        {
            _itemPreviewImage.gameObject.SetActive(false);

            string link = workshopItem.PreviewURL;
            _previewLink = link;

            UIWorkshopItemPageWindow itemPageWindow = this;
            RepositoryManager.Instance.GetCustomTexture(link, delegate (Texture2D texture)
            {
                if (!itemPageWindow || link != _previewLink)
                {
                    if (texture)
                        Destroy(texture);

                    return;
                }

                _previewTexture = texture;
                _itemPreviewImage.gameObject.SetActive(true);
                _itemPreviewImage.texture = texture;
                _itemPreviewImage.rectTransform.sizeDelta = new Vector2(Mathf.Min(145f * (texture.width / (float)texture.height), 257.7778f), 145f);
            }, null, out _webRequest, 60);
        }

        private void getAuthorAvatar(WorkshopItem workshopItem)
        {
            _authorAvatarImage.gameObject.SetActive(false);

            int handle = SteamFriends.GetMediumFriendAvatar(workshopItem.AuthorID);
            if (SteamUtils.GetImageSize(handle, out uint width, out uint height))
            {
                byte[] bytes = new byte[width * height * 4];
                if (SteamUtils.GetImageRGBA(handle, bytes, bytes.Length))
                {
                    try
                    {
                        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                        texture.LoadRawTextureData(bytes);
                        texture.Apply();
                        _authorAvatarTexture = texture;
                        _authorAvatarImage.texture = texture;
                        _authorAvatarImage.gameObject.SetActive(true);
                    }
                    catch
                    {
                        _authorAvatarImage.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void populateDetails(WorkshopItem workshopItem)
        {
            string tagsText = "-";
            string[] tagsArray = workshopItem.Tags;
            if (!tagsArray.IsNullOrEmpty())
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < tagsArray.Length; i++)
                {
                    _ = stringBuilder.Append(tagsArray[i]);
                    if (i < tagsArray.Length - 1)
                        _ = stringBuilder.Append(", ");
                }
                tagsText = stringBuilder.ToString();
            }

            string postTimeText = $"{workshopItem.PostDate.AddHours(-12).ToShortDateString()}, {workshopItem.PostDate.AddHours(-12).ToShortTimeString()}";
            string updateTimeText = $"{workshopItem.UpdateDate.AddHours(-12).ToShortDateString()}, {workshopItem.UpdateDate.AddHours(-12).ToShortTimeString()}";
            string sizeText = $"{Mathf.Round(workshopItem.Size * 100f) / 100f} MBs";

            int votes = workshopItem.Votes;
            float rating = Mathf.Ceil(workshopItem.Rating * 5f);

            _ratingFillImage.fillAmount = rating / 5f;
            _notEnoughRatingsTextObject.SetActive(votes < 25);
            _starsObject.SetActive(!_notEnoughRatingsTextObject.activeSelf);
            _visitorsText.text = workshopItem.Views.ToString();
            _subscribersText.text = workshopItem.Subscribers.ToString();
            _favoritesText.text = workshopItem.Favorites.ToString();
            _postTimeText.text = postTimeText;
            _updateTimeText.text = updateTimeText;
            _fileSizeText.text = sizeText;
            _tagsText.text = tagsText;
        }

        private void populateAdditionalPreviews(WorkshopItem workshopItem)
        {
            if (_additionalPreviewDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_additionalPreviewDisplayContainer);

            _additionalPreviewsScrollRectObject.SetActive(false);
            _detailsPanel.preferredHeight = 175f;

            if (workshopItem.AdditionalPreviews.IsNullOrEmpty())
                return;

            _additionalPreviewsScrollRectObject.SetActive(true);
            _detailsPanel.preferredHeight = 250f;
            foreach (WorkshopItemPreview preview in workshopItem.AdditionalPreviews.OrderBy(f => f.PreviewType != EItemPreviewType.k_EItemPreviewType_YouTubeVideo))
            {
                if (preview.URL.IsNullOrEmpty())
                    continue;

                ModdedObject moddedObject = Instantiate(_additionalPreviewDisplayPrefab, _additionalPreviewDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementWorkshopItemPreviewDisplay workshopItemPreviewDisplay = moddedObject.gameObject.AddComponent<UIElementWorkshopItemPreviewDisplay>();
                workshopItemPreviewDisplay.isVideo = preview.PreviewType != EItemPreviewType.k_EItemPreviewType_Image;
                workshopItemPreviewDisplay.link = preview.URL;
                workshopItemPreviewDisplay.imageViewerOpenedCallback = onImageViewerOpened;
                workshopItemPreviewDisplay.imageViewerClosedCallback = onImageViewerClosed;
                workshopItemPreviewDisplay.InitializeElement();
            }
        }

        private void refreshManagementDisplays(WorkshopItem workshopItem)
        {
            if (workshopItem == null || workshopItem.IsDisposed())
                return;

            EItemState itemState = ModSteamUGCUtils.GetItemState(workshopItem.ItemID);
            bool installed = ModSteamUGCUtils.IsItemInstalled(workshopItem.ItemID);
            bool subscribed = itemState.HasFlag(EItemState.k_EItemStateSubscribed);
            bool downloading = itemState.HasFlag(EItemState.k_EItemStateDownloading) || itemState.HasFlag(EItemState.k_EItemStateDownloadPending);
            bool needsUpdate = itemState.HasFlag(EItemState.k_EItemStateNeedsUpdate);
            bool allowPlayingFromThere = workshopItem.IsChallengeOrAdventure();

            _subscribeButton.gameObject.SetActive(!subscribed && !downloading);
            _unsubscribeButton.gameObject.SetActive(subscribed);
            _playButton.gameObject.SetActive(allowPlayingFromThere && installed && subscribed && !downloading);
            _playOptionsButton.gameObject.SetActive(ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserHistoryAndCheckpoints) && allowPlayingFromThere && installed && subscribed && !downloading);
            _updateButton.gameObject.SetActive(installed && subscribed && !downloading && needsUpdate);

            _loadingIndicatorObject.SetActive(downloading || needsUpdate);
            if (_loadingIndicatorObject.activeSelf)
                _loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("downloading...")}  {(Mathf.RoundToInt(Mathf.Clamp01(ModSteamUGCUtils.GetItemDownloadProgress(workshopItem.ItemID)) * 100f).ToString() + "%").AddColor(Color.white)}";
        }

        private void refreshUserVote(WorkshopItem workshopItem)
        {
            WorkshopItem item = workshopItem;
            if (item == null || item.IsDisposed())
                return;

            _placeholderVoteButtons.SetActive(true);
            _voteUpButton.gameObject.SetActive(false);
            _voteDownButton.gameObject.SetActive(false);
            SetFavoriteButtonInteractable(true);

            ModSteamUGCUtils.GetUserVote(item.ItemID, delegate (WorkshopItemVote workshopItemVote)
            {
                WorkshopItem item2 = _workshopItem;
                if (item != item2 || item2 == null || item2.IsDisposed())
                    return;

                _voteUpButton.gameObject.SetActive(true);
                _voteDownButton.gameObject.SetActive(true);
                _placeholderVoteButtons.SetActive(false);

                if (!workshopItemVote.HasVoted)
                {
                    _voteUpButton.interactable = true;
                    _voteDownButton.interactable = true;
                    return;
                }
                _voteUpButton.interactable = !workshopItemVote.VoteValue;
                _voteDownButton.interactable = workshopItemVote.VoteValue;
            });
        }

        private void refreshUserVoteCounters(WorkshopItem workshopItem)
        {
            _upVoteButtonText.text = workshopItem.UpVotes.ToString();
            _downVoteButtonText.text = workshopItem.DownVotes.ToString();
        }

        private void onImageViewerOpened()
        {
            isImageViewerShown = true;
        }

        private void onImageViewerClosed()
        {
            isImageViewerShown = false;
        }

        private void dispose()
        {
            _workshopItem = null;

            if (_webRequest != null)
            {
                try
                {
                    _webRequest.Abort();
                    _webRequest = null;
                }
                catch { }
            }

            Texture2D mp = _previewTexture;
            if (mp)
                Destroy(mp);

            Texture2D aat = _authorAvatarTexture;
            if (aat)
                Destroy(aat);
        }

        public void OnPreviewClicked()
        {
            Texture2D texture = _previewTexture;
            if (!texture)
                return;

            onImageViewerOpened();
            ModUIUtils.ImageViewer(texture, base.transform, onImageViewerClosed);
        }

        public void OnVoteUpButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            bool shouldDecreaseDownvotes = !_voteDownButton.interactable;
            bool shouldIncreaseTheCounter = _voteUpButton.interactable;

            _voteUpButton.interactable = false;
            ModSteamUGCUtils.SetUserVote(item.ItemID, true, delegate (SetUserItemVoteResult_t t, bool ioError)
            {
                WorkshopItem item2 = _workshopItem;
                if (item != item2 || item2 == null || item2.IsDisposed())
                    return;

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                    ModUIUtils.MessagePopupOK("Vote error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                else
                {
                    if (shouldDecreaseDownvotes)
                        item.DownVotes--;

                    if (shouldIncreaseTheCounter)
                        item.UpVotes++;

                    refreshUserVoteCounters(item);

                    _voteDownButton.interactable = t.m_bVoteUp;
                    _voteUpButton.interactable = !t.m_bVoteUp;
                    return;
                }

                _voteUpButton.interactable = true;
            });
        }

        public void OnVoteDownButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            bool shouldDecreaseUpvotes = !_voteUpButton.interactable;
            bool shouldIncreaseTheCounter = _voteDownButton.interactable;

            _voteDownButton.interactable = false;
            ModSteamUGCUtils.SetUserVote(item.ItemID, false, delegate (SetUserItemVoteResult_t t, bool ioError)
            {
                WorkshopItem item2 = _workshopItem;
                if (item != item2 || item2 == null || item2.IsDisposed())
                    return;

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                    ModUIUtils.MessagePopupOK("Vote error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                else
                {
                    if (shouldDecreaseUpvotes)
                        item.UpVotes--;

                    if (shouldIncreaseTheCounter)
                        item.DownVotes++;

                    refreshUserVoteCounters(item);

                    _voteDownButton.interactable = t.m_bVoteUp;
                    _voteUpButton.interactable = !t.m_bVoteUp;
                    return;
                }

                _voteDownButton.interactable = true;
            });
        }

        public void OnFavoriteButtonClicked()
        {
            ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("workshop_favorite_this_ite_header"), string.Empty, 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                WorkshopItem item = _workshopItem;
                if (item == null || item.IsDisposed())
                    return;

                ModSteamUGCUtils.AddItemToFavorites(item.ItemID, delegate (UserFavoriteItemsListChanged_t t, bool ioError)
                {
                    WorkshopItem item2 = _workshopItem;
                    if (item != item2 || item2 == null || item2.IsDisposed())
                        return;

                    if (ioError || t.m_eResult != EResult.k_EResultOK)
                    {
                        ModUIUtils.MessagePopupOK("Mark item as favorite error", $"Error code:{t.m_eResult} (ioError: {ioError})", 150f, true);
                    }
                    else
                    {
                        SetFavoriteButtonInteractable(false);
                        return;
                    }

                    SetFavoriteButtonInteractable(true);
                });

                SetFavoriteButtonInteractable(false);
            });
        }

        public void OnSubscribeButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            ModSteamUGCUtils.SubscribeItem(item.ItemID, delegate (RemoteStorageSubscribePublishedFileResult_t t, bool ioError)
            {
                WorkshopItem item2 = _workshopItem;
                if (item != item2 || item2 == null || item2.IsDisposed())
                    return;

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                    ModUIUtils.MessagePopupOK("Subscription error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                else
                    _refreshDisplaysNextFrame = true;
            });
            _refreshDisplaysNextFrame = true;

            _makeButtonsInteractableInTime = 2f;
            _subscribeButton.interactable = false;
        }

        public void OnUnsubscribeButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            ModSteamUGCUtils.UnsubscribeItem(item.ItemID, delegate (RemoteStorageUnsubscribePublishedFileResult_t t, bool ioError)
            {
                WorkshopItem item2 = _workshopItem;
                if (item != item2 || item2 == null || item2.IsDisposed())
                    return;

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                    ModUIUtils.MessagePopupOK("Unsubscription error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                else
                    _refreshDisplaysNextFrame = true;
            });
            _refreshDisplaysNextFrame = true;

            _makeButtonsInteractableInTime = 2f;
            _unsubscribeButton.interactable = false;
        }

        public void OnPlayButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            if (SteamUGC.GetItemInstallInfo(item.ItemID, out _, out string folder, ModSteamUGCUtils.cchFolderSize, out _))
                item.Folder = folder;

            if (!WorkshopChallengeManager.Instance.StartChallengeFromWorkshop(item.ToSteamWorkshopItem()))
            {
                ModUIUtils.MessagePopupOK("Incompatible game version!", "This item was made on newer version of the game.\nTo become able to play this level, update the game.", true);
                _playButton.interactable = false;
            }
            else
            {
                Hide();
                browserUI.Hide();
            }
        }

        public void OnPlayOptionsClicked()
        {
            ModUIConstants.ShowWorkshopItemPagePlayOptions(base.transform);
        }

        public void OnEraseProgressButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            string path = DataRepository.Instance.GetFullPath($"ChallengeData{item.ItemID}", false);
            ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("reset_progress_header"), LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                if (File.Exists(path))
                    try
                    {
                        File.Delete(path);
                        _eraseProgressButton.interactable = false;
                    }
                    catch (Exception exception)
                    {
                        ModUIUtils.MessagePopupOK("Error", $"Could not erase save file.\nDetails: {exception}");
                    }
            });
        }

        public void OnOptionsButtonClicked()
        {

        }

        public void OnSetLevelOnTitleScreenButtonClicked()
        {

        }

        public void OnUpdateButtonClicked()
        {
            WorkshopItem item = _workshopItem;
            if (item == null || item.IsDisposed())
                return;

            _ = ModSteamUGCUtils.UpdateItem(item.ItemID, delegate (DownloadItemResult_t t)
            {
                WorkshopItem item2 = _workshopItem;
                if (item != item2 || item2 == null || item2.IsDisposed())
                    return;

                if (t.m_unAppID == SteamUtils.GetAppID() && t.m_nPublishedFileId == item2.ItemID)
                {
                    if (t.m_eResult != EResult.k_EResultOK)
                        ModUIUtils.MessagePopupOK("Update error", $"Error code: {t.m_eResult}", 150f, true);
                    else
                        _refreshDisplaysNextFrame = true;
                }
            });
            _refreshDisplaysNextFrame = true;

            _makeButtonsInteractableInTime = 2f;
            _updateButton.interactable = false;
        }

        public void OnSteamPageButtonClicked()
        {
            string link = _itemLink;
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage(link);
            else
                Application.OpenURL(link);
        }

        public void OnShareButtonClicked()
        {
            GUIUtility.systemCopyBuffer = _itemLink;
            ModUIUtils.MessagePopupOK(LocalizationManager.Instance.GetTranslatedString("workshop_link_copied"), string.Empty, false);
        }

        public void OnAuthorProfileButtonClicked()
        {
            string link = _authorProfileLink;
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage(link);
            else
                Application.OpenURL(link);
        }

        public void OnAuthorLevelsButtonClicked()
        {
            UIWorkshopBrowser bui = browserUI;
            bui.searchLevelsByUser = _authorId;
            bui.searchUserList = EUserUGCList.k_EUserUGCList_Published;
            bui.sourceType = 1;
            bui.browseCollections = false;
            bui.browseChildrenOfCollection = default;
            bui.Populate();
            Hide();
        }

        public void OnRefreshButtonClicked()
        {
            _refreshButton.interactable = false;
            Populate(_workshopItem);
            DelegateScheduler.Instance.Schedule(delegate
            {
                if (_refreshButton)
                    _refreshButton.interactable = true;
            }, 1f);
        }
    }
}
