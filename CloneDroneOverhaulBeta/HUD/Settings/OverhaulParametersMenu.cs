using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulParametersMenu : OverhaulUI
    {
        private ModdedObject m_CategoryEntryPrefab;
        private Transform m_CategoryContainer;

        private Transform m_MainContainer;
        private ModdedObject m_SectionPrefab;
        private ModdedObject m_SettingPrefab;

        private Transform m_DescriptionTransform;

        public override void Initialize()
        {
            m_CategoryEntryPrefab = MyModdedObject.GetObject<ModdedObject>(1);
            m_CategoryEntryPrefab.gameObject.SetActive(false);
            m_CategoryContainer = MyModdedObject.GetObject<Transform>(2);
            m_MainContainer = MyModdedObject.GetObject<Transform>(3);
            m_SectionPrefab = MyModdedObject.GetObject<ModdedObject>(4);
            m_SectionPrefab.gameObject.SetActive(false);
            m_SettingPrefab = MyModdedObject.GetObject<ModdedObject>(6);
            m_SettingPrefab.gameObject.SetActive(false);
            m_DescriptionTransform = MyModdedObject.GetObject<Transform>(7);
            MyModdedObject.GetObject<Button>(5).onClick.AddListener(Hide);

            Hide();
        }

        protected override void OnDisposed()
        {
            m_CategoryContainer = null;
            m_CategoryEntryPrefab = null;
            m_MainContainer = null;
            m_SectionPrefab = null;
            m_SettingPrefab = null;
            m_DescriptionTransform = null;
        }

        public void Show()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(false);
            }

            base.gameObject.SetActive(true);
            populateCategories();
            PopulateDescription(null, null);

            DelegateScheduler.Instance.Schedule(delegate
            {
                ParametersMenuCategoryButton.SetSelectedFirst();
            }, 0.1f);
        }

        public void Hide()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(true);
            }
            base.gameObject.SetActive(false);
        }

        private void populateCategories()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(m_CategoryContainer);
            List<string> categories = SettingsController.GetAllCategories();
            foreach (string category in categories)
            {
                ModdedObject categoryEntry = Instantiate(m_CategoryEntryPrefab, m_CategoryContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = category;
                categoryEntry.gameObject.AddComponent<ParametersMenuCategoryButton>().Initialize(this, categoryEntry, category);
            }
        }

        public void PopulateCategory(in string categoryName)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(m_MainContainer);

            List<string> sections = SettingsController.GetAllSections(categoryName);
            foreach (string sectionName in sections)
            {
                string[] array = sectionName.Split('.');
                ModdedObject categoryEntry = Instantiate(m_SectionPrefab, m_MainContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = array[1];

                List<string> settings = SettingsController.GetAllSettings(categoryName, array[1]);
                foreach (string settingName in settings)
                {
                    List<string> childrenSettings = SettingsController.GetChildrenSettings(settingName);
                    PopulateSetting(settingName, childrenSettings.Count == 0 ? PerametersMenuSettingPosition.Normal : PerametersMenuSettingPosition.Top);
                    int index = 0;
                    if (childrenSettings.Count != 0)
                    {
                        foreach (string cSettingName in childrenSettings)
                        {
                            PerametersMenuSettingPosition pos = PerametersMenuSettingPosition.Center;
                            if (childrenSettings.Count - 1 == index)
                            {
                                pos = PerametersMenuSettingPosition.Bottom;
                            }
                            PopulateSetting(cSettingName, pos);
                            index++;
                        }
                    }
                }
            }
        }

        public void PopulateSetting(in string path, in PerametersMenuSettingPosition position)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            ModdedObject setting = Instantiate(m_SettingPrefab, m_MainContainer);
            setting.gameObject.SetActive(true);
            setting.gameObject.AddComponent<ParametersMenuSetting>().Initialize(this, setting, path, position);
        }

        public void PopulateDescription(in SettingInfo info, in SettingDescription description)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (info == null || description == null)
            {
                m_DescriptionTransform.gameObject.SetActive(false);
                return;
            }
            m_DescriptionTransform.gameObject.SetActive(true);

            MyModdedObject.GetObject<Text>(8).text = info.Name;
            MyModdedObject.GetObject<Text>(9).text = description.Description;
            MyModdedObject.GetObject<Image>(10).gameObject.SetActive(description.Has43Image);
            MyModdedObject.GetObject<Image>(11).gameObject.SetActive(description.Has169Image);
            MyModdedObject.GetObject<Image>(10).sprite = description.Image_4_3;
            MyModdedObject.GetObject<Image>(11).sprite = description.Image_16_9;
        }
    }
}