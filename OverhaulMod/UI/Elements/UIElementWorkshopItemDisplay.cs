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
        private readonly Text _titleText;

        [UIElement("AuthorText")]
        private readonly Text _authorText;

        [UIElement("DescriptionText")]
        private readonly Text _description;

        [UIElement("Preview", false)]
        private readonly RawImage _thumbnail;

        [UIElement("SelectedFrame", false)]
        private readonly GameObject _selectedFrame;

        [UIElement("LoadingIndicator", true)]
        private readonly GameObject _loadingIndicator;

        [UIElement("CompletedIndicator", false)]
        private readonly GameObject _completedIndicator;

        private UnityWebRequest _webRequest;

        private bool _isMouseIn;

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
                _webRequest.Abort();
            }
            catch { }
        }

        public void Populate(WorkshopItem workshopItem)
        {
            this.workshopItem = workshopItem;
            GetThumbnail();

            if (isCollection)
            {
                if (!workshopItem.Author.IsNullOrEmpty() && workshopItem.Author != "[unknown]")
                    _authorText.text = $"By {workshopItem.Author.AddColor(Color.white)}";
                else
                    _authorText.text = $"By {workshopItem.AuthorID.ToString().AddColor(Color.white)}";


                _description.text = workshopItem.Description;
            }
            else
            {
                _completedIndicator.SetActive(ChallengeManager.Instance.HasCompletedChallenge(workshopItem.ItemID.ToString()));
            }
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
                _loadingIndicator.SetActive(false);
                _thumbnail.gameObject.SetActive(true);
                _thumbnail.texture = texture;
            }, delegate
            {
                if (!workshopItemDisplay)
                    return;

                _loadingIndicator.SetActive(false);
            }, out _webRequest, 60);
        }

        public void RefreshSelectedFrame()
        {
            _selectedFrame.SetActive(_isMouseIn || browserUI.IsItemSelected(this));
        }

        private void onClicked()
        {
            UIWorkshopBrowser browser = browserUI;
            if (browser.HideContextMenuIfShown())
                return;

            WorkshopItem item = workshopItem;
            if (item == null || item.IsDisposed())
                return;

            if (isCollection)
            {
                browser.ViewingCollection = item.ItemID;
                browser.Populate();
                return;
            }

            UIWorkshopItemPageWindow window = ModUIs.ShowWorkshopItemPageWindow(itemPageWindowParentTransform);
            window.browserUI = browser;
            window.Populate(item);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseIn = true;
            RefreshSelectedFrame();
            if (isCollection)
                return;

            browserUI.QuickPreview(workshopItem);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseIn = false;
            RefreshSelectedFrame();
            if (isCollection)
                return;

            browserUI.QuickPreview(null);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMouseIn = false;
            RefreshSelectedFrame();
            if (isCollection)
                return;

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
