using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementGalleryImage : OverhaulUIBehaviour
    {
        [UIElement("Title")]
        public Text m_titleText;

        [UIElement("Description")]
        public Text m_descriptionText;

        [UIElement("Image")]
        public RawImage m_image;

        private Texture2D m_texture;

        public string filePath
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            GetDescription();
            GetImage();

            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                ModUIUtils.ImageViewer(m_texture, ModCache.gameUIRoot.transform);
            });
        }

        public override void OnDestroy()
        {
            if (m_texture)
                Destroy(m_texture);
        }

        public void GetDescription()
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('/') + 1).Replace(".jpg", string.Empty);
            string displayName = fileName;
            string description = "No description provided.";
            switch (fileName)
            {
                case "ExperimentalLevelEditorUIV1":
                    displayName = "Level editor UI rework v1";
                    description = "This existed during early mod development. Many modded UIs looked weird back then.";
                    break;
                case "ExperimentalLevelEditorUIV2":
                    displayName = "Level editor UI rework v2";
                    description = "Made during overhaul v2 development. The UI was hard to code so it was scrapped.";
                    break;
                case "ArenaOverhaulWip":
                    displayName = "Early arena overhaul";
                    description = "This is how overhauled arena looked like during v3 development. It also had enemy count displays on walls.";
                    break;
                case "OldPersonalizationEditor":
                    displayName = "Old customization editor UI";
                    description = "This was made before player customization system was re-implemented.";
                    break;
                case "OldV4SettingsMenu":
                    displayName = "Old overhauled settings menu";
                    description = "This was done before getting combined into one UI.";
                    break;
                case "OldWorkshopBrowserRework":
                    displayName = "Old overhauled workshop browser";
                    description = "The first iteration of reworked workshop browser.";
                    break;
                case "OldWorkshopBrowserRework2":
                    displayName = "Old overhauled workshop browser (again)";
                    description = "The second iteration of reworked workshop browser. Tabs feature was supposed to be added so you could check several levels at the same time.";
                    break;
                case "OldTitleScreenRework":
                    displayName = "Old overhauled title screen";
                    description = "made before i decided to completely redesign the mod";
                    break;
                case "OldPhotoModeRework":
                    displayName = "Old advanced photo mode";
                    description = "This is how photo mode looked like in V2 developer builds.";
                    break;
                case "OldCustomizationBrowser":
                    displayName = "Old player customization browser";
                    description = "It was made under 30 minutes.";
                    break;
                case "ScrappedGMSSRework":
                    displayName = "Scrapped game mode selection menu";
                    description = "It was scrapped due to technical reasons.";
                    break;
            }

            m_titleText.text = displayName;
            m_descriptionText.text = description;
        }

        public void GetImage()
        {
            byte[] bytes;
            try
            {
                bytes = ModIOUtils.ReadBytes(filePath);
            }
            catch
            {
                base.gameObject.SetActive(false);
                return;
            }

            Texture2D texture = new Texture2D(1, 1);
            _ = texture.LoadImage(bytes);
            texture.Apply();
            m_texture = texture;
            m_image.texture = texture;
        }
    }
}
