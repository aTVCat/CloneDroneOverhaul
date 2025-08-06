using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotOverlay : MonoBehaviour
    {
        private RawImage m_resultImage;

        private float m_timeLeftToHideResult;

        private void Start()
        {
            m_resultImage = GetComponent<ModdedObject>().GetObject<RawImage>(0);
        }

        private void Update()
        {
            m_timeLeftToHideResult = Mathf.Max(0f, m_timeLeftToHideResult - Time.unscaledDeltaTime);
            if (m_resultImage.enabled && m_timeLeftToHideResult == 0f)
            {
                destroyRecentTexture();
                m_resultImage.enabled = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot()
        {
            destroyRecentTexture();

            Texture2D texture = PersonalizationEditorScreenshotManager.Instance.TakeScreenshotOfObject(1024, 1024);
            m_resultImage.enabled = true;
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
