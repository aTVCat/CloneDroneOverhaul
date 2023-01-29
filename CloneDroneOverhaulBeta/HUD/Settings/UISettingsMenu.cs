using CDOverhaul.Gameplay;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Sony.NP.Commerce;

namespace CDOverhaul.HUD
{
    public class UISettingsMenu : UIBase
    {
        private ModdedObject _categoryEntryPrefab;
        private Transform _categoryContainer;

        private Transform _mainContainer;
        private ModdedObject _sectionPrefab;

        public override void Initialize()
        {
            _categoryEntryPrefab = MyModdedObject.GetObject<ModdedObject>(1);
            _categoryEntryPrefab.gameObject.SetActive(false);
            _categoryContainer = MyModdedObject.GetObject<Transform>(2);
            _mainContainer = MyModdedObject.GetObject<Transform>(3);
            _sectionPrefab = MyModdedObject.GetObject<ModdedObject>(4);
            _sectionPrefab.gameObject.SetActive(false);

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
            foreach(string category in categories)
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
            foreach(string sectionName in sections)
            {
                ModdedObject categoryEntry = Instantiate(_sectionPrefab, _mainContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = sectionName;

                List<string> settings = SettingsController.GetAllSettings(categoryName, sectionName);
                foreach(string settingName in settings)
                {

                }
            }
        }
    }
}