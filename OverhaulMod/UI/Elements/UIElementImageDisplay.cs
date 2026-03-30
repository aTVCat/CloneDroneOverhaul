using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementImageDisplay : OverhaulUIBehaviour
    {
        [UIElement("Image", false)]
        private readonly RawImage _rawImage;

        [UIElement("LoadingIndicator", true)]
        private readonly GameObject _loadingIndicatorObject;

        private Texture2D _loadedTexture;

        private UnityWebRequest _webRequest;

        private bool _isDestroyed;

        public Transform imageViewerParentTransform
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(onClicked);
        }

        public void Populate(string link, bool isCustomLink = false)
        {
            if (isCustomLink)
            {
                RepositoryManager.Instance.GetCustomTexture(link, onGetTexture, onFailedToGetTexture, out _webRequest, 20, true);
                return;
            }
            RepositoryManager.Instance.GetTexture(link, onGetTexture, onFailedToGetTexture, out _webRequest, 20, true);
        }

        private void onGetTexture(Texture2D texture)
        {
            if (_isDestroyed)
                return;

            _loadedTexture = texture;
            _rawImage.texture = texture;
            _rawImage.gameObject.SetActive(true);
            _loadingIndicatorObject.SetActive(false);
            _webRequest = null;
        }

        private void onFailedToGetTexture(string error)
        {
            if (_isDestroyed)
                return;

            _rawImage.gameObject.SetActive(true);
            _loadingIndicatorObject.SetActive(false);
            _webRequest = null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _isDestroyed = true;

            UnityWebRequest webRequest = _webRequest;
            if (webRequest != null)
            {
                try
                {
                    webRequest.Abort();
                }
                catch { }
            }

            Texture2D texture = _loadedTexture;
            if (texture)
                Destroy(texture);
        }

        private void onClicked()
        {
            Texture2D texture = _loadedTexture;
            if (!texture)
                return;

            ModUIUtils.ImageViewer(texture, imageViewerParentTransform);
        }
    }
}
