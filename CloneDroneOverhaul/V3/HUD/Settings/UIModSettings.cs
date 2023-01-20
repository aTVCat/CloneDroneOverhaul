using System;
using System.Collections.Generic;
using System.Linq;
using CloneDroneOverhaul.V3.Utilities;
using CloneDroneOverhaul.V3.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UIModSettings : V3_ModHUDBase
    {
        private bool _hasInitialized;

        private ModdedObject _categoryEntry;
        private Transform _categoryContainer;
        private List<UISettingCategoryButtonBehaviour> _spawnedCategoryButtons = new List<UISettingCategoryButtonBehaviour>();
        private List<UISettingButtonBehaviour> _spawnedButtonBehaviours = new List<UISettingButtonBehaviour>();

        private Transform _settingsContainer;
        private ModdedObject _containerHeaderPrefab;
        private ModdedObject _containerBoolSettingPrefab;
        private ModdedObject _containerFloatOrIntSettingPrefab;
        private ModdedObject _containerStringSettingPrefab;
        private ModdedObject _containerDropdownSettingPrefab;

        public string SelectedCategory { get; set; }

        private void Start()
        {
            _hasInitialized = true;

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(Hide);

            _categoryEntry = MyModdedObject.GetObjectFromList<ModdedObject>(1);
            _categoryContainer = MyModdedObject.GetObjectFromList<Transform>(2);

            _categoryEntry.gameObject.SetActive(false);

            _settingsContainer = MyModdedObject.GetObjectFromList<Transform>(5);
            _containerHeaderPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(3);
            _containerHeaderPrefab.gameObject.SetActive(false);
            _containerBoolSettingPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(4);
            _containerBoolSettingPrefab.gameObject.SetActive(false);
            _containerFloatOrIntSettingPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(6);
            _containerFloatOrIntSettingPrefab.gameObject.SetActive(false);
            _containerStringSettingPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(7);
            _containerStringSettingPrefab.gameObject.SetActive(false);
            _containerDropdownSettingPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(8);
            _containerDropdownSettingPrefab.gameObject.SetActive(false);

            base.gameObject.SetActive(false);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);

            populateCategories();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        private void populateCategories()
        {
            _spawnedCategoryButtons.Clear();
            TransformUtils.DestroyAllChildren(_categoryContainer);
            List<string> list = new List<string>();
            foreach(ModSetting setting in ModSettingsController.Settings)
            {
                if (setting == null)
                {
                    break;
                }
                if (!list.Contains(setting.SettingCategory))
                {
                    list.Add(setting.SettingCategory);

                    ModdedObject prefab = Instantiate<ModdedObject>(_categoryEntry, _categoryContainer);
                                 prefab.gameObject.SetActive(true);
                    _spawnedCategoryButtons.Add(prefab.gameObject.AddComponent<UISettingCategoryButtonBehaviour>().Initialize(setting.SettingCategory, this));
                }
            }
            _spawnedCategoryButtons[0].Select();
        }

        private void populateSettings()
        {
            _spawnedButtonBehaviours.Clear();
            TransformUtils.DestroyAllChildren(_settingsContainer);

            foreach (string section in ModSettingsController.CachedSections[SelectedCategory])
            {
                ModdedObject prefab1 = Instantiate<ModdedObject>(_containerHeaderPrefab, _settingsContainer);
                prefab1.gameObject.SetActive(true);
                prefab1.GetComponent<Text>().text = section;

                foreach (ModSetting setting in ModSettingsController.Settings)
                {
                    if (setting == null)
                    {
                        break;
                    }

                    if (setting.SettingCategory == SelectedCategory && setting.SettingSection == section && !setting.IsHiddenFromMainView)
                    {
                        PopulateSetting(setting);
                    }
                }
            }
        }

        public void PopulateSetting(in ModSetting setting)
        {
            ModdedObject prefab = null;
            if (setting.Type == EPlayerPrefType.Bool)
            {
                prefab = _containerBoolSettingPrefab;
            }
            if (setting.Type == EPlayerPrefType.Int || setting.Type == EPlayerPrefType.Float)
            {
                if (setting.Type == EPlayerPrefType.Int && setting.Dropdown_Options != null)
                {
                    prefab = _containerDropdownSettingPrefab;
                }
                else
                {
                    prefab = _containerFloatOrIntSettingPrefab;
                }
            }
            if (setting.Type == EPlayerPrefType.String)
            {
                prefab = _containerStringSettingPrefab;
            }

            ModdedObject prefab2 = Instantiate<ModdedObject>(prefab, _settingsContainer);
            prefab2.gameObject.SetActive(true);
            UISettingButtonBehaviour behaviour = prefab2.gameObject.AddComponent<UISettingButtonBehaviour>().Initialize(setting, this);
            _spawnedButtonBehaviours.Add(behaviour);
            if (setting.HasChildrenSettings)
            {
                foreach(ModSetting cSetting in setting.ChildrenSettings)
                {
                    PopulateSetting(cSetting);
                }
            }
        }

        public void ShowSettings(List<ModSetting> settings)
        {
            foreach(UISettingButtonBehaviour b in _spawnedButtonBehaviours)
            {
                if (b.IsHidden && settings.Contains(b.MySetting))
                {
                    b.gameObject.SetActive(true);
                }
            }
        }

        public void HideSettings(List<ModSetting> settings)
        {
            foreach (UISettingButtonBehaviour b in _spawnedButtonBehaviours)
            {
                if (b.IsHidden && settings.Contains(b.MySetting))
                {
                    b.gameObject.SetActive(false);
                }
            }
        }

        public void SelectCategory(in string category)
        {
            SelectedCategory = category;
            foreach(UISettingCategoryButtonBehaviour b in _spawnedCategoryButtons)
            {
                b.SetSelected(false);
            }
            populateSettings();
        }
    }
}
