using CDOverhaul.HUD;
using CDOverhaul.NetworkAssets;
using System.Collections;
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

        public OverhaulRequestProgressInfo CurrentRequestProgress;
        public OverhaulWorkshopRequestResult CurrentRequestResult;

        #region UI elements

        private Button m_ExitButton;

        private PrefabAndContainer m_WorkshopItemsContainer;
        private LoadingIndicator m_LoadingIndicator;

        private GameObject m_QuickInfo;
        private Text m_QuickInfoLevelName;
        private Text m_QuickInfoLevelDesc;
        private Text m_QuickInfoLevelStars;

        #endregion

        public override void Initialize()
        {
            BrowserUIInstance = this;

            m_WorkshopItemsContainer = new PrefabAndContainer(MyModdedObject, 1, 2);
            m_LoadingIndicator = new LoadingIndicator(MyModdedObject, 3, 4);
            m_ExitButton = MyModdedObject.GetObject<Button>(0);
            m_ExitButton.onClick.AddListener(Hide);
            m_QuickInfo = MyModdedObject.GetObject<Transform>(5).gameObject;
            m_QuickInfoLevelName = MyModdedObject.GetObject<Text>(6);
            m_QuickInfoLevelDesc = MyModdedObject.GetObject<Text>(7);
            m_QuickInfoLevelStars = MyModdedObject.GetObject<Text>(8);

            Hide(true);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            BrowserUIInstance = null;
        }

        public bool TryShow()
        {
            if (!UseThisUI)
            {
                return false;
            }

            Show();
            return true;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            OverhaulCanvasController.SetCanvasPixelPerfect(false);
            OverhaulUIDescriptionTooltip.SetActive(true, "Steam Workshop Browser", "Play and rate human levels!");
            DEBUGRequest();
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
        }

        public void DEBUGRequest()
        {
            if (CurrentRequestProgress != null)
            {
                return;
            }

            CurrentRequestResult = null;
            CurrentRequestProgress = new OverhaulRequestProgressInfo();
            LoadingIndicator.ResetIndicator(m_LoadingIndicator);
            OverhaulWorkshopBrowser.RequestItems(Steamworks.EUGCQuery.k_EUGCQuery_RankedByPublicationDate, Steamworks.EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, OnReceivedWorkshopResult, CurrentRequestProgress, "Adventure", 1);
            StaticCoroutineRunner.StartStaticCoroutine(populateItemsCoroutine());
        }

        public void OnReceivedWorkshopResult(OverhaulWorkshopRequestResult requestResult)
        {
            if (requestResult == null)
            {
                Debug.LogWarning("[OverhaulMod] RequestResult is null");
                return;
            }
            if (requestResult.Error)
            {
                Debug.LogWarning("[OverhaulMod] RequestResult has error");
                return;
            }
            CurrentRequestResult = requestResult;

            DelegateScheduler.Instance.Schedule(delegate
            {
                CurrentRequestProgress = null;
            }, 0.5f);
        }

        private IEnumerator populateItemsCoroutine()
        {
            m_WorkshopItemsContainer.ClearContainer();
            while (CurrentRequestResult == null)
            {
                yield return null;
            }
            yield return new WaitUntil(() => m_LoadingIndicator.CanvasGr.alpha <= 0.2f);

            if (!CurrentRequestResult.ItemsReceived.IsNullOrEmpty())
            {
                ItemUIEntry[] uiItems = new ItemUIEntry[CurrentRequestResult.ItemsReceived.Length];
                int i = 0;
                do
                {
                    if (i % 10 == 0)
                    {
                        yield return null;
                    }
                    ModdedObject m = m_WorkshopItemsContainer.CreateNew();
                    ItemUIEntry entry = ItemUIEntry.CreateNew(m, CurrentRequestResult.ItemsReceived[i]);
                    uiItems[i] = entry;
                    i++;
                } while (i < CurrentRequestResult.ItemsReceived.Length);

                i = 0;
                do
                {
                    if (uiItems[i] != null)
                    {
                        uiItems[i].ShowEntry();
                    }
                    yield return new WaitForSecondsRealtime(0.024f);
                    i++;
                } while (i < CurrentRequestResult.ItemsReceived.Length);
            }

            yield break;
        }

        public void ShowQuickInfo(OverhaulWorkshopItem workshopItem)
        {
            if (workshopItem == null)
            {
                m_QuickInfo.SetActive(false);
                return;
            }
            string stars = workshopItem.Stars.ToString();

            m_QuickInfo.SetActive(true);
            m_QuickInfoLevelName.text = workshopItem.Name;
            m_QuickInfoLevelDesc.text = workshopItem.Description;
            m_QuickInfoLevelStars.text = stars.Length > 3 ? stars.Remove(3) : stars;
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
                StaticCoroutineRunner.StartStaticCoroutine(loadImageCoroutine());
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
                    if (CacheImages) StaticCoroutineRunner.StartStaticCoroutine(saveImageCoroutine(handler.DownloadedData, m_WorkshopItem.ID.m_PublishedFileId.ToString()));
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
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (OverhaulWorkshopBrowserUI.BrowserIsNull)
                {
                    return;
                }
                OverhaulWorkshopBrowserUI.BrowserUIInstance.ShowQuickInfo(null);
            }
        }
    }
}
