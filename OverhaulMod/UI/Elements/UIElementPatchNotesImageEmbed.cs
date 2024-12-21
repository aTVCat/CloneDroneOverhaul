using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPatchNotesImageEmbed : OverhaulUIBehaviour
    {
        [UIElement("Image", false)]
        private RawImage m_image;

        [UIElement("Image")]
        private AspectRatioFitter m_imageARF;

        [UIElementAction(nameof(OnClickedOnImage))]
        [UIElement("Image")]
        private Button m_imageButton;

        [UIElement("NotAvailableLabel", false)]
        private GameObject m_notAvailableLabelObject;

        private Texture2D m_texture;

        private UnityWebRequest m_webRequest;

        private LayoutElement m_layoutElement;

        private bool m_refreshSizeNextFrame;

        public string URL;

        public Transform PatchNotesTransform;

        protected override void OnInitialized()
        {
            m_layoutElement = GetComponent<LayoutElement>();
            m_layoutElement.minHeight = 20f;

            RepositoryManager.Instance.GetTexture(URL, delegate (Texture2D texture)
            {
                m_webRequest = null;

                m_texture = texture;
                m_image.gameObject.SetActive(true);
                m_image.rectTransform.sizeDelta = new Vector2(Mathf.Min(400f, texture.width / 2f), 0f);
                m_image.texture = texture;
                m_imageARF.aspectRatio = texture.width / (float)texture.height;
                m_refreshSizeNextFrame = true;
            }, delegate
            {
                m_webRequest = null;
                if (m_notAvailableLabelObject)
                    m_notAvailableLabelObject.SetActive(true);
            }, out UnityWebRequest unityWebRequest);
            m_webRequest = unityWebRequest;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_webRequest != null)
            {
                m_webRequest.Abort();
                m_webRequest = null;
            }

            if (m_texture)
            {
                Destroy(m_texture);
                m_texture = null;
            }
        }

        public override void Update()
        {
            base.Update();
            if (m_refreshSizeNextFrame)
            {
                m_refreshSizeNextFrame = false;
                m_layoutElement.minHeight = m_image.rectTransform.rect.height;
            }
        }

        public void OnClickedOnImage()
        {
            ModUIUtils.ImageViewer(m_texture, PatchNotesTransform);
        }
    }
}
