using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementWorkshopItemDisplay : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
    {
        [UIElement("Text")]
        private readonly Text m_titleText;

        [UIElement("Preview", false)]
        private readonly RawImage m_thumbnail;

        [UIElement("SelectedFrame", false)]
        private readonly GameObject m_selectedFrame;

        [UIElement("LoadingIndicator", true)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("CompletedIndicator", false)]
        private readonly GameObject m_completedIndicator;

        private UnityWebRequest m_webRequest;

        private bool m_isMouseIn;

        public WorkshopItem workshopItem
        {
            get;
            set;
        }

        public Texture2D thumbnailTexture
        {
            get;
            set;
        }

        public Transform itemPageWindowParentTransform
        {
            get;
            set;
        }

        public UIWorkshopBrowser browserUI
        {
            get;
            set;
        }

        public bool isCollection
        {
            get;
            set;
        }

        /*
        protected override void OnInitialized()
        {
            base.OnInitialized();

            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(onClicked);
        }*/

        public override void OnDestroy()
        {
            WorkshopItem item = workshopItem;
            if (item != null && !item.IsDisposed())
            {
                item.Dispose();
            }
            workshopItem = null;

            Texture2D texture = thumbnailTexture;
            if (texture)
                Destroy(texture);

            try
            {
                m_webRequest.Abort();
            }
            catch { }
        }

        public void Populate(WorkshopItem workshopItem)
        {
            this.workshopItem = workshopItem;
            GetThumbnail();

            m_completedIndicator.SetActive(ChallengeManager.Instance.HasCompletedChallenge(workshopItem.ItemID.ToString()));
        }

        public void GetThumbnail()
        {
            WorkshopItem steamWorkshopItem = workshopItem;
            if (steamWorkshopItem == null || steamWorkshopItem.PreviewURL.IsNullOrEmpty())
                return;

            UIElementWorkshopItemDisplay workshopItemDisplay = this;
            RepositoryManager.Instance.GetCustomTexture(steamWorkshopItem.PreviewURL, delegate (Texture2D texture)
            {
                if (!workshopItemDisplay)
                {
                    if (texture)
                        Destroy(texture);

                    return;
                }

                thumbnailTexture = texture;
                m_loadingIndicator.SetActive(false);
                m_thumbnail.gameObject.SetActive(true);
                m_thumbnail.texture = texture;
            }, delegate
            {
                if (!workshopItemDisplay)
                    return;

                m_loadingIndicator.SetActive(false);
            }, out m_webRequest, 60);
        }

        public void RefreshSelectedFrame()
        {
            m_selectedFrame.SetActive(m_isMouseIn || browserUI.IsItemSelected(this));
        }

        private void onClicked()
        {
            var browser = browserUI;
            if (browser.HideContextMenuIfShown())
                return;

            WorkshopItem item = workshopItem;
            if (item == null || item.IsDisposed())
                return;

            if (isCollection)
            {
                browser.browseChildrenOfCollection = item.ItemID;
                browser.Populate();
                return;
            }

            UIWorkshopItemPageWindow window = ModUIConstants.ShowWorkshopItemPageWindow(itemPageWindowParentTransform);
            window.browserUI = browser;
            window.Populate(item);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isMouseIn = true;
            RefreshSelectedFrame();
            browserUI.QuickPreview(workshopItem);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isMouseIn = false;
            RefreshSelectedFrame();
            browserUI.QuickPreview(null);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_isMouseIn = false;
            RefreshSelectedFrame();
            browserUI.QuickPreview(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserContextMenu))
            {
                onClicked();
                return;
            }

            UIWorkshopBrowser bui = browserUI;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                bui.SetItemSelected(this, true);
                bui.ShowContextMenu(this);
                RefreshSelectedFrame();
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    bui.SetItemSelected(this, !bui.IsItemSelected(this));
                    RefreshSelectedFrame();
                }
                else
                {
                    onClicked();
                }
            }
        }
    }
}
