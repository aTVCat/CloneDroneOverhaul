using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIGenericImageViewer : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Texture")]
        private readonly RawImage m_image;

        [UIElement("Texture")]
        private readonly RectTransform m_imageRectTransform;

        public override bool enableCursor => true;

        public void Populate(Texture2D texture)
        {
            m_imageRectTransform.sizeDelta = new Vector2(390f * (texture.width / (float)texture.height), 390f);
            m_image.texture = texture;
        }
    }
}
