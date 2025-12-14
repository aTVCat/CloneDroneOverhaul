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
        private CanvasGroup m_canvasGroup;

        private RawImage m_resultImage;

        private GameObject m_resultImageFrame;

        private GameObject m_autoScreenshotSettingsPanel;

        private Dropdown m_categoryDropdown;

        private Dropdown m_weaponTypeDropdown;

        private float m_timeLeftToHideResult;

        private string m_overrideItemFolder;

        private void Start()
        {
            ModdedObject moddedObject = GetComponent<ModdedObject>();

            m_canvasGroup = base.GetComponent<CanvasGroup>();
            m_canvasGroup.blocksRaycasts = false;

            m_resultImage = moddedObject.GetObject<RawImage>(0);
            m_resultImageFrame = m_resultImage.transform.parent.gameObject;
            m_resultImageFrame.SetActive(false);

            m_autoScreenshotSettingsPanel = moddedObject.GetObject<GameObject>(1);
            m_autoScreenshotSettingsPanel.SetActive(false);
            moddedObject.GetObject<Button>(2).onClick.AddListener(HideAutoScreenshotSettingsPanel);
            moddedObject.GetObject<Button>(5).onClick.AddListener(TakeScreenshotsOfMultipleItems);

            m_categoryDropdown = moddedObject.GetObject<Dropdown>(3);
            System.Collections.Generic.List<Dropdown.OptionData> options = m_categoryDropdown.options;
            options.Clear();

            foreach (PersonalizationCategory category in typeof(PersonalizationCategory).GetEnumValues())
                options.Add(new DropdownIntOptionData() { IntValue = (int)category, text = StringUtils.AddSpacesToCamelCasedString(category.ToString()) });

            m_weaponTypeDropdown = moddedObject.GetObject<Dropdown>(4);
            System.Collections.Generic.List<Dropdown.OptionData> options2 = m_weaponTypeDropdown.options;
            options2.Clear();
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Sword, text = "Sword" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Bow, text = "Bow" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Hammer, text = "Hammer" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)WeaponType.Spear, text = "Spear" });
            options2.Add(new DropdownIntOptionData() { IntValue = (int)ModWeaponsManager.SCYTHE_TYPE, text = "Scythe" });
        }

        private void Update()
        {
            m_timeLeftToHideResult = Mathf.Max(0f, m_timeLeftToHideResult - Time.unscaledDeltaTime);
            if (m_resultImageFrame.activeSelf && m_timeLeftToHideResult == 0f)
            {
                destroyRecentTexture();
                m_resultImageFrame.SetActive(false);
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
            PersonalizationCategory personalizationCategory = (PersonalizationCategory)(m_categoryDropdown.options[m_categoryDropdown.value] as DropdownIntOptionData).IntValue;
            WeaponType weaponType = (WeaponType)(m_weaponTypeDropdown.options[m_weaponTypeDropdown.value] as DropdownIntOptionData).IntValue;
            PersonalizationEditorScreenshotCameraAnglesInfo angles = stage.GetCameraAnglesInfo();
            Transform cameraTransform = stage.GetCameraController().transform;

            for (int i = 0; i < items.Count; i++)
            {
                PersonalizationItemInfo item = items[i];
                if (item.Category != personalizationCategory) continue;

                if (item.Category == PersonalizationCategory.WeaponSkins && item.Weapon != weaponType) continue;

                UIPersonalizationEditor.instance.Utilities.SetRandomFavoriteColor();

                m_overrideItemFolder = item.FolderPath;
                stage.SpawnItemInHolder(items[i]);

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
            Texture2D texture = PersonalizationEditorScreenshotStage.Instance.TakeScreenshotOfObject(256, 256, 1);
            QualitySettings.antiAliasing = antiAliasingBefore;
            m_resultImageFrame.SetActive(true);
            m_resultImage.texture = texture;
            m_timeLeftToHideResult = 5f;

            string folderPath = m_overrideItemFolder ?? PersonalizationEditorManager.Instance.currentEditingItemFolder;
            string path = Path.Combine(folderPath, "preview.png");
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

        public void ShowAutoScreenshotSettingsPanel()
        {
            m_autoScreenshotSettingsPanel.SetActive(true);
            m_canvasGroup.blocksRaycasts = true;
        }

        public void HideAutoScreenshotSettingsPanel()
        {
            m_autoScreenshotSettingsPanel.SetActive(false);
            m_canvasGroup.blocksRaycasts = false;
        }
    }
}
