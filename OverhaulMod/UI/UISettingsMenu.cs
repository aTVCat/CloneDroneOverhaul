using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private Button m_CloseButton;

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

        protected override void OnInitialized()
        {
        }

        public override void Show()
        {
            base.Show();
            PopulatePage();
            SetTitleScreenButtonActive(false);
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public void ClearPageContents()
        {
            if (PageContentsTransform && PageContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(PageContentsTransform);
        }

        public void PopulatePage()
        {
            SettingsMenu settingsMenu = GameUIRoot.Instance?.SettingsMenu;
            if (settingsMenu)
                settingsMenu.populateSettings();
            else
                return;

            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Test 1");
                _ = pageBuilder.Header2("Test 2");
                _ = pageBuilder.Header3("Test 3");
                _ = pageBuilder.Slider(0f, 55f, false, 15f, null);
                _ = pageBuilder.Slider(15f, 30f, true, 10f, null);
                _ = pageBuilder.Toggle(false, null, "A toggle [off]");
                _ = pageBuilder.Toggle(true, null, "A toggle [on]");
                _ = pageBuilder.Dropdown(settingsMenu.RegionDropdown.options, 3, null);
                _ = pageBuilder.DropdownWithImage(settingsMenu.MultiplayerCharacterModelDropdown.options, 3, null);
            }
        }

        public class PageBuilder : IDisposable
        {
            public UISettingsMenu SettingsMenu;

            public PageBuilder(UISettingsMenu settingsMenu)
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
