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
        private readonly GameObject _videoIcon;

        private RawImage _image;

        private UnityWebRequest _webRequest;

        private Texture2D _texture;

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
            _image = base.GetComponent<RawImage>();
            _videoIcon.SetActive(isVideo);
            GetThumbnail();

            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(onClicked);
        }

        public override void OnDestroy()
        {
            Texture2D texture = _texture;
            if (texture)
                Destroy(texture);

            try
            {
                _webRequest.Abort();
            }
            catch { }
        }

        public void GetThumbnail()
        {
            _image.enabled = false;

            UIElementWorkshopItemPreviewDisplay previewDisplay = this;
            RepositoryManager.Instance.GetCustomTexture(isVideo ? $"https://img.youtube.com/vi/{link}/sddefault.jpg" : link, delegate (Texture2D texture)
            {
                if (!previewDisplay)
                {
                    if (texture)
                        Destroy(texture);

                    return;
                }

                _texture = texture;
                _image.enabled = true;
                _image.texture = texture;
                _image.rectTransform.sizeDelta = new Vector2(57.5f * (texture.width / (float)texture.height), 57.5f);
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

                        _texture = texture;
                        _image.enabled = true;
                        _image.texture = texture;
                        _image.rectTransform.sizeDelta = new Vector2(57.5f * (texture.width / (float)texture.height), 57.5f);
                    }, null, out _webRequest, 60);
                }
            }, out _webRequest, 60);
        }

        private void onClicked()
        {
            if (isVideo)
                Application.OpenURL($"https://youtu.be/{link}");
            else
            {
                if (!_texture)
                    return;

                imageViewerOpenedCallback?.Invoke();

                ModUIUtils.ImageViewer(_texture, imageViewerParentTransform, imageViewerClosedCallback);
            }
        }
    }
}
