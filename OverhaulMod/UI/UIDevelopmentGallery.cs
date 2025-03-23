using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDevelopmentGallery : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("ImageDisplay", false)]
        private readonly ModdedObject m_imageDisplay;

        [UIElement("Content")]
        private readonly Transform m_imageDisplaysContainer;

        public override bool hideTitleScreen => true;

        public override void Show()
        {
            base.Show();
            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            if (m_imageDisplaysContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_imageDisplaysContainer);
        }

        public void Populate()
        {
            _ = base.StartCoroutine(populateCoroutine());
        }

        private IEnumerator populateCoroutine()
        {
            if (m_imageDisplaysContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_imageDisplaysContainer);

            if (!AddonManager.Instance.HasInstalledAddon(AddonManager.GALLERY_ADDON_ID, out string directory))
            {
                ModUIUtils.MessagePopupOK("\"Behind The Scenes\" addon not installed", "Install this addon to make this menu work.", true);
                yield break;
            }

            string[] images = Directory.GetFiles(directory, "*.jpg");
            if (images.IsNullOrEmpty())
                yield break;

            int counter = 0;
            foreach (string imageFilePath in images)
            {
                ModdedObject moddedObject = Instantiate(m_imageDisplay, m_imageDisplaysContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementGalleryImage galleryImage = moddedObject.gameObject.AddComponent<UIElementGalleryImage>();
                galleryImage.filePath = imageFilePath;
                galleryImage.InitializeElement();

                counter++;

                if (counter % 4 == 0)
                    yield return null;
            }
            yield break;
        }
    }
}
