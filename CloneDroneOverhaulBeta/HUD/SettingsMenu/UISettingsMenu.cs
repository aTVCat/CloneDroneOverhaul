using CDOverhaul.Gameplay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UISettingsMenu : UIController
    {
        public static readonly Color DefaultBarColor = new Color(0.25f, 0.4375f, 1f, 1f);

        [PrefabContainer(0, 2)]
        private readonly PrefabContainer m_CategoriesContainer;
        [PrefabContainer(1, 2)]
        private readonly PrefabContainer m_SeparatorsContainer;

        [PrefabContainer(4, 3)]
        private readonly PrefabContainer m_SettingInfoDisplaysContainer;
        [PrefabContainer(5, 3)]
        private readonly PrefabContainer m_SectionButtonsContainer;

        [UIElementReference("ThemeOutline")]
        private readonly Graphic m_ThemeOutline;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("EmptyListIndicator")]
        private readonly GameObject m_EmptyListIndicator;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("KeyBindingScreen")]
        private readonly GameObject m_KeyBindingScreen;

        private bool m_HasPopulatedCategories, m_IsBindingAKey;

        private OverhaulSettingsManager m_SettingsManager;

        public string selectedCategoryId { get; set; }
        public string selectedSectionId { get; set; }

        public UIElementSettingsMenuCategoryButton recentSelectedCategoryButton
        {
            get;
            set;
        }

        private UIElementSettingsMenuKeyBinder m_CurrentKeyBinder;
        public UIElementSettingsMenuKeyBinder currentKeyBinder
        {
            get => m_CurrentKeyBinder;
            set
            {
                m_CurrentKeyBinder = value;
                m_KeyBindingScreen.SetActive(value);
                m_IsBindingAKey = value;

                if (value)
                {
                    StartCoroutine(waitUntilKeyWasPressed());
                }
            }
        }

        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Initialize()
        {
            base.Initialize();
            m_SettingsManager = OverhaulSettingsManager.reference;
        }

        public override void Show()
        {
            base.Show();

            selectedCategoryId = "Home";
            selectedSectionId = string.Empty;
            m_ThemeOutline.color = OverhaulCombatState.GetUIThemeColor(DefaultBarColor);
            PopulateCategoryButtons();
            Populate();
        }

        public override void Update()
        {
            base.Update();
            if (m_IsBindingAKey)
            {
                if (Input.anyKeyDown && m_CurrentKeyBinder.onValueChanged != null)
                {
                    m_CurrentKeyBinder.BindKey(Event.current.keyCode);
                }
            }
        }

        public void PopulateCategoryButtons(bool force = false)
        {
            if (m_HasPopulatedCategories && !force)
                return;

            selectedCategoryId = "Home";
            m_CategoriesContainer.Clear();

            recentSelectedCategoryButton = InstantiateCategoryButton("Home");
            _ = m_SeparatorsContainer.InstantiateEntry();

            List<string> categories = m_SettingsManager.GetCategories();
            if (categories.IsNullOrEmpty())
            {
                OverhaulDebug.Warn("Settings - No categories to show", EDebugType.UI);
                return;
            }

            int categoryIndex = 0;
            do
            {
                _ = InstantiateCategoryButton(categories[categoryIndex]);

                categoryIndex++;
            } while (categoryIndex < categories.Count);

            m_HasPopulatedCategories = true;
        }

        public void Populate()
        {
            m_EmptyListIndicator.SetActive(false);
            m_SectionButtonsContainer.Clear();

            if (string.IsNullOrEmpty(selectedCategoryId))
            {
                m_EmptyListIndicator.SetActive(true);
                return;
            }

            if (string.IsNullOrEmpty(selectedSectionId))
            {
                // Sections stuff
                List<string> sections = m_SettingsManager.GetSections(selectedCategoryId);
                if (!sections.IsNullOrEmpty())
                {
                    int sectionIndex = 0;
                    do
                    {
                        _ = InstantiateSectionButton(selectedCategoryId, sections[sectionIndex]);

                        sectionIndex++;
                    } while (sectionIndex < sections.Count);
                }
                return;
            }

            List<OverhaulSettingInfo> settingInfos = m_SettingsManager.GetSettingInfos(selectedCategoryId, selectedSectionId);
            if(settingInfos.IsNullOrEmpty() && settingInfos.IsNullOrEmpty())
            {
                m_EmptyListIndicator.SetActive(true);
                return;
            }

            if (!settingInfos.IsNullOrEmpty())
            {
                int settingIndex = 0;
                do
                {
                    _ = InstantiateSettingInfoDisplay(settingInfos[settingIndex]);

                    settingIndex++;
                } while (settingIndex < settingInfos.Count);
            }
        }

        private IEnumerator waitUntilKeyWasPressed()
        {
            float timeToStop = Time.unscaledTime + 7f;
            yield return new WaitUntil(() => (Event.current.isKey && Event.current.keyCode != KeyCode.None) || Time.unscaledTime >= timeToStop);
            if(Time.unscaledTime < timeToStop)
            {
                currentKeyBinder.BindKey(Event.current.keyCode);
                currentKeyBinder = null;
            }
            yield break;
        }

        public UIElementSettingsMenuCategoryButton InstantiateCategoryButton(string category)
        {
            ModdedObject moddedObject = m_CategoriesContainer.InstantiateEntry();
            UIElementSettingsMenuCategoryButton categoryButton = moddedObject.gameObject.AddComponent<UIElementSettingsMenuCategoryButton>();
            categoryButton.categoryId = category;
            return categoryButton;
        }

        public UIElementSettingsMenuSectionButton InstantiateSectionButton(string category, string section)
        {
            ModdedObject moddedObject = m_SectionButtonsContainer.InstantiateEntry();
            UIElementSettingsMenuSectionButton sectionButton = moddedObject.gameObject.AddComponent<UIElementSettingsMenuSectionButton>();
            sectionButton.categoryId = category;
            sectionButton.sectionId = section;
            return sectionButton;
        }

        public UIElementSettingsMenuSettingInfoDisplay InstantiateSettingInfoDisplay(OverhaulSettingInfo settingInfo)
        {
            ModdedObject moddedObject = m_SettingInfoDisplaysContainer.InstantiateEntry();
            UIElementSettingsMenuSettingInfoDisplay settingDisplay = moddedObject.gameObject.AddComponent<UIElementSettingsMenuSettingInfoDisplay>();
            settingDisplay.settingInfo = settingInfo;
            return settingDisplay;
        }
    }
}
