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
        [UIElement("Header4Prefab", false)]
        public ModdedObject Header4Prefab;

        [UIElement("DropdownPrefab", false)]
        public Dropdown DropdownPrefab;
        [UIElement("DropdownImagePrefab", false)]
        public Dropdown DropdownWithImagePrefab;

        [UIElement("SliderPrefab", false)]
        public Slider SliderPrefab;

        [UIElement("TogglePrefab", false)]
        public ModdedObject TogglePrefab;

        [UIElement("ButtonPrefab", false)]
        public ModdedObject ButtonPrefab;

        [TabManager(typeof(UIElementSettingsTab), nameof(m_tabPrefab), nameof(m_tabContainer), nameof(OnTabCreated), nameof(OnTabSelected), new string[] { "Gameplay", "Graphics", "Sounds", "Controls", "Multiplayer", "Language", "Mod-Bot" })]
        private TabManager m_tabs;
        [UIElement("TabPrefab", false)]
        private ModdedObject m_tabPrefab;
        [UIElement("TabsContainer")]
        private Transform m_tabContainer;

        protected override void OnInitialized()
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
                settingsMenu.populateSettings();

            if (ModBuildInfo.debug)
            {
                m_tabs.AddTab("Debug");
            }
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

            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (!settingsMenu)
                return;

            switch (id)
            {
                case "Debug":
                    populateDebugPage(settingsMenu);
                    break;
                case "Gameplay":
                    populateGameplayPage(settingsMenu);
                    break;
                case "Graphics":
                    populateGraphicsPage(settingsMenu);
                    break;
                case "Sounds":
                    populateSoundsPage(settingsMenu);
                    break;
                case "Controls":
                    populateControlsPage(settingsMenu);
                    break;
                case "Multiplayer":
                    populateMultiplayerPage(settingsMenu);
                    break;
                default:
                    populateDefaultPage(settingsMenu);
                    break;
            }
        }

        private void populateDebugPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Slider(0f, 1f, false, 0.5f, null);

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

        private void populateGraphicsPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Graphics");
                _ = pageBuilder.Header3("Window");
                _ = pageBuilder.Dropdown(settingsMenu.ScreenResolutionDropDown.options, settingsMenu.ScreenResolutionDropDown.value, OnScreenResolutionChanged);
                _ = pageBuilder.Toggle(settingsMenu.FullScreenToggle.isOn, OnFullScreenChanged, "Fullscreen");
                _ = pageBuilder.Toggle(settingsMenu.VsyncOnToggle.isOn, OnVSyncChanged, "V-Sync");

                _ = pageBuilder.Header3("Title bar");
                _ = pageBuilder.Toggle(true, null, "Dark mode");
                _ = pageBuilder.Toggle(true, null, "Custom text");
                _ = pageBuilder.Header4("Works on Windows 10 v1809 or above");
                _ = pageBuilder.Header4("todo: implement custom settings");

                _ = pageBuilder.Header1("User interface");
                _ = pageBuilder.Toggle(!settingsMenu.HideGameUIToggle.isOn, OnHideGameUIToggleChanged, "Show game UI");
                _ = pageBuilder.Toggle(settingsMenu.SubtitlesToggle.isOn, OnSubtitlesToggleChanged, "Show subtitles");
                _ = pageBuilder.Toggle(true, null, "Show watermark");
                _ = pageBuilder.Button("Configure Overhaul UIs", null);

                _ = pageBuilder.Header1("Garbage");
                _ = pageBuilder.Dropdown(settingsMenu.GarbageSettingsDropdown.options, settingsMenu.GarbageSettingsDropdown.value, OnGarbageSettingsChanged);
                _ = pageBuilder.Toggle(settingsMenu.PlayerPushesGarbageToggle.isOn, OnPlayerPushesGarbageToggleChanged, "Collisions");
            }
        }

        private void populateGameplayPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Gameplay settings");
                _ = pageBuilder.Header3("Difficulty");
                _ = pageBuilder.Dropdown(settingsMenu.StoryModeDifficultyDropDown.options, settingsMenu.StoryModeDifficultyDropDown.value, OnStoryDifficultyIndexChanged);
                _ = pageBuilder.Header4("Change what enemies spawn");

                _ = pageBuilder.Header3("Endless levels");
                _ = pageBuilder.Dropdown(settingsMenu.WorkshopLevelPolicyDropdown.options, settingsMenu.WorkshopLevelPolicyDropdown.value, OnWorkshopEndlessLevelPolicyIndexChanged);
                _ = pageBuilder.Button("Get more levels", delegate
                {
                    Hide();
                    if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserRework))
                        ModCache.titleScreenUI.OnWorkshopBrowserButtonClicked();
                    else
                        ModUIConstants.ShowWorkshopBrowserRework();
                });

                _ = pageBuilder.Header1("Twitch");
                _ = pageBuilder.Button("Enemy spawn settings", delegate
                {
                    settingsMenu.OnTwitchEnemyLimitButtonClicked();
                    ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                    {
                        GameUIRoot.Instance.TwitchEnemySettingsMenu.Hide();
                    });
                });
                _ = pageBuilder.Toggle(settingsMenu.MuteEmotesToggle.isOn, OnMuteEmotesToggleChanged, "Mute twitch emotes");
                _ = pageBuilder.Toggle(settingsMenu.DevIsLiveEnabledToggle.isOn, OnDevIsLiveToggleChanged, "Dev stream notifications");
            }
        }

        private void populateSoundsPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Volume");
                _ = pageBuilder.Header3("Global");
                _ = pageBuilder.Slider(0f, 1f, false, settingsMenu.SoundVolume.value, OnGlobalVolumeChanged);
                _ = pageBuilder.Header3("Music");
                _ = pageBuilder.Slider(0f, 1f, false, settingsMenu.MusicVolume.value, OnMusicVolumeChanged);
                _ = pageBuilder.Header3("Commentator");
                _ = pageBuilder.Slider(0f, 1f, false, settingsMenu.CommentatorsVolume.value, OnCommentatorVolumeChanged);

                _ = pageBuilder.Header1("Effects");
                _ = pageBuilder.Toggle(true, null, "Enable reverb");
                _ = pageBuilder.Header3("Reverb intensity");
                _ = pageBuilder.Slider(0f, 1f, false, 0.5f, null);
            }
        }

        private void populateMultiplayerPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Multiplayer settings");
                _ = pageBuilder.Header3("Preferred region");
                _ = pageBuilder.Dropdown(settingsMenu.RegionDropdown.options, settingsMenu.RegionDropdown.value, OnRegionChanged);
                _ = pageBuilder.Header3("Player skin");
                _ = pageBuilder.DropdownWithImage(settingsMenu.MultiplayerCharacterModelDropdown.options, settingsMenu.MultiplayerCharacterModelDropdown.value, OnCharacterModelChanged);
            }
        }

        private void populateControlsPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Controls settings");
                _ = pageBuilder.Button("Edit controls", delegate
                {
                    GameUIRoot.Instance.ControlMapper.Open();
                    ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                    {
                        GameUIRoot.Instance.ControlMapper.Close(true);
                    });
                });
            }
        }

        private void populateDefaultPage(SettingsMenu settingsMenu)
        {
            using (PageBuilder pageBuilder = new PageBuilder(this))
            {
                _ = pageBuilder.Header1("Page not implemented.");
                _ = pageBuilder.Header2("Try using original menu");
                _ = pageBuilder.Button("Open original settings menu", OnLegacyUIButtonClicked);
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

        public void OnWorkshopEndlessLevelPolicyIndexChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.WorkshopLevelPolicyDropdown.value = value;
            }
        }

        public void OnDevIsLiveToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.DevIsLiveEnabledToggle.isOn = value;
            }
        }

        public void OnMuteEmotesToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.MuteEmotesToggle.isOn = value;
            }
        }

        public void OnHideGameUIToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.HideGameUIToggle.isOn = !value;
            }
        }

        public void OnGarbageSettingsChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.GarbageSettingsDropdown.value = value;
            }
        }

        public void OnPlayerPushesGarbageToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.PlayerPushesGarbageToggle.isOn = !value;
            }
        }

        public void OnSubtitlesToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.SubtitlesToggle.isOn = !value;
            }
        }

        public void OnGlobalVolumeChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.SoundVolume.value = value;
            }
        }

        public void OnMusicVolumeChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.MusicVolume.value = value;
            }
        }

        public void OnCommentatorVolumeChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.CommentatorsVolume.value = value;
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
                if (callback == null)
                    callback = delegate { ModUIUtility.MessagePopupNotImplemented(); };

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

            public Text Header4(string text, string localizationId = null)
            {
                return instantiateHeader(text, localizationId, SettingsMenu.Header4Prefab);
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

                slider.gameObject.AddComponent<BetterSliderCallback>();

                return slider;
            }

            public Toggle Toggle(bool isOn, UnityAction<bool> callback, string text, string localizationId = null)
            {
                if (callback == null)
                    callback = delegate { ModUIUtility.MessagePopupNotImplemented(); };

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

            public Button Button(string text, Action onClicked)
            {
                if (onClicked == null)
                    onClicked = ModUIUtility.MessagePopupNotImplemented;

                ModdedObject moddedObject = Instantiate(SettingsMenu.ButtonPrefab, SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = text;
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(new UnityAction(onClicked));
                return button;
            }

            public void Dispose()
            {
                SettingsMenu = null;
            }
        }
    }
}
