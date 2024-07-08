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

            string displayName;
            switch (fileName)
            {
                case "ExperimentalLevelEditorUIV1":
                    displayName = "Level editor UI rework v1";
                    break;
                case "ExperimentalLevelEditorUIV2":
                    displayName = "Level editor UI rework v2";
                    break;
                case "ArenaOverhaulWip":
                    displayName = "Early arena overhaul";
                    break;
                case "OldPersonalizationEditor":
                    displayName = "Old customization editor UI";
                    break;
                case "OldPersonalizationEditor2":
                    displayName = "Early implementation of customization editor";
                    break;
                case "OldErrorWindow":
                    displayName = "Early implementation of crash screen redesign";
                    break;
                case "OldV4SettingsMenu":
                    displayName = "Old settings menu redesign 1";
                    break;
                case "OldWorkshopBrowserRework":
                    displayName = "Old overhauled workshop browser V1";
                    break;
                case "OldWorkshopBrowserRework2":
                    displayName = "Old overhauled workshop browser V2";
                    break;
                case "OldWorkshopBrowserRework3":
                    displayName = "Old overhauled workshop browser V3 (old)";
                    break;
                case "OldPauseMenuRework":
                    displayName = "Old pause menu redesign";
                    break;
                case "OldAchievementsMenuRework":
                    displayName = "Old achievements menu redesign";
                    break;
                case "OldTitleScreenRework":
                    displayName = "Overhauled title screen V1";
                    break;
                case "OldTitleScreenRework2":
                    displayName = "Overhauled title screen V2 (old)";
                    break;
                case "OldPhotoModeRework":
                    displayName = "Old advanced photo mode";
                    break;
                case "OldCustomizationBrowser":
                    displayName = "Old player customization menu";
                    break;
                case "ScrappedGMSSRework":
                    displayName = "Scrapped game mode selection menu";
                    break;
                case "OldUpdatesMenu":
                    displayName = "Old updates menu";
                    break;
                case "OldNewsMenu":
                    displayName = "Old news menu";
                    break;
                case "OldV4SettingsMenu2":
                    displayName = "Old settings menu redesign 2";
                    break;
                case "QuickReset":
                    displayName = "Quick reset - scrapped feature";
                    break;
                case "OldEndlessModeMenu":
                    displayName = "Old endless mode menu";
                    break;
                default:
                    displayName = fileName;
                    break;
            }

            string translationKey = $"behind_the_scenes_{displayName.ToLower().Replace(' ', '_').Replace("(", string.Empty).Replace(")", string.Empty)}";
            displayName = LocalizationManager.Instance.GetTranslatedString($"{translationKey}_name");
            string description = LocalizationManager.Instance.GetTranslatedString($"{translationKey}_description");

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
