using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPatchNotesImageEmbed : OverhaulUIBehaviour
    {
        [UIElement("Image", false)]
        private RawImage _image;

        [UIElement("Image")]
        private AspectRatioFitter _imageARF;

        [UIElementAction(nameof(OnClickedOnImage))]
        [UIElement("Image")]
        private Button _imageButton;

        [UIElement("NotAvailableLabel", false)]
        private GameObject _notAvailableLabelObject;

        private Texture2D _texture;

        private UnityWebRequest _webRequest;

        private LayoutElement _layoutElement;

        private bool _refreshSizeNextFrame;

        private bool _isDestroyed;

        public string URL;

        public Transform PatchNotesTransform;

        protected override void OnInitialized()
        {
            _layoutElement = GetComponent<LayoutElement>();
            _layoutElement.minHeight = 20f;

            RepositoryManager.Instance.GetTexture(URL, onDownloadedTheImage, onFailedToDownloadTheImage, out _webRequest, 20, true);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _isDestroyed = true;

            if (_webRequest != null)
            {
                _webRequest.Abort();
                _webRequest = null;
            }

            if (_texture)
            {
                Destroy(_texture);
                _texture = null;
            }
        }

        public override void Update()
        {
            base.Update();
            if (_refreshSizeNextFrame)
            {
                _refreshSizeNextFrame = false;
                _layoutElement.minHeight = _image.rectTransform.rect.height;
            }
        }

        private void onDownloadedTheImage(Texture2D texture)
        {
            _webRequest = null;

            if (_isDestroyed) return;

            _texture = texture;
            _image.gameObject.SetActive(true);
            _image.rectTransform.sizeDelta = new Vector2(Mathf.Min(400f, texture.width / 2f), 0f);
            _image.texture = texture;
            _imageARF.aspectRatio = texture.width / (float)texture.height;
            _refreshSizeNextFrame = true;
        }

        private void onFailedToDownloadTheImage(string error)
        {
            _webRequest = null;
            if (_notAvailableLabelObject)
                _notAvailableLabelObject.SetActive(true);
        }

        public void OnClickedOnImage()
        {
            ModUIUtils.ImageViewer(_texture, PatchNotesTransform);
        }
    }
}