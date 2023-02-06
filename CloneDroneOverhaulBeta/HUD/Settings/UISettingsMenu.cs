using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UISettingsMenu : UIBase
    {
        private ModdedObject _categoryEntryPrefab;
        private Transform _categoryContainer;

        private Transform _mainContainer;
        private ModdedObject _sectionPrefab;
        private ModdedObject _settingPrefab;

        private Transform _description;

        public override void Initialize()
        {
            _categoryEntryPrefab = MyModdedObject.GetObject<ModdedObject>(1);
            _categoryEntryPrefab.gameObject.SetActive(false);
            _categoryContainer = MyModdedObject.GetObject<Transform>(2);
            _mainContainer = MyModdedObject.GetObject<Transform>(3);
            _sectionPrefab = MyModdedObject.GetObject<ModdedObject>(4);
            _sectionPrefab.gameObject.SetActive(false);
            _settingPrefab = MyModdedObject.GetObject<ModdedObject>(6);
            _settingPrefab.gameObject.SetActive(false);
            _description = MyModdedObject.GetObject<Transform>(7);

            MyModdedObject.GetObject<Button>(5).onClick.AddListener(Hide);

            Hide();
            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        public void Show()
        {
            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(false);
            }

            base.gameObject.SetActive(true);
            populateCategories();
            PopulateDescription(null, null);
        }

        public void Hide()
        {
            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(true);
            }
            base.gameObject.SetActive(false);
        }

        private void populateCategories()
        {
            TransformUtils.DestroyAllChildren(_categoryContainer);

            List<string> categories = SettingsController.GetAllCategories();
            foreach (string category in categories)
            {
                ModdedObject categoryEntry = Instantiate(_categoryEntryPrefab, _categoryContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = category;
                categoryEntry.gameObject.AddComponent<SettingsMenuCategoryEntryBehaviour>().Initialize(this, categoryEntry, category);
            }
        }

        public void PopulateCategory(in string categoryName)
        {
            TransformUtils.DestroyAllChildren(_mainContainer);

            List<string> sections = SettingsController.GetAllSections(categoryName);
            foreach (string sectionName in sections)
            {
                string[] array = sectionName.Split('.');
                ModdedObject categoryEntry = Instantiate(_sectionPrefab, _mainContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = array[1];

                List<string> settings = SettingsController.GetAllSettings(categoryName, array[1]);
                settings.Sort();
                foreach (string settingName in settings)
                {
                    List<string> childrenSettings = SettingsController.GetChildrenSettings(settingName);
                    PopulateSetting(settingName, childrenSettings.Count == 0 ? ESettingPosition.Normal : ESettingPosition.Top);
                    int index = 0;
                    if (childrenSettings.Count != 0)
                    {
                        foreach (string cSettingName in childrenSettings)
                        {
                            ESettingPosition pos = ESettingPosition.Center;
                            if (childrenSettings.Count - 1 == index)
                            {
                                pos = ESettingPosition.Bottom;
                            }
                            PopulateSetting(cSettingName, pos);
                            index++;
                        }
                    }
                }
            }
        }

        public void PopulateSetting(in string path, in ESettingPosition position)
        {
            ModdedObject setting = Instantiate(_settingPrefab, _mainContainer);
            setting.gameObject.SetActive(true);
            setting.gameObject.AddComponent<SettingEntryBehaviour>().Initialize(this, setting, path, position);
        }

        public void PopulateDescription(in SettingInfo info, in SettingDescription description)
        {
            if (info == null || description == null)
            {
                _description.gameObject.SetActive(false);
                return;
            }
            _description.gameObject.SetActive(true);

            MyModdedObject.GetObject<Text>(8).text = info.Name;
            MyModdedObject.GetObject<Text>(9).text = description.Description;
            MyModdedObject.GetObject<Image>(10).gameObject.SetActive(description.Has43Image);
            MyModdedObject.GetObject<Image>(11).gameObject.SetActive(description.Has169Image);
            MyModdedObject.GetObject<Image>(10).sprite = description.Image_4_3;
            MyModdedObject.GetObject<Image>(11).sprite = description.Image_16_9;
        }
    }
}