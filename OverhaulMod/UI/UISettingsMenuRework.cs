using OverhaulMod.Engine;
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
        private readonly TabManager m_tabs;
        [UIElement("TabPrefab", false)]
        private readonly ModdedObject m_tabPrefab;
        [UIElement("TabsContainer")]
        private readonly Transform m_tabContainer;

        public override bool hideTitleScreen => true;

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
                _ = pageBuilder.Dropdown(settingsMenu.QualityDropDown.options, settingsMenu.QualityDropDown.value, OnQualityDropdownChanged);
                _ = pageBuilder.Dropdown(settingsMenu.AntiAliasingDropdown.options, settingsMenu.AntiAliasingDropdown.value, OnAntiAliasingDropdownChanged);
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
                _ = pageBuilder.Button("Configure Overhaul UIs", delegate
                {
                    ModUIConstants.ShowOverhaulUIManagementPanel(base.transform);
                });

                _ = pageBuilder.Header1("Garbage");
                _ = pageBuilder.Dropdown(settingsMenu.GarbageSettingsDropdown.options, settingsMenu.GarbageSettingsDropdown.value, OnGarbageSettingsChanged);
                _ = pageBuilder.Toggle(settingsMenu.PlayerPushesGarbageToggle.isOn, OnPlayerPushesGarbageToggleChanged, "Collisions");

                _ = pageBuilder.Header1("Environment");
                _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingConstants.ENABLE_ARENA_REMODEL), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(ModSettingConstants.ENABLE_ARENA_REMODEL, value, true);
                }, "Arena remodel");
                _ = pageBuilder.Header4("Made by @water2977");

                _ = pageBuilder.Header1("Voxels");
                _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingConstants.ENABLE_FADING), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(ModSettingConstants.ENABLE_FADING, value, true);
                }, "Better fire spreading");
                _ = pageBuilder.Toggle(true, delegate (bool value)
                {
                }, "Force burn");
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
                _ = pageBuilder.Toggle(settingsMenu.RelayToggle.isOn, OnRelayToggleChanged, "Relay connection");
                _ = pageBuilder.Button("Manage muted players", delegate
                {
                    GameUIRoot.Instance.BlockListSettingsUI.Show();
                    ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                    {
                        GameUIRoot.Instance.BlockListSettingsUI.Hide();
                    });
                });

                _ = pageBuilder.Header1("Player");
                _ = pageBuilder.Header3("Skin");
                _ = pageBuilder.DropdownWithImage(settingsMenu.MultiplayerCharacterModelDropdown.options, settingsMenu.MultiplayerCharacterModelDropdown.value, OnCharacterModelChanged);
                _ = pageBuilder.DropdownWithImage(settingsMenu.MultiplayerFavoriteColorDropdown.options, settingsMenu.MultiplayerFavoriteColorDropdown.value, OnMultiplayerFavoriteColorDropdownChanged);
                _ = pageBuilder.Toggle(settingsMenu.UseSkinInSinglePlayer.isOn, OnUseSkinInSinglePlayerToggleChanged, "Use skin in singleplayer");
                _ = pageBuilder.Button("Select emotes", delegate
                {
                    GameUIRoot.Instance.EmoteSettingsUI.Show();
                    ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                    {
                        GameUIRoot.Instance.EmoteSettingsUI.Hide();
                    });
                });
                _ = pageBuilder.Button("Personalize", null);
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
                        _ = GameUIRoot.Instance.ControlMapper.Close(true);
                    });
                });

                _ = pageBuilder.Header3("Mouse");
                _ = pageBuilder.Slider(settingsMenu.MouseSensitivitySlider.minValue, settingsMenu.MouseSensitivitySlider.maxValue, false, settingsMenu.MouseSensitivitySlider.value, OnMouseSensitivityChanged);
                _ = pageBuilder.Toggle(settingsMenu.InvertMouseToggle.isOn, OnInvertMouseToggleChanged, "Invert mouse");
                _ = pageBuilder.Header3("Controller");
                _ = pageBuilder.Slider(settingsMenu.ControllerSensitivitySlider.minValue, settingsMenu.ControllerSensitivitySlider.maxValue, false, settingsMenu.ControllerSensitivitySlider.value, OnControllerSensitivityChanged);
                _ = pageBuilder.Toggle(settingsMenu.InvertControllerToggle.isOn, OnInvertControllerToggleChanged, "Invert controller");
                _ = pageBuilder.Toggle(settingsMenu.EqualLookRatioToggle.isOn, OnEqualLookRatioToggleChanged, "1:1 look ratio");
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

        public void OnQualityDropdownChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.QualityDropDown.value = value;
            }
        }

        public void OnAntiAliasingDropdownChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.AntiAliasingDropdown.value = value;
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

        public void OnMultiplayerFavoriteColorDropdownChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.MultiplayerFavoriteColorDropdown.value = value;
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
                settingsMenu.PlayerPushesGarbageToggle.isOn = value;
            }
        }

        public void OnSubtitlesToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.SubtitlesToggle.isOn = value;
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

        public void OnMouseSensitivityChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.MouseSensitivitySlider.value = value;
            }
        }

        public void OnInvertMouseToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.InvertMouseToggle.isOn = value;
            }
        }

        public void OnControllerSensitivityChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.ControllerSensitivitySlider.value = value;
            }
        }

        public void OnInvertControllerToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.InvertControllerToggle.isOn = value;
            }
        }

        public void OnEqualLookRatioToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.EqualLookRatioToggle.isOn = value;
            }
        }

        public void OnUseSkinInSinglePlayerToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.UseSkinInSinglePlayer.isOn = value;
            }
        }

        public void OnRelayToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
            {
                settingsMenu.RelayToggle.isOn = value;
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
                    callback = delegate { ModUIUtils.MessagePopupNotImplemented(); };

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

                _ = slider.gameObject.AddComponent<BetterSliderCallback>();

                return slider;
            }

            public Toggle Toggle(bool isOn, UnityAction<bool> callback, string text, string localizationId = null)
            {
                if (callback == null)
                    callback = delegate { ModUIUtils.MessagePopupNotImplemented(); };

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
                    onClicked = ModUIUtils.MessagePopupNotImplemented;

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
