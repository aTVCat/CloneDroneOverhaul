using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotOverlay : MonoBehaviour
    {
        private RawImage m_resultImage;

        private GameObject m_resultImageFrame;

        private float m_timeLeftToHideResult;

        private void Start()
        {
            m_resultImage = GetComponent<ModdedObject>().GetObject<RawImage>(0);
            m_resultImageFrame = m_resultImage.transform.parent.gameObject;
            m_resultImageFrame.SetActive(false);
        }

        private void Update()
        {
            m_timeLeftToHideResult = Mathf.Max(0f, m_timeLeftToHideResult - Time.unscaledDeltaTime);
            if (m_resultImageFrame.activeSelf && m_timeLeftToHideResult == 0f)
            {
                destroyRecentTexture();
                m_resultImageFrame.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot()
        {
            destroyRecentTexture();

            int antiAliasingBefore = QualitySettings.antiAliasing;
            QualitySettings.antiAliasing = 8;
            Texture2D texture = PersonalizationEditorScreenshotManager.Instance.TakeScreenshotOfObject(1024, 1024);
            QualitySettings.antiAliasing = antiAliasingBefore;
            m_resultImageFrame.SetActive(true);
            m_resultImage.texture = texture;
            m_timeLeftToHideResult = 5f;

            string itemInfo = PersonalizationEditorManager.Instance.currentEditingItemFolder;
            string path = Path.Combine(itemInfo, "preview.png");
            ModFileUtils.WriteBytes(texture.EncodeToPNG(), path);
        }

        private void destroyRecentTexture()
        {
            Texture texture = m_resultImage.texture;
            if (texture)
            {
                Destroy(texture);
            }
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
