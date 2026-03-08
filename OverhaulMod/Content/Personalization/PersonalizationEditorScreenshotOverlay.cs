using OverhaulMod.Combat;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotOverlay : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private RawImage _resultImage;

        private GameObject _resultImageFrame;

        private GameObject _autoScreenshotSettingsPanel;

        private Dropdown _categoryDropdown;

        private Dropdown _weaponTypeDropdown;

        private float _timeLeftToHideResult;

        private string _overrideItemFolder;

        private void Start()
        {
            ModdedObject moddedObject = GetComponent<ModdedObject>();

            _canvasGroup = base.GetComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;

            _resultImage = moddedObject.GetObject<RawImage>(0);
            _resultImageFrame = _resultImage.transform.parent.gameObject;
            _resultImageFrame.SetActive(false);

            _autoScreenshotSettingsPanel = moddedObject.GetObject<GameObject>(1);
            _autoScreenshotSettingsPanel.SetActive(false);
            moddedObject.GetObject<Button>(2).onClick.AddListener(HideAutoScreenshotSettingsPanel);
            moddedObject.GetObject<Button>(5).onClick.AddListener(TakeScreenshotsOfMultipleItems);

            _categoryDropdown = moddedObject.GetObject<Dropdown>(3);
            System.Collections.Generic.List<Dropdown.OptionData> options = _categoryDropdown.options;
            options.Clear();

            foreach (PersonalizationCategory category in typeof(PersonalizationCategory).GetEnumValues())
                options.Add(new DropdownIntOptionData() { IntValue = (int)category, text = StringUtils.AddSpacesToCamelCasedString(category.ToString()) });

            _weaponTypeDropdown = moddedObject.GetObject<Dropdown>(4);
            System.Collections.Generic.List<Dropdown.OptionData> options2 = _weaponTypeDropdown.options;
            options2.Clear();
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Sword, text = "Sword" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Bow, text = "Bow" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Hammer, text = "Hammer" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Spear, text = "Spear" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)ModWeaponsManager.SCYTHE_TYPE, text = "Scythe" });
        }

        private void Update()
        {
            _timeLeftToHideResult = Mathf.Max(0f, _timeLeftToHideResult - Time.unscaledDeltaTime);
            if (_resultImageFrame.activeSelf && _timeLeftToHideResult == 0f)
            {
                destroyRecentTexture();
                _resultImageFrame.SetActive(false);
            }

            if (InputManager.Instance.GetKeyMode() != KeyMode.GeneralCommands) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                TakeScreenshot();
            }
            else if (Input.GetKeyDown(KeyCode.T) && ModUserInfo.isDeveloper)
            {
                SaveAngle();
            }
            else if (Input.GetKeyDown(KeyCode.U) && ModUserInfo.isDeveloper)
            {
                ShowAutoScreenshotSettingsPanel();
            }
        }

        public void SaveAngle()
        {
            ModUIUtils.InputFieldWindow("Type angle name", "bbb", string.Empty, 0, 125f, delegate (string str)
            {
                PersonalizationEditorScreenshotStage manager = PersonalizationEditorScreenshotStage.Instance;

                PersonalizationEditorScreenshotCameraAngle angle = manager.GetCameraAnglesInfo().GetAngle(str);
                if (angle == null)
                {
                    angle = new PersonalizationEditorScreenshotCameraAngle()
                    {
                        Name = str,
                    };
                }
                angle.SetAngleFromTransform(manager.GetCameraController().transform);

                PersonalizationEditorScreenshotCameraAnglesInfo anglesInfo = manager.GetCameraAnglesInfo();
                anglesInfo.Angles.Add(angle);

                manager.SaveCameraAnglesInfo();
            });
        }

        public void TakeScreenshotsOfMultipleItems()
        {
            HideAutoScreenshotSettingsPanel();
            takeScreenshotsOfMultipleItemsCoroutine().Run();
        }

        private IEnumerator takeScreenshotsOfMultipleItemsCoroutine()
        {
            System.Collections.Generic.List<PersonalizationItemInfo> items = PersonalizationManager.Instance.itemList.GetItems();
            PersonalizationEditorScreenshotStage stage = PersonalizationEditorScreenshotStage.Instance;
            PersonalizationCategory personalizationCategory = (PersonalizationCategory)(_categoryDropdown.options[_categoryDropdown.value] as DropdownIntOptionData).IntValue;
            WeaponType weaponType = (WeaponType)(_weaponTypeDropdown.options[_weaponTypeDropdown.value] as DropdownIntOptionData).IntValue;
            PersonalizationEditorScreenshotCameraAnglesInfo angles = stage.GetCameraAnglesInfo();
            Transform cameraTransform = stage.GetCameraController().transform;

            for (int i = 0; i < items.Count; i++)
            {
                PersonalizationItemInfo item = items[i];
                if (item.Category != personalizationCategory) continue;

                if (item.Category == PersonalizationCategory.WeaponSkins && item.Weapon != weaponType) continue;

                if (File.Exists(Path.Combine(item.FolderPath, "preview.png"))) continue;

                UIPersonalizationEditor.instance.Utilities.SetRandomFavoriteColor();
                PersonalizationEditorManager.Instance.currentEditingItemInfo = item;

                _overrideItemFolder = item.FolderPath;
                stage.SpawnItemInHolder(item);

                PersonalizationEditorScreenshotCameraAngle angle = angles.GetAngle(weaponType == ModWeaponsManager.SCYTHE_TYPE ? "Scythe" : weaponType.ToString());
                angle.ApplyToTransform(cameraTransform);

                for (int j = 0; j < 30; j++)
                    yield return null;

                TakeScreenshot();
            }

            ModUIUtils.MessagePopupOK("Done!", "bbb", true);

            yield break;
        }

        public void TakeScreenshot()
        {
            destroyRecentTexture();

            int antiAliasingBefore = QualitySettings.antiAliasing;
            QualitySettings.antiAliasing = 8;
            Texture2D texture = PersonalizationEditorScreenshotStage.Instance.TakeScreenshotOfObject(128, 128, 1);
            QualitySettings.antiAliasing = antiAliasingBefore;
            _resultImageFrame.SetActive(true);
            _resultImage.texture = texture;
            _timeLeftToHideResult = 5f;

            string folderPath = _overrideItemFolder ?? PersonalizationEditorManager.Instance.currentEditingItemFolder;
            string path = Path.Combine(folderPath, "preview.png");
            ModFileUtils.WriteBytes(texture.EncodeToPNG(), path);
        }

        private void destroyRecentTexture()
        {
            Texture texture = _resultImage.texture;
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

        public void ShowAutoScreenshotSettingsPanel()
        {
            _autoScreenshotSettingsPanel.SetActive(true);
            _canvasGroup.blocksRaycasts = true;
        }

        public void HideAutoScreenshotSettingsPanel()
        {
            _autoScreenshotSettingsPanel.SetActive(false);
            _canvasGroup.blocksRaycasts = false;
        }
    }
}
