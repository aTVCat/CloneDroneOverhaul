using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementGalleryImage : OverhaulUIBehaviour
    {
        [UIElement("Title")]
        public Text _titleText;

        [UIElement("Description")]
        public Text _descriptionText;

        [UIElement("Image")]
        public RawImage _image;

        private Texture2D _texture;

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
                ModUIUtils.ImageViewer(_texture, ModCache.gameUIRoot.transform);
            });
        }

        public override void OnDestroy()
        {
            if (_texture)
                Destroy(_texture);
        }

        public void GetDescription()
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

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

            _titleText.text = displayName;
            _descriptionText.text = description;
        }

        public void GetImage()
        {
            byte[] bytes;
            try
            {
                bytes = ModFileUtils.ReadBytes(filePath);
            }
            catch
            {
                base.gameObject.SetActive(false);
                return;
            }

            Texture2D texture = new Texture2D(1, 1);
            _ = texture.LoadImage(bytes);
            texture.Apply();
            _texture = texture;
            _image.texture = texture;
        }
    }
}
