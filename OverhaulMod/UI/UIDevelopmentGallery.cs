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
        private readonly Button _exitButton;

        [UIElement("ImageDisplay", false)]
        private readonly ModdedObject _imageDisplay;

        [UIElement("Content")]
        private readonly Transform _imageDisplaysContainer;

        public override bool HideTitleScreen => true;

        public override void Show()
        {
            base.Show();
            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            if (_imageDisplaysContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_imageDisplaysContainer);
        }

        public void Populate()
        {
            _ = base.StartCoroutine(populateCoroutine());
        }

        private IEnumerator populateCoroutine()
        {
            if (_imageDisplaysContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_imageDisplaysContainer);

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
                ModdedObject moddedObject = Instantiate(_imageDisplay, _imageDisplaysContainer);
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
