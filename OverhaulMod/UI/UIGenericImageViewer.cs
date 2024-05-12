using System;
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

        private Action m_closedCallback;

        public override bool enableCursor => true;

        public override void Hide()
        {
            base.Hide();
            Action action = m_closedCallback;
            if (action != null)
            {
                action();
                m_closedCallback = null;
            }
        }

        public void Populate(Texture2D texture, Action closedCallback)
        {
            m_closedCallback = closedCallback;
            m_imageRectTransform.sizeDelta = new Vector2(390f * (texture.width / (float)texture.height), 390f);
            m_image.texture = texture;
        }
    }
}
