using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIGenericImageViewer : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("Texture")]
        private readonly RawImage _image;

        [UIElement("Texture")]
        private readonly RectTransform _imageRectTransform;

        private Action _closedCallback;

        public override bool EnableCursor => true;

        public override void Hide()
        {
            base.Hide();
            Action action = _closedCallback;
            if (action != null)
            {
                action();
                _closedCallback = null;
            }
        }

        public void Populate(Texture2D texture, Action closedCallback)
        {
            _closedCallback = closedCallback;
            _imageRectTransform.sizeDelta = new Vector2(390f * (texture.width / (float)texture.height), 390f);
            _image.texture = texture;
        }
    }
}
