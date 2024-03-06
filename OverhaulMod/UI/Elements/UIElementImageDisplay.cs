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
        private RawImage m_rawImage;

        [UIElement("LoadingIndicator", true)]
        private GameObject m_loadingIndicatorObject;

        private Texture2D m_loadedTexture;

        private UnityWebRequest m_webRequest;

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

        public void Populate(string path)
        {
            RepositoryManager.Instance.GetCustomTexture(path, delegate (Texture2D texture)
            {
                m_loadedTexture = texture;
                m_rawImage.texture = texture;
                m_rawImage.gameObject.SetActive(true);
                m_loadingIndicatorObject.SetActive(false);
                m_webRequest = null;
            }, delegate (string error)
            {
                m_rawImage.gameObject.SetActive(true);
                m_loadingIndicatorObject.SetActive(false);
                m_webRequest = null;
            }, out m_webRequest);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UnityWebRequest webRequest = m_webRequest;
            if(webRequest != null)
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
