using CDOverhaul.HUD;
using CDOverhaul.NetworkAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.Workshop
{
    public class OverhaulWorkshopBrowserUI : OverhaulUI
    {
        [OverhaulSetting("Game interface.Network.New Workshop browser design", true)]
        public static bool UseThisUI;

        [OverhaulSetting("Game interface.Network.Cache images", false, false, null, null, null, "Game interface.Network.New Workshop browser design")]
        public static bool CacheImages;

        public static bool BrowserIsNull => BrowserUIInstance == null;
        public static OverhaulWorkshopBrowserUI BrowserUIInstance;

        public string LevelTypeRequiredTag
        {
            get;
            private set;
        }

        private float m_UnscaledTimeClickedOnOption;
        public bool ShouldResetRequest()
        {
            return IsPopulatingItems && Time.unscaledTime >= m_UnscaledTimeClickedOnOption + 4f;
        }

        public OverhaulRequestProgressInfo CurrentRequestProgress;
        public OverhaulWorkshopRequestResult CurrentRequestResult;
        public bool IsPopulatingItems
        {
            get;
            private set;
        }

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

        #region UI elements

        private Button m_ExitButton;

        private PrefabAndContainer m_WorkshopItemsContainer;
        private PrefabAndContainer m_LevelTypesContainer;
        private LoadingIndicator m_LoadingIndicator;

        private GameObject m_QuickInfo;
        private Text m_QuickInfoLevelName;
        private Text m_QuickInfoLevelDesc;
        private Text m_QuickInfoLevelStars;
        private Text m_QuickInfoUserName;
        private RawImage m_QuickInfoUserAvatar;

        private GameObject m_ErrorWindow;
        private Button m_ErrorWindowRetryButton;

        #endregion

        #region Patches

        public bool TryShow()
        {
            if (OverhaulVersion.Upd2Hotfix || !UseThisUI)
            {
                return false;
            }

            Show();
            return true;
        }

        #endregion

        #region Initilaizing & disposing

        public override void Initialize()
        {
            BrowserUIInstance = this;

            m_WorkshopItemsContainer = new PrefabAndContainer(MyModdedObject, 1, 2);
            m_LevelTypesContainer = new PrefabAndContainer(MyModdedObject, 9, 10);
            m_LoadingIndicator = new LoadingIndicator(MyModdedObject, 3, 4);
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

            LevelTypeRequiredTag = "Adventure";

            Hide(true);
        }

        public void PopulateLevelTypes()
        {
            m_LevelTypesContainer.ClearContainer();
            foreach (DropdownStringOptionData data in LevelTypes)
            {
                ModdedObject m = m_LevelTypesContainer.CreateNew();
                LevelTypeUIEntry.CreateNew(m, data.StringValue);
                m.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(data.text);
            }
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            BrowserUIInstance = null;
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
            LoadingIndicator.UpdateIndicator(m_LoadingIndicator, CurrentRequestProgress);
            if (ShouldResetRequest())
            {
                SetErrorWindowActive(true);
                ResetRequest();
            }
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
            m_WorkshopItemsContainer.ClearContainer();
            m_UnscaledTimeClickedOnOption = Time.unscaledTime;
            CurrentRequestResult = null;
            CurrentRequestProgress = new OverhaulRequestProgressInfo();

            LoadingIndicator.ResetIndicator(m_LoadingIndicator);
            StaticCoroutineRunner.StopStaticCoroutine(populateItemsCoroutine());
            OverhaulSteamBrowser.RequestItems(Steamworks.EUGCQuery.k_EUGCQuery_RankedByPublicationDate, Steamworks.EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, OnReceivedWorkshopResult, CurrentRequestProgress, LevelTypeRequiredTag, 1);
        }

        /// <summary>
        /// Called when our steam workshop request is done
        /// </summary>
        /// <param name="requestResult"></param>
        public void OnReceivedWorkshopResult(OverhaulWorkshopRequestResult requestResult)
        {
            IsPopulatingItems = false;
            CurrentRequestResult = requestResult;

            // todo: add timeout
            if (requestResult == null || requestResult.Error)
            {
                Debug.LogWarning("[OverhaulMod] RequestResult error");
                return;
            }

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
            int itemIndex = 0;
            do
            {
                ItemUIEntry entry = ItemUIEntry.CreateNew(m_WorkshopItemsContainer.CreateNew(), CurrentRequestResult.ItemsReceived[itemIndex]);
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
            m_QuickInfoLevelName.text = workshopItem.Name;
            m_QuickInfoLevelDesc.text = workshopItem.Description;
            m_QuickInfoLevelStars.text = stars.Length > 3 ? stars.Remove(3) : stars;
            m_QuickInfoUserName.text = workshopItem.Author;
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
                OverhaulWorkshopBrowserUI.BrowserUIInstance.SetTag(m_Tag, true);
                m_Button.OnDeselect(null);
            }

            private void Update()
            {
                if (!OverhaulWorkshopBrowserUI.BrowserIsNull && Time.frameCount % 3 == 0)
                {
                    m_Text.color = m_Tag == OverhaulWorkshopBrowserUI.BrowserUIInstance.LevelTypeRequiredTag ? SelectedColor : DeselectedColor;
                }
            }
        }

        public class ItemUIEntry : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            // todo: add unloading

            public static ItemUIEntry CreateNew(ModdedObject moddedObject, OverhaulWorkshopItem workshopItem)
            {
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
                entry.SetTitleText(workshopItem.Name);
                entry.LoadPreview();
                return entry;
            }

            public bool CanWorkWithImage()
            {
                return this != null && base.gameObject != null && base.gameObject.activeInHierarchy && !IsDisposedOrDestroyed() && m_ThumbnailImage != null && m_ThumbnailProgressBar != null;
            }

            private CanvasRenderer m_CanvasRenderer;
            private CanvasGroup m_CanvasGroup;
            private OverhaulWorkshopItem m_WorkshopItem;

            private Text m_TitleText;

            private RawImage m_ThumbnailImage;
            private Image m_ThumbnailProgressBar;

            private GameObject m_HoverOutline;

            public bool Show;

            public void SetTitleText(string titleText)
            {
                m_TitleText.text = titleText;
            }

            public void LoadPreview()
            {
                if (!base.enabled || !base.gameObject.activeInHierarchy || string.IsNullOrEmpty(m_WorkshopItem.ThumbnailURL))
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
                string texturePath = m_WorkshopItem.ThumbnailURL;

                if (CacheImages && File.Exists(OverhaulNetworkController.DownloadFolder + "Steam/" + m_WorkshopItem.ID.m_PublishedFileId + ".png"))
                {
                    texturePath = "file://" + OverhaulNetworkController.DownloadFolder + "Steam/" + m_WorkshopItem.ID.m_PublishedFileId + ".png";
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
                    if (CacheImages) _ = StaticCoroutineRunner.StartStaticCoroutine(saveImageCoroutine(handler.DownloadedData, m_WorkshopItem.ID.m_PublishedFileId.ToString()));
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
                OverhaulWorkshopBrowserUI.BrowserUIInstance.ShowQuickInfo(m_WorkshopItem);
                m_HoverOutline.SetActive(true);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                {
                    return;
                }
                OverhaulWorkshopBrowserUI.BrowserUIInstance.ShowQuickInfo(null);
                m_HoverOutline.SetActive(false);
            }
        }
    }
}
