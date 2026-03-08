using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementOverhaulUIInfo : OverhaulUIBehaviour
    {
        [UIElement("Name")]
        private readonly Text _uiNameText;

        [UIElement("MissingPreviewText", false)]
        private readonly Text _missingPreviewText;

        [UIElement("TextBG")]
        private readonly RectTransform _textBG;

        private RawImage _image;

        private Texture2D _loadedImage;

        private UnityWebRequest _webRequest;

        public string PreviewFile;

        private bool _destroyed;

        protected override void OnInitialized()
        {
            _image = GetComponent<RawImage>();
            _image.color = Color.black;
            LoadPreview();
        }

        public override void OnDestroy()
        {
            _destroyed = true;

            Texture2D texture = _loadedImage;
            if (texture)
            {
                Destroy(texture);
            }

            UnityWebRequest webRequest = _webRequest;
            if (webRequest != null)
            {
                try
                {
                    webRequest.Abort();
                }
                catch { }
            }
        }

        public void LoadPreview()
        {
            _ = base.StartCoroutine(loadPreviewCoroutine());
        }

        private IEnumerator loadPreviewCoroutine()
        {
            yield return null;

            RectTransform rectTransform = _textBG;
            Vector2 sd = rectTransform.sizeDelta;
            sd.x = _uiNameText.preferredWidth + 10f;
            rectTransform.sizeDelta = sd;

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{PreviewFile}"))
            {
                _webRequest = unityWebRequest;
                yield return unityWebRequest.SendWebRequest();
                if (_destroyed)
                    yield break;

                if (unityWebRequest.isDone && !unityWebRequest.isHttpError && !unityWebRequest.isNetworkError)
                {
                    Texture2D texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture).texture;
                    _loadedImage = texture;
                    _image.texture = texture;
                    _image.color = Color.white;
                }
                else
                {
                    _missingPreviewText.gameObject.SetActive(true);
                    _missingPreviewText.text = $"Missing preview file:\n{PreviewFile}";
                }
            }
            yield break;
        }
    }
}
