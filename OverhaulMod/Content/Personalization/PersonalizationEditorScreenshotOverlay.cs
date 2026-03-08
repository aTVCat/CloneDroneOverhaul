using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotOverlay : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private RawImage _resultImage;

        private GameObject _resultImageFrame;

        private float _timeLeftToHideResult;

        private void Start()
        {
            ModdedObject moddedObject = GetComponent<ModdedObject>();

            _canvasGroup = base.GetComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;

            _resultImageFrame = moddedObject.GetObject<GameObject>(0);
            _resultImageFrame.SetActive(false);

            _resultImage = moddedObject.GetObject<RawImage>(1);
        }

        private void Update()
        {
            _timeLeftToHideResult = Mathf.Max(0f, _timeLeftToHideResult - Time.unscaledDeltaTime);
            if (_resultImageFrame.activeSelf && _timeLeftToHideResult == 0f)
            {
                destroyRecentTexture();
                _resultImageFrame.SetActive(false);
            }
        }

        public void ShowResultImage(Texture2D texture)
        {
            destroyRecentTexture();

            _resultImageFrame.SetActive(true);
            _resultImage.texture = texture;
            _timeLeftToHideResult = 7f;
        }

        private void destroyRecentTexture()
        {
            Texture texture = _resultImage.texture;
            if (texture) Destroy(texture);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }
    }
}