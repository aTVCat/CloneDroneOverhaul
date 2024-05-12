using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementWorkshopItemPreviewDisplay : OverhaulUIBehaviour
    {
        [UIElement("VideoIcon", false)]
        private readonly GameObject m_videoIcon;

        private RawImage m_image;

        private UnityWebRequest m_webRequest;

        private Texture2D m_texture;

        public string link
        {
            get;
            set;
        }

        public bool isVideo
        {
            get;
            set;
        }

        public Transform imageViewerParentTransform
        {
            get;
            set;
        }

        public Action imageViewerOpenedCallback
        {
            get;
            set;
        }

        public Action imageViewerClosedCallback
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            m_image = base.GetComponent<RawImage>();
            m_videoIcon.SetActive(isVideo);
            GetThumbnail();

            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(onClicked);
        }

        public override void OnDestroy()
        {
            Texture2D texture = m_texture;
            if (texture)
                Destroy(texture);

            try
            {
                m_webRequest.Abort();
            }
            catch { }
        }

        public void GetThumbnail()
        {
            m_image.enabled = false;

            UIElementWorkshopItemPreviewDisplay previewDisplay = this;
            RepositoryManager.Instance.GetCustomTexture(isVideo ? $"https://img.youtube.com/vi/{link}/sddefault.jpg" : link, delegate (Texture2D texture)
            {
                if (!previewDisplay)
                {
                    if (texture)
                        Destroy(texture);

                    return;
                }

                m_texture = texture;
                m_image.enabled = true;
                m_image.texture = texture;
                m_image.rectTransform.sizeDelta = new Vector2(57.5f * (texture.width / (float)texture.height), 57.5f);
            }, delegate
            {
                if (previewDisplay && isVideo)
                {
                    RepositoryManager.Instance.GetCustomTexture(isVideo ? $"https://img.youtube.com/vi/{link}/0.jpg" : link, delegate (Texture2D texture)
                    {
                        if (!previewDisplay)
                        {
                            if (texture)
                                Destroy(texture);

                            return;
                        }

                        m_texture = texture;
                        m_image.enabled = true;
                        m_image.texture = texture;
                        m_image.rectTransform.sizeDelta = new Vector2(57.5f * (texture.width / (float)texture.height), 57.5f);
                    }, null, out m_webRequest, 60);
                }
            }, out m_webRequest, 60);
        }

        private void onClicked()
        {
            if (isVideo)
                Application.OpenURL($"https://youtu.be/{link}");
            else
            {
                if (!m_texture)
                    return;

                imageViewerOpenedCallback?.Invoke();

                ModUIUtils.ImageViewer(m_texture, imageViewerParentTransform, imageViewerClosedCallback);
            }
        }
    }
}
