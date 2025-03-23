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
        private readonly RawImage m_rawImage;

        [UIElement("LoadingIndicator", true)]
        private readonly GameObject m_loadingIndicatorObject;

        private Texture2D m_loadedTexture;

        private UnityWebRequest m_webRequest;

        private bool m_isDestroyed;

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
                RepositoryManager.Instance.GetCustomTexture(link, onGetTexture, onFailedToGetTexture, out m_webRequest);
                return;
            }
            RepositoryManager.Instance.GetTexture(link, onGetTexture, onFailedToGetTexture, out m_webRequest);
        }

        private void onGetTexture(Texture2D texture)
        {
            if (m_isDestroyed)
                return;

            m_loadedTexture = texture;
            m_rawImage.texture = texture;
            m_rawImage.gameObject.SetActive(true);
            m_loadingIndicatorObject.SetActive(false);
            m_webRequest = null;
        }

        private void onFailedToGetTexture(string error)
        {
            if (m_isDestroyed)
                return;

            m_rawImage.gameObject.SetActive(true);
            m_loadingIndicatorObject.SetActive(false);
            m_webRequest = null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_isDestroyed = true;

            UnityWebRequest webRequest = m_webRequest;
            if (webRequest != null)
            {
                try
                {
                    webRequest.Abort();
                }
                catch { }
            }

            Texture2D texture = m_loadedTexture;
            if (texture)
                Destroy(texture);
        }

        private void onClicked()
        {
            Texture2D texture = m_loadedTexture;
            if (!texture)
                return;

            ModUIUtils.ImageViewer(texture, imageViewerParentTransform);
        }
    }
}
