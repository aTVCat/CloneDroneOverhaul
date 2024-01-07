using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingsMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        public Button LegacyUIButton;

        [UIElement("Content")]
        public Transform PageContentsTransform;

        [UIElement("Header1Prefab", false)]
        public ModdedObject Header1Prefab;
        [UIElement("Header2Prefab", false)]
        public ModdedObject Header2Prefab;
        [UIElement("Header3Prefab", false)]
        public ModdedObject Header3Prefab;

        [UIElement("DropdownPrefab", false)]
        public Dropdown DropdownPrefab;
        [UIElement("DropdownImagePrefab", false)]
        public Dropdown DropdownWithImagePrefab;

        [UIElement("SliderPrefab", false)]
        public Slider SliderPrefab;

        [UIElement("TogglePrefab", false)]
        public ModdedObject TogglePrefab;

        [TabManager(typeof(UIElementSettingsTab), nameof(m_tabPrefab), nameof(m_tabContainer), nameof(OnTabCreated), nameof(OnTabSelected), new string[] { "Test", "Gameplay", "Sounds", "Multiplayer", "Mod-Bot" })]
        private TabManager m_tabs;
        [UIElement("TabPrefab", false)]
        private ModdedObject m_tabPrefab;
        [UIElement("TabsContainer")]
        private Transform m_tabContainer;

        protected override void OnInitialized()
        {
            m_tabs.SelectTab("Gameplay");
        }

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            PopulatePage(elementTab.tabId);
        }

        public void OnTabCreated(UIElementTab elementTab)
        {
        }

        public void ClearPageContents()
        {
            if (PageContentsTransform && PageContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(PageContentsTransform);
        }

        public void PopulatePage(string id)
        {
            ClearPageContents();

            SettingsMenu settingsMenu = GameUIRoot.Instance?.SettingsMenu;
            if (settingsMenu)
                settingsMenu.populateSettings();
            else
                return;

            switch (id)
            {
                case "Test":
                    populateTestPage(settingsMenu);
                    break;
                case "Gameplay":
                    populateGameplayPage(settingsMenu);
                    break;
                default:
                    populateDefaultPage(settingsMenu);
                    break;
            }
        }

        private void populateTestPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Graphics test");
                _ = pageBuilder.Header3("Window settings");
                _ = pageBuilder.Dropdown(settingsMenu.ScreenResolutionDropDown.options, settingsMenu.ScreenResolutionDropDown.value, OnScreenResolutionChanged);
                _ = pageBuilder.Toggle(settingsMenu.FullScreenToggle.isOn, OnFullScreenChanged, "Fullscreen");
                _ = pageBuilder.Toggle(settingsMenu.VsyncOnToggle.isOn, OnVSyncChanged, "V-Sync");

                _ = pageBuilder.Header1("Multiplayer test");
                _ = pageBuilder.Header3("Preferred region");
                _ = pageBuilder.Dropdown(settingsMenu.RegionDropdown.options, settingsMenu.RegionDropdown.value, OnRegionChanged);
                _ = pageBuilder.Header3("Player skin");
                _ = pageBuilder.DropdownWithImage(settingsMenu.MultiplayerCharacterModelDropdown.options, settingsMenu.MultiplayerCharacterModelDropdown.value, OnCharacterModelChanged);
            }
        }

        private void populateGameplayPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Gameplay settings");
                _ = pageBuilder.Header3("Difficulty");
                _ = pageBuilder.Dropdown(settingsMenu.StoryModeDifficultyDropDown.options, settingsMenu.StoryModeDifficultyDropDown.value, OnStoryDifficultyIndexChanged);
            }
        }

        private void populateDefaultPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Page not implemented.");
                _ = pageBuilder.Header2("This might get fixed in future");
            }
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                Hide();
                titleScreenUI.OnOptionsButtonClicked();
            }
        }

        public void OnScreenResolutionChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.ScreenResolutionDropDown.value = value;
            }
        }

        public void OnFullScreenChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.FullScreenToggle.isOn = value;
            }
        }

        public void OnVSyncChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.VsyncOnToggle.isOn = value;
            }
        }

        public void OnRegionChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.RegionDropdown.value = value;
            }
        }

        public void OnCharacterModelChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.MultiplayerCharacterModelDropdown.value = value;
            }
        }

        public void OnStoryDifficultyIndexChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.StoryModeDifficultyDropDown.value = value;
            }
        }

        public class PageBuilder : IDisposable
        {
            public UISettingsMenuRework SettingsMenu;

            public PageBuilder(UISettingsMenuRework settingsMenu)
            {
                SettingsMenu = settingsMenu;
            }

            private void addLocalizedTextField(Text text, string localizationId)
            {
                if (localizationId == null)
                    localizationId = string.Empty;

                if (!string.IsNullOrEmpty(localizationId))
                {
                    LocalizedTextField localizedTextField = text.gameObject.AddComponent<LocalizedTextField>();
                    localizedTextField.LocalizationID = localizationId;
                }
            }

            private Text instantiateHeader(string text, string localizationId, ModdedObject prefab)
            {
                ModdedObject moddedObject = Instantiate(prefab, SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                Text textComponent = moddedObject.GetObject<Text>(0);
                textComponent.text = text;
                addLocalizedTextField(textComponent, localizationId);
                return textComponent;
            }

            private Dropdown instantiateDropdown(List<Dropdown.OptionData> list, int value, UnityAction<int> callback, Dropdown prefab)
            {
                if (list == null)
                    list = new List<Dropdown.OptionData>();

                Dropdown dropdown = Instantiate(prefab, SettingsMenu.PageContentsTransform);
                dropdown.gameObject.SetActive(true);
                dropdown.options = list;
                dropdown.value = value;
                if (callback != null)
                    dropdown.onValueChanged.AddListener(callback);
                return dropdown;
            }

            public Text Header1(string text, string localizationId = null)
            {
                return instantiateHeader(text, localizationId, SettingsMenu.Header1Prefab);
            }

            public Text Header2(string text, string localizationId = null)
            {
                return instantiateHeader(text, localizationId, SettingsMenu.Header2Prefab);
            }

            public Text Header3(string text, string localizationId = null)
            {
                return instantiateHeader(text, localizationId, SettingsMenu.Header3Prefab);
            }

            public Dropdown Dropdown(List<Dropdown.OptionData> list, int value, UnityAction<int> callback)
            {
                return instantiateDropdown(list, value, callback, SettingsMenu.DropdownPrefab);
            }

            public Dropdown DropdownWithImage(List<Dropdown.OptionData> list, int value, UnityAction<int> callback)
            {
                return instantiateDropdown(list, value, callback, SettingsMenu.DropdownWithImagePrefab);
            }

            public Slider Slider(float min, float max, bool wholeNumbers, float value, UnityAction<float> callback)
            {
                Slider slider = Instantiate(SettingsMenu.SliderPrefab, SettingsMenu.PageContentsTransform);
                slider.gameObject.SetActive(true);
                slider.minValue = min;
                slider.maxValue = max;
                slider.wholeNumbers = wholeNumbers;
                slider.value = value;
                if (callback != null)
                    slider.onValueChanged.AddListener(callback);
                return slider;
            }

            public Toggle Toggle(bool isOn, UnityAction<bool> callback, string text, string localizationId = null)
            {
                ModdedObject moddedObject = Instantiate(SettingsMenu.TogglePrefab, SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                Text textComponent = moddedObject.GetObject<Text>(1);
                textComponent.text = text;
                addLocalizedTextField(textComponent, localizationId);
                Toggle toggle = moddedObject.GetObject<Toggle>(0);
                toggle.isOn = isOn;
                if (callback != null)
                    toggle.onValueChanged.AddListener(callback);
                return toggle;
            }

            public void Dispose()
            {
                SettingsMenu = null;
            }
        }
    }
}
