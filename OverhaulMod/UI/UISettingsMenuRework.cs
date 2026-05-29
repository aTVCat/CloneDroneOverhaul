using AmplifyOcclusion;
using InternalModBot;
using ModBotWebsiteAPI;
using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Patches.Behaviours;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingsMenuRework : OverhaulUIBehaviour
    {
        private static readonly string[] s_pages = new string[] { "setup", "Gameplay", "Interface", "Graphics", "Effects", "Sounds", "Controls", "Multiplayer", "Languages", "Advanced" };

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        public Button LegacyUIButton;

        [UIElementAction(nameof(OnImportSettingsButtonClicked))]
        [UIElement("ImportSettingsButton")]
        private Button _importSettingsButton;

        [UIElementAction(nameof(OnExportSettingsButtonClicked))]
        [UIElement("ExportSettingsButton")]
        private Button _exportSettingsButton;

        [UIElement("Shadow")]
        private GameObject _shadow;

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
        [UIElement("DropdownImage169Prefab", false)]
        public Dropdown DropdownWithImage169Prefab;
        [UIElement("DropdownWithText", false)]
        public ModdedObject DropdownWithTextPrefab;

        [UIElement("SliderPrefab", false)]
        public Slider SliderPrefab;

        [UIElement("TogglePrefab", false)]
        public ModdedObject TogglePrefab;
        [UIElement("ToggleWithOptionsPrefab", false)]
        public ModdedObject ToggleWithOptionsPrefab;

        [UIElement("ButtonPrefab", false)]
        public ModdedObject ButtonPrefab;

        [UIElement("KeyBindPrefab", false)]
        public ModdedObject KeyBindPrefab;

        [UIElement("GridContainer", false)]
        public GameObject GridContainer;

        [UIElement("LanguageButtonPrefab", false)]
        public ModdedObject LanguageButton;

        [UIElement("AddonDownload", false)]
        public ModdedObject AddonDownload;

        [TabManager(typeof(UIElementSettingsMenuCategoryTab), nameof(_tabPrefab), nameof(_tabContainer), nameof(OnTabCreated), nameof(OnTabSelected), new string[] { "Gameplay", "Interface", "Graphics", "Effects", "Sounds", "Controls", "Multiplayer", "Languages", "Advanced" })]
        private readonly TabManager _tabs;
        [UIElement("TabPrefab", false)]
        private readonly ModdedObject _tabPrefab;
        [UIElement("TabsContainer")]
        private readonly Transform _tabContainer;

        [UIElement("PanelNew")]
        private readonly RectTransform _panelTransform;

        [UIElement("Shading")]
        private readonly GameObject _shadingObject;

        [UIElement("BG")]
        private readonly GameObject _normalBgObject;

        [UIElement("BGSetup")]
        private readonly GameObject _setupBgObject;

        [UIElement("SettingDescriptionBox", typeof(UIElementSettingsMenuSettingDescriptionBox))]
        private readonly UIElementSettingsMenuSettingDescriptionBox _descriptionBox;

        private bool _hasSelectedTab;

        private bool _hasMultiplayerCustomizationChanges;

        private string _selectedTabId;

        private string _initialPage;

        public override bool HideTitleScreen => true;

        public bool DisallowUsingKey
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu)
                settingsMenu.populateSettings();
        }

        public override void Show()
        {
            base.Show();

            SubtitleTextFieldPatchBehaviour subtitleTextFieldPatchBehaviour = GamePatchBehaviour.GetBehaviour<SubtitleTextFieldPatchBehaviour>();
            if (subtitleTextFieldPatchBehaviour) subtitleTextFieldPatchBehaviour.SetSiblingIndex(base.transform);

            if (!_selectedTabId.IsNullOrEmpty())
                PopulatePage(_selectedTabId);

            _descriptionBox.Hide();
        }

        public override void Hide()
        {
            base.Hide();

            SubtitleTextFieldPatchBehaviour subtitleTextFieldPatchBehaviour = GamePatchBehaviour.GetBehaviour<SubtitleTextFieldPatchBehaviour>();
            if (subtitleTextFieldPatchBehaviour) subtitleTextFieldPatchBehaviour.ResetSiblingIndex();

            ModSettingsDataManager.Instance.Save();
            if (_hasMultiplayerCustomizationChanges && BoltNetwork.IsRunning && !BoltNetwork.IsServer && !BoltNetwork.IsSinglePlayer)
            {
                MultiplayerMatchManager.Instance.SendClientCharacterCustomizationEvent();
                _hasMultiplayerCustomizationChanges = false;
            }
        }

        public string GetSelectedTabID()
        {
            return _selectedTabId;
        }

        public void ShowDescriptionBox(string settingIdOfDescription, RectTransform element)
        {
            string settingId = StringUtils.AddSpacesToCamelCasedString(settingIdOfDescription).ToLower().Replace(" ", "_");

            _descriptionBox.SetYPosition(element.position.y);
            _descriptionBox.SetText(LocalizationManager.Instance.GetTranslatedString($"sd_{settingId}"), ModSettingsManager.Instance.GetSubDescription(settingId));
            _descriptionBox.Show();
        }

        public void HideDescription()
        {
            _descriptionBox.Hide();
        }

        public void ShowRegularElements()
        {
            DisallowUsingKey = false;
            _panelTransform.anchorMax = new Vector2(1f, 1f);
            _panelTransform.anchorMin = new Vector2(0f, 0f);
            _panelTransform.sizeDelta = new Vector2(0f, 0f);
            _shadingObject.SetActive(true);
            _normalBgObject.SetActive(true);
            _setupBgObject.SetActive(false);
            _shadow.SetActive(true);

            if (!_hasSelectedTab)
            {
                _tabs.SelectTab("Gameplay");
                _hasSelectedTab = true;
            }
        }

        public void ShowSetupElements()
        {
            DisallowUsingKey = true;
            _panelTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _panelTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _panelTransform.sizeDelta = new Vector2(360f, 500f);
            _shadingObject.SetActive(false);
            _normalBgObject.SetActive(false);
            _setupBgObject.SetActive(true);
            _shadow.SetActive(false);
            PopulatePage("setup");
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            PopulatePage(elementTab.tabId);
        }

        public void OnTabCreated(UIElementTab elementTab)
        {
            UIElementSettingsMenuCategoryTab elementTabWithText = elementTab as UIElementSettingsMenuCategoryTab;
            elementTabWithText.LocalizationID = $"settings_tab_{elementTab.tabId.ToLower()}";
        }

        public void ClearPageContents()
        {
            if (PageContentsTransform && PageContentsTransform.childCount > 0)
                TransformUtils.DestroyAllChildren(PageContentsTransform);

            _descriptionBox.Hide();
        }

        public void PopulatePageIfSelected(string id)
        {
            if (_selectedTabId != id)
                return;

            PopulatePage(id);
        }

        public void PopulatePage(string id)
        {
            string initialPage;
            if (!s_pages.Contains(id)) // if the page we want to switch to is sub page,
            {
                if (s_pages.Contains(_selectedTabId)) // save current page id, if current page is one of main pages
                {
                    initialPage = _selectedTabId;
                    _initialPage = initialPage;
                }
                else // use saved page id, if current page is sub page
                {
                    initialPage = _initialPage;
                }
            }
            else
            {
                initialPage = string.Empty;
            }

            _selectedTabId = id;
            ClearPageContents();

            UIElementTab oldTab = _tabs.PreviousSelectedTab;
            UIElementTab newTab = _tabs.SelectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (!settingsMenu)
                return;

            switch (id)
            {
                case "setup":
                    populateSetupPage(settingsMenu);
                    break;
                case "Gameplay":
                    populateGameplayPage(settingsMenu);
                    break;
                case "Interface":
                    populateInterfacePage(settingsMenu);
                    break;
                case "Graphics":
                    populateGraphicsPage(settingsMenu);
                    break;
                case "Effects":
                    populateEffectsPage(settingsMenu);
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
                case "Advanced":
                    populateAdvancedPage(settingsMenu);
                    break;
                case "Languages":
                    populateLanguagesPage(settingsMenu);
                    break;

                case "SSAO":
                    populateSSAOSettingsPage(initialPage);
                    break;
                case "CA":
                    populateCASettingsPage(initialPage);
                    break;
                case "UKTD":
                    populateUKTDReworkSettingsPage(initialPage);
                    break;
                case "SubtitlesRework":
                    populateSubtitlesReworkSettingsPage(initialPage);
                    break;
                case "MuteSound":
                    populateMuteSoundPage(initialPage);
                    break;
                case "Graphics2":
                    populateAdditionalGraphicsPage(settingsMenu);
                    break;

                default:
                    populateDefaultPage(settingsMenu);
                    break;
            }
        }

        private void populateSetupPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);

            _ = pageBuilder.Header1("Graphics");
            _ = pageBuilder.Header3("Post effects");
            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.ENABLE_SSAO, "Ambient occlusion", "SSAO");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_SSAO);

            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, "Chromatic aberration", "CA");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION);

            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_DITHERING, "Dithering");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_DITHERING);

            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_VIGNETTE, "Vignette");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_VIGNETTE);

            _ = pageBuilder.DropdownWithText(PostEffectsManager.BloomOptions, ModSettingIDs.BLOOM_MODE, "Bloom");

            _ = pageBuilder.Header3("Particle effects");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_GARBAGE_PARTICLES, "Enable sparks");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_HIT_PARTICLES, "Enable particles", "setup");
            if (ModSettingsManager.GetBoolValue(ModSettingIDs.ENABLE_HIT_PARTICLES))
            {
                _ = pageBuilder.Toggle(ModSettingIDs.REDUCE_FLASHES, "Reduce flashes");
                _ = pageBuilder.Toggle(ModSettingIDs.NEW_EXPLOSION_PARTICLES, "New explosion particles");
            }

            _ = pageBuilder.Header3("Voxel engine");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_VOXEL_FIRE_FADING, "Better fire spreading");
            _ = pageBuilder.Toggle(ModSettingIDs.CHANGE_HIT_COLORS, "Better damage colors");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_VOXEL_BURNING, "Always burn voxels");

            _ = pageBuilder.Header3("Camera");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_CAMERA_ROLLING, "Camera rolling");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_FIRST_PERSON_MODE, "First person mode");
            _ = pageBuilder.KeyBind(ModSettingIDs.CAMERA_MODE_TOGGLE_KEYBIND, "Toggle camera mode", KeyCode.Y);
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_CAMERA_BOBBING, "Camera bobbing");

            _ = pageBuilder.Header1("Game interface");
            _ = pageBuilder.Header3("Cursor skin");
            _ = pageBuilder.DropdownWithImage(ModConstants.CursorSkinOptions, ModSettingIDs.CURSOR_SKIN);

            _ = pageBuilder.Toggle(ModSettingIDs.SHOW_VERSION_LABEL, "Show Overhaul mod version");
            _ = pageBuilder.Button("Configure Overhaul mod UIs", delegate
            {
                _ = ModUIs.ShowOverhaulUIManagementPanel(base.transform);
            });

            _ = pageBuilder.Button("Done", delegate
            {
                ModSettingsManager.SetBoolValue(ModSettingIDs.SHOW_MOD_SETUP_SCREEN_ON_START, false);
                UITitleScreenRework titleScreenCustomizationPanel = ModUIManager.Instance?.Get<UITitleScreenRework>(ModAssetBundles.UI, ModUIs.UI_TITLE_SCREEN_REWORK);
                if (titleScreenCustomizationPanel)
                {
                    titleScreenCustomizationPanel.SetSkinAccordingToSettings();
                }

                ModUIUtils.ShowNewUpdateMessageOrChangelog(1f, false);

                Hide();
            });
        }

        private void populateInterfacePage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Game interface");
            _ = pageBuilder.Header3("Cursor skin");

            List<Dropdown.OptionData> cursorSkinOptions = ModConstants.CursorSkinOptions;
            cursorSkinOptions[0].text = LocalizationManager.Instance.GetTranslatedString("settings_option_default");
            cursorSkinOptions[1].text = LocalizationManager.Instance.GetTranslatedString("settings_option_skin_1");
            cursorSkinOptions[2].text = LocalizationManager.Instance.GetTranslatedString("settings_option_skin_2");

            _ = pageBuilder.DropdownWithImage(cursorSkinOptions, ModSettingIDs.CURSOR_SKIN);

            _ = pageBuilder.Button("Configure Overhaul mod UIs", delegate
            {
                _ = ModUIs.ShowOverhaulUIManagementPanel(base.transform);
            });
            _ = pageBuilder.Toggle(ModSettingIDs.SHOW_VERSION_LABEL, "Show Overhaul mod version");
            _ = pageBuilder.Toggle(ModSettingIDs.UI_SOUNDS, "Interface sounds");

            GameObject hideGameUIToggleNote = null;
            _ = pageBuilder.Toggle(!settingsMenu.HideGameUIToggle.isOn, delegate (bool value)
            {
                hideGameUIToggleNote.SetActive(value && CutSceneManager.Instance.IsInCutscene());
                OnHideGameUIToggleChanged(value);
            }, "Show game UI");
            hideGameUIToggleNote = pageBuilder.Header4("You're in cutscene mode, UI will be still hidden.".AddColor(Color.yellow)).transform.parent.gameObject;
            hideGameUIToggleNote.SetActive(!settingsMenu.HideGameUIToggle.isOn && CutSceneManager.Instance.IsInCutscene());


            _ = pageBuilder.Header1("Subtitles");
            _ = pageBuilder.Toggle(settingsMenu.SubtitlesToggle.isOn, OnSubtitlesToggleChanged, "Show subtitles");
            _ = pageBuilder.Toggle(ModSettingIDs.SHOW_SPEAKER_NAME, "Display who's speaking", delegate
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });
            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.ENABLE_SUBTITLE_TEXT_FIELD_REWORK, "Commentator subtitles rework", "SubtitlesRework", delegate (bool value)
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });


            _ = pageBuilder.Header1("Labels");
            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.ENABLE_PRESS_BUTTON_TRIGGER_DESCRIPTION_REWORK, "Use key trigger description rework", "UKTD", delegate (bool value)
            {
                if (value) UseKeyTriggerManager.Instance.ShowThenHideDescription(ModConstants.LoremIpsumText, 2f);
            });


            _ = pageBuilder.Header1("Energy bar enhancements");
            _ = pageBuilder.Toggle(ModSettingIDs.ENERGY_UI_FADE_OUT_IF_FULL, "Fade out energy bar if full", "Interface");
            if (EnergyBarBehaviour.EnableBehaviour)
            {
                _ = pageBuilder.Header3("Fade out intensity");
                _ = pageBuilder.SliderFloat(0.1f, 1f, ModSettingIDs.ENERGY_UI_FADE_OUT_INTENSITY, true);
            }
            _ = pageBuilder.Button("Reset energy bar settings", delegate
            {
                ModSettingsManager.ResetValue(ModSettingIDs.ENERGY_UI_REWORK, true);
                ModSettingsManager.ResetValue(ModSettingIDs.ENERGY_UI_FADE_OUT_IF_FULL, true);
                ModSettingsManager.ResetValue(ModSettingIDs.ENERGY_UI_FADE_OUT_INTENSITY, true);
                PopulatePage("Interface");
            });


            _ = pageBuilder.Header1("Photo mode");
            _ = pageBuilder.Toggle(ModSettingIDs.ADVANCED_PHOTO_MODE, "Advanced photo mode", "Interface");
            if (AdvancedPhotoModeManager.EnableAdvancedPhotoMode)
            {
                _ = pageBuilder.Toggle(ModSettingIDs.REQUIRE_RMB_HOLD_WHEN_UI_IS_HIDDEN, "Require RMB holding");
                Text rmbHoldHeader4 = pageBuilder.Header4("Require holding right mouse button when controls are hidden");
                Vector2 rmbHoldHeader4SizeDelta = (rmbHoldHeader4.transform.parent as RectTransform).sizeDelta;
                rmbHoldHeader4SizeDelta.y += 15f;
                (rmbHoldHeader4.transform.parent as RectTransform).sizeDelta = rmbHoldHeader4SizeDelta;
            }


            _ = pageBuilder.Header1("Transitions");
            _ = pageBuilder.Toggle(ModSettingIDs.OVERHAUL_SCENE_TRANSITIONS, "Better scene transitions", "Interface");
            if (TransitionManager.OverhaulSceneTransitions)
            {
                _ = pageBuilder.Toggle(ModSettingIDs.TRANSITION_SOUND, "Transition sound");
            }

            _ = pageBuilder.Toggle(ModSettingIDs.TRANSITION_ON_STARTUP, "Transition on startup");
            _ = pageBuilder.Header4("Doborog logo will be smoothly faded out on game start");
        }

        private void populateGraphicsPage(SettingsMenu settingsMenu)
        {
            settingsMenu.refreshResolutionOptions();

            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Window");
            _ = pageBuilder.Dropdown(settingsMenu.ScreenResolutionDropDown.options, settingsMenu.ScreenResolutionDropDown.value, OnScreenResolutionChanged);
            _ = pageBuilder.Toggle(settingsMenu.FullScreenToggle.isOn, OnFullScreenChanged, "Fullscreen");

            if (ModSpecialUtils.SupportsTitleBarOverhaul())
            {
                _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_TITLE_BAR_OVERHAUL, "Enable title bar changes");
            }

            bool vsyncToggleValue = settingsMenu.VsyncOnToggle.isOn;
            _ = pageBuilder.Toggle(vsyncToggleValue, delegate (bool value)
            {
                OnVSyncChanged(value);
                PopulatePage("Graphics");
            }, "V-Sync");

            int fpsCapValue = FPSManager.Instance.GetFPSCapDropdownValue();
            Dropdown fpsCapDropdown = pageBuilder.Dropdown(FPSManager.FPSCapOptions, fpsCapValue, delegate (int value)
            {
                FPSManager.Instance.SetFPSCapDropdownValue(value);
                PopulatePage("Graphics");
            });
            fpsCapDropdown.interactable = !vsyncToggleValue;

            if (vsyncToggleValue) _ = pageBuilder.Header4("Turn V-Sync off to customize FPS");

            if (!vsyncToggleValue && fpsCapValue == 0)
            {
                Slider fpsCapSlider = pageBuilder.Slider(2, 200, true, Mathf.RoundToInt(ModSettingsManager.GetIntValue(ModSettingIDs.FPS_CAP) / 5f), delegate (float value)
                {
                    ModSettingsManager.SetIntValue(ModSettingIDs.FPS_CAP, Mathf.RoundToInt(value * 5f), true);
                }, true, (float val) =>
                {
                    return $"{Mathf.RoundToInt(val * 5f)} FPS";
                });
                RectTransform fpsCapSliderRectTransform = fpsCapSlider.transform as RectTransform;
                Vector2 fpsCapSliderRectTransformSizeDelta = fpsCapSliderRectTransform.sizeDelta;
                fpsCapSliderRectTransformSizeDelta.x = 325f;
                fpsCapSliderRectTransform.sizeDelta = fpsCapSliderRectTransformSizeDelta;
            }

            _ = pageBuilder.Header1("Render");
            _ = pageBuilder.Dropdown(settingsMenu.QualityDropDown.options, settingsMenu.QualityDropDown.value, OnQualityDropdownChanged);
            _ = pageBuilder.Dropdown(settingsMenu.AntiAliasingDropdown.options, settingsMenu.AntiAliasingDropdown.value, OnAntiAliasingDropdownChanged);

            _ = pageBuilder.Button("Additional settings", delegate
            {
                ClearPageContents();
                populateAdditionalGraphicsPage(settingsMenu);
            });


            _ = pageBuilder.Header1("Camera");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_CAMERA_ROLLING, "Camera rolling");
            _ = pageBuilder.Toggle(ModSettingIDs.DISABLE_SCREEN_SHAKING, "Disable shaking effects");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_FOV_OVERRIDE, "Enable FOV override", "Graphics");
            Text fovOverrideHeader4 = pageBuilder.Header4("fov_override");
            Vector2 fovOverrideHeader4SizeDelta = (fovOverrideHeader4.transform.parent as RectTransform).sizeDelta;
            fovOverrideHeader4SizeDelta.y += 15f;
            (fovOverrideHeader4.transform.parent as RectTransform).sizeDelta = fovOverrideHeader4SizeDelta;

            if (CameraFOVController.EnableFOVOverride)
            {
                _ = pageBuilder.Slider(-10f, CameraFOVController.FOV_MAX_POSITIVE_OFFSET, true, ModSettingsManager.GetFloatValue(ModSettingIDs.CAMERA_FOV_OFFSET), delegate (float value)
                {
                    ModSettingsManager.SetFloatValue(ModSettingIDs.CAMERA_FOV_OFFSET, value, true);
                }, true, (float val) =>
                {
                    float roundedValue = Mathf.Round(val * 10f) / 10f;
                    return $"{(val > 0f ? "+" : string.Empty)}{roundedValue} ({CameraFOVController.DEFAULT_FOV + roundedValue})";
                });
            }

            _ = pageBuilder.KeyBind(ModSettingIDs.CAMERA_MODE_TOGGLE_KEYBIND, "Camera mode", KeyCode.Y);
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_FIRST_PERSON_MODE, "First person mode");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_CAMERA_BOBBING, "Camera bobbing");

            _ = pageBuilder.Button("Reset camera settings", delegate
            {
                ModSettingsManager.ResetValue(ModSettingIDs.CAMERA_FOV_OFFSET, true);
                ModSettingsManager.ResetValue(ModSettingIDs.ENABLE_CAMERA_ROLLING, true);
                ModSettingsManager.ResetValue(ModSettingIDs.ENABLE_CAMERA_BOBBING, true);
                PopulatePage("Graphics");
            });


            bool showExperimentalSettings = ModFeatures.IsEnabled(ModFeatures.FeatureType.DisplayNewGraphicsOptionsInSettings);

            _ = pageBuilder.Header1("Post effects");
            if (showExperimentalSettings)
            {
                _ = pageBuilder.Dropdown(PostEffectsManager.PresetOptions, 0, delegate (int value)
                {
                    if (value == 0)
                        return;

                    PostEffectsManager.Instance.ApplyGraphicsPreset(value - 1);
                    settingsMenu.populateSettings();

                    ClearPageContents();
                    populateGraphicsPage(settingsMenu);
                });
            }

            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.ENABLE_SSAO, "Ambient occlusion", "SSAO");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_SSAO);

            if (showExperimentalSettings || ModSettingsManager.GetBoolValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION))
            {
                _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, "Global Illumination");
                pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION);
            }

            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, "Chromatic aberration", "CA");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION);

            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_DITHERING, "Dithering");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_DITHERING);
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_VIGNETTE, "Vignette");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_VIGNETTE);

            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_DOF, "Depth of field (DoF)");
            pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_DOF);

            if (showExperimentalSettings || ModSettingsManager.GetBoolValue(ModSettingIDs.ENABLE_SUN_SHAFTS))
            {
                _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_SUN_SHAFTS, "Sun shafts");
                pageBuilder.AddDescriptionBoxToRecentElement(ModSettingIDs.ENABLE_SUN_SHAFTS);
            }

            List<Dropdown.OptionData> bloomOptions = PostEffectsManager.BloomOptions;
            bloomOptions[0].text = LocalizationManager.Instance.GetTranslatedString("settings_option_disabled");
            bloomOptions[1].text = LocalizationManager.Instance.GetTranslatedString("settings_option_vanilla");
            bloomOptions[2].text = LocalizationManager.Instance.GetTranslatedString("settings_option_fancy");
            bloomOptions[3].text = LocalizationManager.Instance.GetTranslatedString("settings_option_fanciest");
            _ = pageBuilder.DropdownWithText(bloomOptions, ModSettingIDs.BLOOM_MODE, "Bloom");

            _ = pageBuilder.Header1("Color blindness mode");

            List<Dropdown.OptionData> colorBlindnessOptions = PostEffectsManager.ColorBlindnessOptions;
            colorBlindnessOptions[0].text = LocalizationManager.Instance.GetTranslatedString("settings_option_normal_vision");
            colorBlindnessOptions[1].text = LocalizationManager.Instance.GetTranslatedString("settings_option_protanopia");
            colorBlindnessOptions[2].text = LocalizationManager.Instance.GetTranslatedString("settings_option_deuteranopia");
            colorBlindnessOptions[3].text = LocalizationManager.Instance.GetTranslatedString("settings_option_tritanopia");

            _ = pageBuilder.Dropdown(PostEffectsManager.ColorBlindnessOptions, ModSettingIDs.COLOR_BLINDNESS_MODE);
            _ = pageBuilder.Toggle(ModSettingIDs.COLOR_BLINDNESS_AFFECT_UI, "Affect UI");
        }

        private void populateEffectsPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);

            _ = pageBuilder.Header1("Particles");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_HIT_PARTICLES, "Rework hit particles", "Effects");
            _ = pageBuilder.Toggle(ModSettingIDs.NEW_EXPLOSION_PARTICLES, "Rework explosion particles", "Effects");
            if (ModSettingsManager.GetBoolValue(ModSettingIDs.ENABLE_HIT_PARTICLES) || ModSettingsManager.GetBoolValue(ModSettingIDs.NEW_EXPLOSION_PARTICLES))
            {
                _ = pageBuilder.Toggle(ModSettingIDs.REDUCE_FLASHES, "Reduce flashes");
            }
            _ = pageBuilder.Toggle(ModSettingIDs.NEW_WELDING_PARTICLES, "Rework welding particles", "Effects");

            _ = pageBuilder.Header1("Voxel engine");
            _ = pageBuilder.Toggle(ModSettingIDs.CHANGE_HIT_COLORS, "Better damage colors");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_VOXEL_FIRE_FADING, "Better fire spreading");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_VOXEL_BURNING, "Always burn voxels");

            _ = pageBuilder.Header3("Chunk update delay");
            _ = pageBuilder.Slider(0, 2, true, ModSettingsManager.GetIntValue(ModSettingIDs.CHUNK_UPDATE_DELAY), delegate (float value)
            {
                ModSettingsManager.SetIntValue(ModSettingIDs.CHUNK_UPDATE_DELAY, Mathf.RoundToInt(value), true);
            }, true, (float val) =>
            {
                ChunkUpdateDelay value = (ChunkUpdateDelay)Mathf.RoundToInt(val);
                return LocalizationManager.Instance.GetTranslatedString($"chunkupdatedelay_{value.ToString().ToLower()}");
            });
            _ = pageBuilder.Header4("chunkupdatedelay_desc");

            _ = pageBuilder.Header1("Robots");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_WEAPON_BAG, "Weapons on back");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_ARROW_REWORK, "New arrow model");

            _ = pageBuilder.Header1("Environment");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_ARENA_REMODEL, "Arena remodel");
            _ = pageBuilder.Header4("arena_rework");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_LIGHTING_TRANSITION, "Lighting transitions");
            _ = pageBuilder.Header4("The lighting changes smoothly as level switches");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_FLOATING_DUST, "Floating dust");

            _ = pageBuilder.Header1("Garbage");
            _ = pageBuilder.Dropdown(settingsMenu.GarbageSettingsDropdown.options, settingsMenu.GarbageSettingsDropdown.value, OnGarbageSettingsChanged);
            _ = pageBuilder.Toggle(settingsMenu.PlayerPushesGarbageToggle.isOn, OnPlayerPushesGarbageToggleChanged, "Collisions");
        }

        private void populateGameplayPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Difficulty");
            _ = pageBuilder.Dropdown(settingsMenu.StoryModeDifficultyDropDown.options, settingsMenu.StoryModeDifficultyDropDown.value, OnStoryDifficultyIndexChanged);
            _ = pageBuilder.Header4("Change what enemies spawn");

            _ = pageBuilder.Header1("Endless levels");
            _ = pageBuilder.Dropdown(settingsMenu.WorkshopLevelPolicyDropdown.options, settingsMenu.WorkshopLevelPolicyDropdown.value, OnWorkshopEndlessLevelPolicyIndexChanged);
            Button button = pageBuilder.Button("Get more levels", delegate
            {
                Hide();
                if (ModUIManager.ShowWorkshopBrowserRework)
                    _ = ModUIs.ShowWorkshopBrowserRework();
                else
                    ModCache.TitleScreenUI.OnWorkshopBrowserButtonClicked();
            });
            button.interactable = GameModeManager.IsOnTitleScreen();

            _ = pageBuilder.Header1("Twitch");
            _ = pageBuilder.Button("Enemy spawn settings", delegate
            {
                settingsMenu.OnTwitchEnemyLimitButtonClicked();
                ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                {
                    ModCache.UIRoot.TwitchEnemySettingsMenu.Hide();
                });
            });
            _ = pageBuilder.Toggle(settingsMenu.MuteEmotesToggle.isOn, OnMuteEmotesToggleChanged, "Mute twitch emotes");
            _ = pageBuilder.Toggle(settingsMenu.DevIsLiveEnabledToggle.isOn, OnDevIsLiveToggleChanged, "Dev stream notifications");

            _ = pageBuilder.Header1("Player");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_SCROLL_TO_SWITCH_WEAPON, "Scroll to switch weapon", "Gameplay");
            if (CharacterExtension.EnableScrollToSwitchWeapon)
            {
                _ = pageBuilder.Header3("Cooldown");
                _ = pageBuilder.Slider(2, 50, true, Mathf.RoundToInt(ModSettingsManager.GetFloatValue(ModSettingIDs.WEAPON_SWITCH_COOLDOWN) * 100f), delegate (float value)
                {
                    ModSettingsManager.SetFloatValue(ModSettingIDs.WEAPON_SWITCH_COOLDOWN, value / 100f, true);
                }, true, (float val) =>
                {
                    return $"{Mathf.RoundToInt(val * 10f)} {LocalizationManager.Instance.GetTranslatedString("milliseconds_shortened")}";
                });
            }
        }

        private void populateSoundsPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Volume");
            _ = pageBuilder.Header3("Global");
            _ = pageBuilder.Slider(0f, 1f, false, settingsMenu.SoundVolume.value, OnGlobalVolumeChanged);
            _ = pageBuilder.Header3("Music");
            _ = pageBuilder.Slider(0f, 1f, false, settingsMenu.MusicVolume.value, OnMusicVolumeChanged);
            _ = pageBuilder.Header3("Commentator");
            _ = pageBuilder.Slider(0f, 1f, false, settingsMenu.CommentatorsVolume.value, OnCommentatorVolumeChanged);

            _ = pageBuilder.Header1("Filters");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_REVERB_FILTER, "Reverb", "Sounds");

            if (ModAudioManager.EnableReverbFilter)
            {
                _ = pageBuilder.Header3("Reverb intensity");
                _ = pageBuilder.SliderFloat(0.1f, 1.5f, ModSettingIDs.REVERB_FILTER_INTENSITY);
            }

            _ = pageBuilder.Header1("Misc.");
            _ = pageBuilder.Toggle(ModSettingIDs.UI_SOUNDS, "Interface sounds");
            _ = pageBuilder.Toggle(ModSettingIDs.CUSTOMIZATION_EDITOR_AMBIANCE, "Customization editor ambiance");
            _ = pageBuilder.ToggleWithOptions(ModSettingIDs.MUTE_SOUND_WHEN_UNFOCUSED, "Mute sounds when unfocused", "MuteSound");
        }

        private void populateMultiplayerPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Multiplayer settings");

            _ = pageBuilder.Header3("Preferred region");
            _ = pageBuilder.Dropdown(settingsMenu.RegionDropdown.options, settingsMenu.RegionDropdown.value, OnRegionChanged);
            _ = pageBuilder.Toggle(settingsMenu.RelayToggle.isOn, OnRelayToggleChanged, "Relay connection");
            _ = pageBuilder.Header4("Improves ping on some machines, but can also make it worse");

            _ = pageBuilder.Button("Manage muted players", delegate
            {
                ModCache.UIRoot.BlockListSettingsUI.Show();
                ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                {
                    ModCache.UIRoot.BlockListSettingsUI.Hide();
                });
            });

            _ = pageBuilder.Header1("Player");
            _ = pageBuilder.Button("Select emotes", delegate
            {
                ModCache.UIRoot.EmoteSettingsUI.Show();
            });
        }

        private void populateControlsPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Controls settings");
            _ = pageBuilder.Button("Edit controls", delegate
            {
                ModCache.UIRoot.ControlMapper.Open();
                ModUIManager.Instance.InvokeActionInsteadOfHidingCustomUI(delegate
                {
                    ModCache.UIRoot.ControlMapper.Close(true);
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

        private void populateDefaultPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("This page is not implemented yet.");
            _ = pageBuilder.Header2("Try using original menu");
            _ = pageBuilder.Button("Open original settings menu", OnLegacyUIButtonClicked);
        }

        private void populateAdvancedPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);

            if (ModBuild.IsDebugBuild)
            {
                _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingIDs.ENABLE_DEBUG_MENU), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(ModSettingIDs.ENABLE_DEBUG_MENU, value, true);
                }, "Debug menu");
            }

            _ = pageBuilder.Header1("Mod-Bot");
            _ = pageBuilder.Header3("Controls");
            _ = pageBuilder.KeyBind("Open console", ModBotPrefs.GetKeyCode(ModBotInputType.OpenConsole), KeyCode.F1, delegate (KeyCode value)
            {
                ModBotPrefs.InputOptions[0].Key = value;
            });
            _ = pageBuilder.KeyBind("Toggle FPS label", ModBotPrefs.GetKeyCode(ModBotInputType.ToggleFPSLabel), KeyCode.F3, delegate (KeyCode value)
            {
                ModBotPrefs.InputOptions[1].Key = value;
            });

            _ = pageBuilder.Header3("Website integration");
            if (API.HasSession)
            {
                _ = pageBuilder.Button("Edit tags", delegate
                {
                    Application.OpenURL("https://modbot.org/tagBrowsing.html");
                });

                Button button = null;
                button = pageBuilder.Button("Sign out", delegate
                {
                    button.interactable = false;
                    API.SignOut(onSignedOut);
                });
            }
            else
            {
                _ = pageBuilder.Button("Sign in", delegate
                {
                    ModCache.UIRoot.SettingsMenu.Hide();
                    ModBotUIRoot.Instance.ModBotSignInUI.OpenSignInForm();
                });
            }

            _ = pageBuilder.Header1("Rich presence");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_RPC, "Enable rich presence");
            _ = pageBuilder.Toggle(ModSettingIDs.RPC_DETAILS, "Display details");
            _ = pageBuilder.Toggle(ModSettingIDs.RPC_DISPLAY_LEVEL_FILE_NAME, "Display editing level name");

            _ = pageBuilder.Header1("Reset settings");
            _ = pageBuilder.Button("Reset Overhaul settings", delegate
            {
                ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("settings_reset_settings_header"), LocalizationManager.Instance.GetTranslatedString("settings_reset_settings_description"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Yes", "No", null, delegate
                {
                    ModSettingsManager.Instance.ResetSettings();
                    _ = ModUIs.ShowRestartRequiredScreen(true);
                });
            });
        }

        private void populateAdditionalGraphicsPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Button("Go back", delegate
            {
                ClearPageContents();
                populateGraphicsPage(settingsMenu);
            });

            _ = pageBuilder.Header1("Shadows");
            _ = pageBuilder.Header3("Resolution");
            _ = pageBuilder.Dropdown(QualityManager.Instance.GetShadowResolutionOptions(), ModSettingIDs.SHADOW_RESOLUTION, delegate (int value)
            {
                QualityManager.Instance.RefreshQualitySettings();
            });
            _ = pageBuilder.Header3("Distance");
            _ = pageBuilder.Slider(100f, 1500f, true, ModSettingsManager.GetFloatValue(ModSettingIDs.SHADOW_DISTANCE), delegate (float value)
            {
                ModSettingsManager.SetFloatValue(ModSettingIDs.SHADOW_DISTANCE, value, true);
                QualityManager.Instance.RefreshQualitySettings();
            }, true, (float val) =>
            {
                float roundedValue = Mathf.Round(val * 10f) / 10f;
                return roundedValue.ToString();
            });

            _ = pageBuilder.Header1("Lights");
            _ = pageBuilder.Toggle(ModSettingIDs.UNLIMITED_LIGHT_SOURCES, "Unlimited light sources", "Graphics2", delegate
            {
                QualityManager.Instance.RefreshQualitySettings();
            });
            _ = pageBuilder.Header4("Not recommended for low end devices");

            if (!ModSettingsManager.GetBoolValue(ModSettingIDs.UNLIMITED_LIGHT_SOURCES))
            {
                _ = pageBuilder.Header3("Limit");
                _ = pageBuilder.Slider(1, 15, true, ModSettingsManager.GetIntValue(ModSettingIDs.MAX_LIGHT_COUNT), delegate (float value)
                {
                    ModSettingsManager.SetIntValue(ModSettingIDs.MAX_LIGHT_COUNT, Mathf.RoundToInt(value), true);
                    QualityManager.Instance.RefreshQualitySettings();
                }, true, (float val) =>
                {
                    int roundedValue = Mathf.RoundToInt(val);
                    return roundedValue.ToString();
                });
            }
        }

        private void populateLanguagesPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);

            Text header = pageBuilder.Header1("Change language");
            header.alignment = TextAnchor.LowerCenter;

            RectTransform container = pageBuilder.GridContainer(new Vector2(410f, 1f), new Vector2(200f, 50f), new Vector2(5f, 5f));
            pageBuilder.LanguageButton("en", container);
            pageBuilder.LanguageButton("fr", container);
            pageBuilder.LanguageButton("it", container);
            pageBuilder.LanguageButton("de", container);
            pageBuilder.LanguageButton("es-ES", container);
            pageBuilder.LanguageButton("es-419", container);
            pageBuilder.LanguageButton("zh-CN", container);
            pageBuilder.LanguageButton("zh-TW", container);
            pageBuilder.LanguageButton("ru", container);
            pageBuilder.LanguageButton("pt-BR", container);
            pageBuilder.LanguageButton("ja", container);
            pageBuilder.LanguageButton("ko", container);
        }

        private void populateSubtitlesReworkSettingsPage(string initialPage)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Button("Go back", delegate
            {
                ClearPageContents();
                PopulatePage(initialPage);
            });

            _ = pageBuilder.Header1("Commentator subtitles rework");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_SUBTITLE_TEXT_FIELD_REWORK, "Enable", delegate
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });
            _ = pageBuilder.Toggle(ModSettingIDs.SUBTITLE_TEXT_FIELD_UPPER_POSITION, "Be on top", delegate
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });
            _ = pageBuilder.Toggle(ModSettingIDs.SUBTITLE_TEXT_FIELD_BG, "Enable background", delegate
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });
            _ = pageBuilder.Toggle(ModSettingIDs.SHOW_SPEAKER_NAME, "Display who's speaking", "SubtitlesRework", delegate
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });

            if (ModSettingsManager.GetBoolValue(ModSettingIDs.SHOW_SPEAKER_NAME))
            {
                _ = pageBuilder.Toggle(ModSettingIDs.SWAP_SUBTITLES_COLOR, "Swap subtitles color", delegate
                {
                    SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
                });
            }

            _ = pageBuilder.Header3("Font");
            _ = pageBuilder.Dropdown(ModConstants.GetFontOptions(ModSettingsManager.GetIntValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_FONT)), ModSettingIDs.SUBTITLE_TEXT_FIELD_FONT, delegate
            {
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            });
            _ = pageBuilder.Header4("Some languages might not be supported by certain fonts");

            if (!AddonManager.Instance.HasInstalledAddon(AddonManager.EXTRAS_ADDON_ID, 0))
                pageBuilder.AddonDownload("Install \"Extras\" addon for more fonts", AddonManager.EXTRAS_ADDON_ID, 0, delegate
                {
                    PopulatePage("SubtitlesRework");
                });

            _ = pageBuilder.Header3("Font size");
            _ = pageBuilder.Slider(8f, 15f, true, ModSettingsManager.GetIntValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_FONT_SIZE), delegate (float value)
            {
                ModSettingsManager.SetIntValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_FONT_SIZE, Mathf.RoundToInt(value), true);
                SpeechAudioManager.Instance.PlaySequence("CloneDroneIntro", false);
            }, true, (float val) =>
            {
                return $"{Mathf.RoundToInt(val)}";
            });

            _ = pageBuilder.Button("Reset settings", delegate
            {
                ModSettingsManager.ResetValue(ModSettingIDs.ENABLE_SUBTITLE_TEXT_FIELD_REWORK, true);
                ModSettingsManager.ResetValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_UPPER_POSITION, true);
                ModSettingsManager.ResetValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_BG, true);
                ModSettingsManager.ResetValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_FONT, true);
                ModSettingsManager.ResetValue(ModSettingIDs.SUBTITLE_TEXT_FIELD_FONT_SIZE, true);

                ClearPageContents();
                populateSubtitlesReworkSettingsPage(initialPage);
            });
        }

        private void populateUKTDReworkSettingsPage(string initialPage)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Button("Go back", delegate
            {
                ClearPageContents();
                PopulatePage(initialPage);
            });

            _ = pageBuilder.Header1("Use key trigger description rework");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_PRESS_BUTTON_TRIGGER_DESCRIPTION_REWORK, "Enable", delegate (bool value)
            {
                if (value) UseKeyTriggerManager.Instance.ShowThenHideDescription(ModConstants.LoremIpsumText, 2f);
            });
            _ = pageBuilder.Toggle(ModSettingIDs.PAK_DESCRIPTION_BG, "Enable background", delegate (bool value)
            {
                UseKeyTriggerManager.Instance.ShowThenHideDescription(ModConstants.LoremIpsumText, 2f);
            });

            _ = pageBuilder.Header3("Font");
            _ = pageBuilder.Dropdown(ModConstants.GetFontOptions(ModSettingsManager.GetIntValue(ModSettingIDs.PAK_DESCRIPTION_FONT)), ModSettingIDs.PAK_DESCRIPTION_FONT, delegate
            {
                UseKeyTriggerManager.Instance.ShowThenHideDescription(ModConstants.LoremIpsumText, 2f);
            });
            _ = pageBuilder.Header4("Some languages might not be supported by certain fonts");

            if (!AddonManager.Instance.HasInstalledAddon(AddonManager.EXTRAS_ADDON_ID, 0))
                pageBuilder.AddonDownload("Install \"Extras\" addon for more fonts", AddonManager.EXTRAS_ADDON_ID, 0, delegate
                {
                    ClearPageContents();
                    populateUKTDReworkSettingsPage(initialPage);
                });

            _ = pageBuilder.Header3("Font size");
            _ = pageBuilder.Slider(8f, 13f, true, ModSettingsManager.GetIntValue(ModSettingIDs.PAK_DESCRIPTION_FONT_SIZE), delegate (float value)
            {
                ModSettingsManager.SetIntValue(ModSettingIDs.PAK_DESCRIPTION_FONT_SIZE, Mathf.RoundToInt(value), true);
                UseKeyTriggerManager.Instance.ShowThenHideDescription(ModConstants.LoremIpsumText, 2f);
            }, true, (float val) =>
            {
                return $"{Mathf.RoundToInt(val)}";
            });

            _ = pageBuilder.Button("Reset settings", delegate
            {
                ModSettingsManager.ResetValue(ModSettingIDs.ENABLE_PRESS_BUTTON_TRIGGER_DESCRIPTION_REWORK, true);
                ModSettingsManager.ResetValue(ModSettingIDs.PAK_DESCRIPTION_BG, true);
                ModSettingsManager.ResetValue(ModSettingIDs.PAK_DESCRIPTION_FONT, true);
                ModSettingsManager.ResetValue(ModSettingIDs.PAK_DESCRIPTION_FONT_SIZE, true);

                ClearPageContents();
                populateUKTDReworkSettingsPage(initialPage);
            });
        }

        private void populateSSAOSettingsPage(string initialPage)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Button("Go back", delegate
            {
                ClearPageContents();
                PopulatePage(initialPage);
            });

            _ = pageBuilder.Header1("Ambient occlusion settings");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_SSAO, "Enable");

            _ = pageBuilder.Header3("Intensity");
            _ = pageBuilder.SliderFloat(0.4f, 1.2f, ModSettingIDs.SSAO_INTENSITY);

            _ = pageBuilder.Header3("Sample count");
            _ = pageBuilder.Slider(0f, 3f, true, ModSettingsManager.GetIntValue(ModSettingIDs.SSAO_SAMPLE_COUNT), delegate (float value)
            {
                ModSettingsManager.SetIntValue(ModSettingIDs.SSAO_SAMPLE_COUNT, Mathf.RoundToInt(value), true);
            }, true, (float val) =>
            {
                SampleCountLevel sampleCountLevel = (SampleCountLevel)Mathf.RoundToInt(val);
                switch (sampleCountLevel)
                {
                    case SampleCountLevel.Low:
                        return LocalizationManager.Instance.GetTranslatedString("settings_option_low");
                    case SampleCountLevel.Medium:
                        return LocalizationManager.Instance.GetTranslatedString("settings_option_medium");
                    case SampleCountLevel.High:
                        return LocalizationManager.Instance.GetTranslatedString("settings_option_high");
                    case SampleCountLevel.VeryHigh:
                        return LocalizationManager.Instance.GetTranslatedString("settings_option_very_high");
                }
                return sampleCountLevel.ToString();
            });

            _ = pageBuilder.Button("Reset settings", delegate
            {
                ModSettingsManager.ResetValue(ModSettingIDs.ENABLE_SSAO, true);
                ModSettingsManager.ResetValue(ModSettingIDs.SSAO_INTENSITY, true);
                ModSettingsManager.ResetValue(ModSettingIDs.SSAO_SAMPLE_COUNT, true);

                ClearPageContents();
                populateSSAOSettingsPage(initialPage);
            });
        }

        private void populateCASettingsPage(string initialPage)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Button("Go back", delegate
            {
                ClearPageContents();
                PopulatePage(initialPage);
            });

            _ = pageBuilder.Header1("Chromatic aberration settings");
            _ = pageBuilder.Toggle(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, "Enable");

            _ = pageBuilder.Header3("Intensity");
            _ = pageBuilder.SliderFloat(0.01f, 1f, ModSettingIDs.CHROMATIC_ABERRATION_INTENSITY);

            _ = pageBuilder.Toggle(ModSettingIDs.CHROMATIC_ABERRATION_ON_SCREEN_EDGES, "On screen edges");

            _ = pageBuilder.Button("Reset settings", delegate
            {
                ModSettingsManager.ResetValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, true);
                ModSettingsManager.ResetValue(ModSettingIDs.CHROMATIC_ABERRATION_INTENSITY, true);
                ModSettingsManager.ResetValue(ModSettingIDs.CHROMATIC_ABERRATION_ON_SCREEN_EDGES, true);

                ClearPageContents();
                populateCASettingsPage(initialPage);
            });
        }

        private void populateMuteSoundPage(string initialPage)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Button("Go back", delegate
            {
                ClearPageContents();
                PopulatePage(initialPage);
            });

            _ = pageBuilder.Header1("Mute sounds when unfocused");
            _ = pageBuilder.Toggle(ModSettingIDs.MUTE_SOUND_WHEN_UNFOCUSED, "Enable", "MuteSound");

            if (ModSettingsManager.GetBoolValue(ModSettingIDs.MUTE_SOUND_WHEN_UNFOCUSED))
            {
                _ = pageBuilder.Toggle(ModSettingIDs.MUTE_SOUND_INSTANTLY_WHEN_UNFOCUSED, "Mute instantly", "MuteSound");
                if (!ModSettingsManager.GetBoolValue(ModSettingIDs.MUTE_SOUND_INSTANTLY_WHEN_UNFOCUSED))
                {
                    _ = pageBuilder.Header3("Speed multiplier");
                    _ = pageBuilder.Slider(2f, 30f, true, ModSettingsManager.GetFloatValue(ModSettingIDs.MUTE_SPEED_MULTIPLIER) * 10f, delegate (float value)
                    {
                        ModSettingsManager.SetFloatValue(ModSettingIDs.MUTE_SPEED_MULTIPLIER, value / 10f, true);
                    }, true, (float val) =>
                    {
                        return $"{val / 10f}";
                    });
                }
                _ = pageBuilder.Toggle(ModSettingIDs.MUTE_MASTER_VOLUME_WHEN_UNFOCUSED, "Mute all sounds", "MuteSound");
                if (!ModSettingsManager.GetBoolValue(ModSettingIDs.MUTE_MASTER_VOLUME_WHEN_UNFOCUSED))
                {
                    _ = pageBuilder.Toggle(ModSettingIDs.MUTE_MUSIC_WHEN_UNFOCUSED, "Mute music");
                    _ = pageBuilder.Toggle(ModSettingIDs.MUTE_COMMENTATORS_WHEN_UNFOCUSED, "Mute commentators");
                }
            }
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.TitleScreenUI;
            if (titleScreenUI && GameModeManager.IsOnTitleScreen())
            {
                Hide();
                titleScreenUI.OnOptionsButtonClicked();
                return;
            }

            SettingsMenu settingsMenu = ModCache.UIRoot.SettingsMenu;
            if (settingsMenu)
            {
                Hide();

                ModActionUtils.DoInFrame(delegate
                {
                    settingsMenu.Show();
                });
            }
        }

        public void OnImportSettingsButtonClicked()
        {
            ModUIUtils.FileExplorer(base.transform, true, delegate (string path)
            {
                if (path.IsNullOrEmpty())
                    return;

                ModSettingsDataContainer modSettingsDataContainer;
                try
                {
                    modSettingsDataContainer = ModJsonUtils.DeserializeStream<ModSettingsDataContainer>(path);
                    modSettingsDataContainer.FixValues();
                }
                catch (Exception)
                {
                    ModUIUtils.MessagePopupOK("Import error", "The file is corrupted.", true);
                    return;
                }

                ModSettingsDataManager.Instance.GetDataContainer().SetValues(modSettingsDataContainer, true);
                ModUIUtils.MessagePopupOK("Import successful", $"Imported the file \"{Path.GetFileNameWithoutExtension(path)}\".", true);
                PopulatePage(_selectedTabId);
            }, ModDirectories.SavesFolder, "*.json");
        }

        public void OnExportSettingsButtonClicked()
        {
            ModUIs.ShowSettingsImportExportMenu(base.transform);
        }

        public void OnQualityDropdownChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.QualityDropDown.value = value;
        }

        public void OnAntiAliasingDropdownChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.AntiAliasingDropdown.value = value;
        }

        public void OnLanguageDropdownChanged(int value)
        {
            LocalizationManager localizationManager = LocalizationManager.Instance;
            if (localizationManager)
            {
                localizationManager.SetCurrentLanguage(localizationManager.SupportedLanguages[value].LanguageCode);

                _tabs.ReinstantiatePreconfiguredTabs();
                _tabs.SelectTab("Home");
            }
        }

        public void OnScreenResolutionChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.ScreenResolutionDropDown.value = value;
        }

        public void OnFullScreenChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.FullScreenToggle.isOn = value;
        }

        public void OnVSyncChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.VsyncOnToggle.isOn = value;
        }

        public void OnRegionChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.RegionDropdown.value = value;
        }

        public void OnStoryDifficultyIndexChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.StoryModeDifficultyDropDown.value = value;
        }

        public void OnWorkshopEndlessLevelPolicyIndexChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.WorkshopLevelPolicyDropdown.value = value;
        }

        public void OnDevIsLiveToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.DevIsLiveEnabledToggle.isOn = value;
        }

        public void OnMuteEmotesToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.MuteEmotesToggle.isOn = value;
        }

        public void OnHideGameUIToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.HideGameUIToggle.isOn = !value;
        }

        public void OnGarbageSettingsChanged(int value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.GarbageSettingsDropdown.value = value;
        }

        public void OnPlayerPushesGarbageToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.PlayerPushesGarbageToggle.isOn = value;
        }

        public void OnSubtitlesToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.SubtitlesToggle.isOn = value;
        }

        public void OnGlobalVolumeChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.SoundVolume.value = value;
        }

        public void OnMusicVolumeChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.MusicVolume.value = value;
        }

        public void OnCommentatorVolumeChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.CommentatorsVolume.value = value;
        }

        public void OnMouseSensitivityChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.MouseSensitivitySlider.value = value;
        }

        public void OnInvertMouseToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.InvertMouseToggle.isOn = value;
        }

        public void OnControllerSensitivityChanged(float value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.ControllerSensitivitySlider.value = value;
        }

        public void OnInvertControllerToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.InvertControllerToggle.isOn = value;
        }

        public void OnEqualLookRatioToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.EqualLookRatioToggle.isOn = value;
        }

        public void OnRelayToggleChanged(bool value)
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.RelayToggle.isOn = value;
        }

        private static void onSignedOut(JsonObject jsonObject)
        {
            ModLibrary.ModBotUserIdentifier.Instance.SignOut();
            VersionLabelManager.Instance.SetLine(2, "Not signed in");

            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Get<UISettingsMenuRework>(ModAssetBundles.UI, ModUIs.UI_SETTINGS_MENU_REWORK);
            if (settingsMenuRework) settingsMenuRework.PopulatePage("Advanced");
        }

        public class PageBuilder
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

                if (!localizationId.IsNullOrEmpty())
                {
                    LocalizedTextField localizedTextField = text.gameObject.AddComponent<LocalizedTextField>();
                    localizedTextField.LocalizationID = localizationId;
                }
            }

            private Text instantiateHeader(string text, string localizationId, ModdedObject prefab, Transform parentOverride = null)
            {
                ModdedObject moddedObject = Instantiate(prefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                Text textComponent = moddedObject.GetObject<Text>(0);
                textComponent.text = text;
                addLocalizedTextField(textComponent, localizationId);
                return textComponent;
            }

            private Dropdown instantiateDropdown(List<Dropdown.OptionData> list, int value, UnityAction<int> callback, Dropdown prefab, Transform parentOverride = null)
            {
                if (callback == null)
                    callback = delegate { ModUIUtils.MessagePopupNotImplemented(); };

                if (list == null)
                    list = new List<Dropdown.OptionData>();

                Dropdown dropdown = Instantiate(prefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                dropdown.gameObject.SetActive(true);
                dropdown.options = list;
                dropdown.value = value;
                if (callback != null)
                    dropdown.onValueChanged.AddListener(callback);
                return dropdown;
            }

            private Dropdown instantiateDropdownWithText(List<Dropdown.OptionData> list, string text, bool localize, int value, UnityAction<int> callback, ModdedObject prefab, Transform parentOverride = null)
            {
                if (callback == null)
                    callback = delegate { ModUIUtils.MessagePopupNotImplemented(); };

                if (list == null)
                    list = new List<Dropdown.OptionData>();

                ModdedObject moddedObject = Instantiate(prefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);

                Dropdown dropdown = moddedObject.GetObject<Dropdown>(0);
                dropdown.options = list;
                dropdown.value = value;
                if (callback != null)
                    dropdown.onValueChanged.AddListener(callback);

                Text textComponent = moddedObject.GetObject<Text>(1);
                textComponent.text = text;
                addLocalizedTextField(textComponent, localize ? $"settings_subheader_{text.ToLower().Replace(' ', '_')}" : null);

                return dropdown;
            }

            public Text Header1(string text, bool localize = true, Transform parentOverride = null)
            {
                return instantiateHeader(text, localize ? $"settings_header_{text.ToLower().Replace(' ', '_')}" : null, SettingsMenu.Header1Prefab, parentOverride);
            }

            public Text Header2(string text, bool localize = true, Transform parentOverride = null)
            {
                return instantiateHeader(text, localize ? $"settings_subheader_{text.ToLower().Replace(' ', '_')}" : null, SettingsMenu.Header2Prefab, parentOverride);
            }

            public Text Header3(string text, bool localize = true, Transform parentOverride = null)
            {
                return instantiateHeader(text, localize ? $"settings_subheader_{text.ToLower().Replace(' ', '_')}" : null, SettingsMenu.Header3Prefab, parentOverride);
            }

            public Text Header4(string text, bool localize = true, Transform parentOverride = null)
            {
                return instantiateHeader(text, localize ? $"settings_tooltip_{text.ToLower().Replace(' ', '_')}" : null, SettingsMenu.Header4Prefab, parentOverride);
            }

            public Dropdown Dropdown(List<Dropdown.OptionData> list, string settingId)
            {
                return Dropdown(list, ModSettingsManager.GetIntValue(settingId), delegate (int value)
                {
                    ModSettingsManager.SetIntValue(settingId, value, true);
                });
            }

            public Dropdown Dropdown(List<Dropdown.OptionData> list, string settingId, Action<int> action)
            {
                return Dropdown(list, ModSettingsManager.GetIntValue(settingId), delegate (int value)
                {
                    ModSettingsManager.SetIntValue(settingId, value, true);
                    action(value);
                });
            }

            public Dropdown Dropdown(List<Dropdown.OptionData> list, int value, UnityAction<int> callback, Transform parentOverride = null)
            {
                return instantiateDropdown(list, value, callback, SettingsMenu.DropdownPrefab, parentOverride);
            }

            public Dropdown DropdownWithImage(List<Dropdown.OptionData> list, string settingId)
            {
                return DropdownWithImage(list, ModSettingsManager.GetIntValue(settingId), delegate (int value)
                {
                    ModSettingsManager.SetIntValue(settingId, value, true);
                });
            }

            public Dropdown DropdownWithImage(List<Dropdown.OptionData> list, int value, UnityAction<int> callback, Transform parentOverride = null)
            {
                return instantiateDropdown(list, value, callback, SettingsMenu.DropdownWithImagePrefab, parentOverride);
            }

            public Dropdown DropdownWithImage169(List<Dropdown.OptionData> list, int value, UnityAction<int> callback, Transform parentOverride = null)
            {
                return instantiateDropdown(list, value, callback, SettingsMenu.DropdownWithImage169Prefab, parentOverride);
            }

            public Dropdown DropdownWithText(List<Dropdown.OptionData> list, string settingId, string text)
            {
                return DropdownWithText(list, text, true, ModSettingsManager.GetIntValue(settingId), delegate (int value)
                {
                    ModSettingsManager.SetIntValue(settingId, value, true);
                });
            }

            public Dropdown DropdownWithText(List<Dropdown.OptionData> list, string text, bool localize, int value, UnityAction<int> callback, Transform parentOverride = null)
            {
                return instantiateDropdownWithText(list, text, localize, value, callback, SettingsMenu.DropdownWithTextPrefab, parentOverride);
            }

            public Slider SliderFloat(float min, float max, string settingId, bool callbackOnChange = false)
            {
                return Slider(min, max, false, ModSettingsManager.GetFloatValue(settingId), delegate (float value)
                {
                    ModSettingsManager.SetFloatValue(settingId, value, true);
                }, callbackOnChange);
            }

            public Slider SliderInt(int min, int max, string settingId, bool callbackOnChange = false)
            {
                return Slider(min, max, true, Mathf.RoundToInt(ModSettingsManager.GetIntValue(settingId)), delegate (float value)
                {
                    ModSettingsManager.SetIntValue(settingId, Mathf.RoundToInt(value), true);
                }, callbackOnChange);
            }

            public Slider Slider(float min, float max, bool wholeNumbers, float value, UnityAction<float> callback, bool callbackOnChange = false, Func<float, string> fillTextFunc = null, Transform parentOverride = null)
            {
                Slider slider = Instantiate(SettingsMenu.SliderPrefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                slider.gameObject.SetActive(true);
                slider.minValue = min;
                slider.maxValue = max;
                slider.wholeNumbers = wholeNumbers;
                slider.value = value;
                if (callback != null)
                    slider.onValueChanged.AddListener(callback);

                if (!callbackOnChange)
                    _ = slider.gameObject.AddComponent<BetterSliderCallback>();

                if (fillTextFunc == null)
                    fillTextFunc = GetFillText;

                ModdedObject moddedObject = slider.GetComponent<ModdedObject>();

                SliderFillText sliderFillText = slider.gameObject.AddComponent<SliderFillText>();
                sliderFillText.SliderComponent = slider;
                sliderFillText.Label = moddedObject.GetObject<Text>(0);
                sliderFillText.Function = fillTextFunc;

                return slider;
            }

            public Toggle Toggle(string settingId, string text)
            {
                return Toggle(ModSettingsManager.GetBoolValue(settingId), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(settingId, value, true);
                }, text);
            }

            public Toggle Toggle(string settingId, string text, Action<bool> action)
            {
                return Toggle(ModSettingsManager.GetBoolValue(settingId), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(settingId, value, true);
                    action(value);
                }, text);
            }

            public Toggle Toggle(string settingId, string text, string pageToPopulate)
            {
                return Toggle(ModSettingsManager.GetBoolValue(settingId), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(settingId, value, true);
                    SettingsMenu.PopulatePage(pageToPopulate);
                }, text);
            }

            public Toggle Toggle(string settingId, string text, string pageToPopulate, Action<bool> action)
            {
                return Toggle(ModSettingsManager.GetBoolValue(settingId), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(settingId, value, true);
                    action(value);
                    SettingsMenu.PopulatePage(pageToPopulate);
                }, text);
            }

            public Toggle Toggle(bool isOn, UnityAction<bool> callback, string text, bool localize = true, Transform parentOverride = null)
            {
                if (callback == null)
                    callback = delegate { ModUIUtils.MessagePopupNotImplemented(); };

                ModdedObject moddedObject = Instantiate(SettingsMenu.TogglePrefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                Text textComponent = moddedObject.GetObject<Text>(1);
                textComponent.text = text;
                if (localize)
                    addLocalizedTextField(textComponent, $"settings_checkbox_{text.ToLower().Replace(' ', '_')}");
                Toggle toggle = moddedObject.GetObject<Toggle>(0);
                toggle.isOn = isOn;
                if (callback != null)
                    toggle.onValueChanged.AddListener(callback);

                return toggle;
            }

            public Toggle ToggleWithOptions(string settingId, string text, string pageToPopulate)
            {
                return ToggleWithOptions(ModSettingsManager.GetBoolValue(settingId), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(settingId, value, true);
                }, text, delegate
                {
                    SettingsMenu.PopulatePage(pageToPopulate);
                });
            }

            public Toggle ToggleWithOptions(string settingId, string text, string pageToPopulate, Action<bool> action)
            {
                return ToggleWithOptions(ModSettingsManager.GetBoolValue(settingId), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(settingId, value, true);
                    action(value);
                }, text, delegate
                {
                    SettingsMenu.PopulatePage(pageToPopulate);
                });
            }

            public Toggle ToggleWithOptions(bool isOn, UnityAction<bool> callback, string text, Action populatePageAction, bool localize = true, Transform parentOverride = null)
            {
                if (callback == null)
                    callback = delegate { ModUIUtils.MessagePopupNotImplemented(); };

                ModdedObject moddedObject = Instantiate(SettingsMenu.ToggleWithOptionsPrefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                Text textComponent = moddedObject.GetObject<Text>(2);
                textComponent.text = text;
                if (localize)
                    addLocalizedTextField(textComponent, $"settings_checkbox_{text.ToLower().Replace(' ', '_')}");
                Toggle toggle = moddedObject.GetObject<Toggle>(0);
                toggle.isOn = isOn;
                if (callback != null)
                    toggle.onValueChanged.AddListener(callback);

                Button button = moddedObject.GetObject<Button>(1);
                button.onClick.AddListener(delegate
                {
                    if (populatePageAction != null)
                    {
                        SettingsMenu.ClearPageContents();
                        populatePageAction();
                    }
                });

                return toggle;
            }

            public Button Button(string text, Action onClicked, bool localize = true, Transform parentOverride = null)
            {
                if (onClicked == null)
                    onClicked = ModUIUtils.MessagePopupNotImplemented;

                ModdedObject moddedObject = Instantiate(SettingsMenu.ButtonPrefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = localize ? LocalizationManager.Instance.GetTranslatedString($"settings_button_{text.ToLower().Replace(' ', '_')}") : text;
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(new UnityAction(onClicked));
                return button;
            }

            public UIElementKeyBindSetter KeyBind(string settingdId, string name, KeyCode defaultKey)
            {
                return KeyBind(name, (KeyCode)ModSettingsManager.GetIntValue(settingdId), defaultKey, delegate (KeyCode value)
                {
                    ModSettingsManager.SetIntValue(settingdId, (int)value, true);
                });
            }

            public UIElementKeyBindSetter KeyBind(string name, KeyCode keyCode, KeyCode defaultKey, Action<KeyCode> onChanged, Transform parentOverride = null)
            {
                ModdedObject moddedObject = Instantiate(SettingsMenu.KeyBindPrefab, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);

                UIElementKeyBindSetter elementKeyBind = moddedObject.gameObject.AddComponent<UIElementKeyBindSetter>();
                elementKeyBind.InitializeAsElement();
                elementKeyBind.key = keyCode;
                elementKeyBind.defaultKey = defaultKey;
                elementKeyBind.SetDescription(LocalizationManager.Instance.GetTranslatedString($"settings_subheader_{name.ToLower().Replace(' ', '_')}"));
                elementKeyBind.onValueChanged.AddListener(new UnityAction<KeyCode>(onChanged));

                return elementKeyBind;
            }

            public RectTransform GridContainer(Vector2 size, Vector2 cellSize, Vector2 spacing, Transform parentOverride = null)
            {
                GameObject gridContainerObject = Instantiate(SettingsMenu.GridContainer, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                gridContainerObject.SetActive(true);

                RectTransform rectTransform = gridContainerObject.GetComponent<RectTransform>();
                Vector2 sizeDelta = rectTransform.sizeDelta;
                sizeDelta.y = size.y;
                rectTransform.sizeDelta = sizeDelta;

                LayoutElement layoutElement = gridContainerObject.GetComponent<LayoutElement>();
                layoutElement.minWidth = size.x;

                GridLayoutGroup gridLayoutGroup = gridContainerObject.GetComponent<GridLayoutGroup>();
                gridLayoutGroup.cellSize = cellSize;
                gridLayoutGroup.spacing = spacing;

                return rectTransform;
            }

            public void LanguageButton(string langCode, Transform parentOverride = null)
            {
                ModdedObject moddedObject = Instantiate(SettingsMenu.LanguageButton, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);

                Text header = moddedObject.GetObject<Text>(0);
                Text subHeader = moddedObject.GetObject<Text>(1);

                string headerText;
                string subHeaderText;

                switch (langCode)
                {
                    case "en":
                        headerText = "English";
                        subHeaderText = "(English) [en]";
                        break;
                    case "fr":
                        headerText = "Français - la France";
                        subHeaderText = "(French - France) [fr]";
                        break;
                    case "it":
                        headerText = "Italiano";
                        subHeaderText = "(Italian) [it]";
                        break;
                    case "de":
                        headerText = "Deutsch";
                        subHeaderText = "(German) [de]";
                        break;
                    case "es-ES":
                        headerText = "Español - España";
                        subHeaderText = "(Spanish - Spain) [es-ES]";
                        break;
                    case "es-419":
                        headerText = "Español - Latinoamérica";
                        subHeaderText = "(Spanish - Latin America) [es-419]";
                        break;
                    case "zh-CN":
                        headerText = "简体中文";
                        subHeaderText = "(Simplified Chinese) [zh-CN]";
                        break;
                    case "zh-TW":
                        headerText = "繁體中文";
                        subHeaderText = "(Traditional Chinese) [zh-TW]";
                        break;
                    case "ru":
                        headerText = "Pусский";
                        subHeaderText = "(Russian) [ru]";
                        break;
                    case "pt-BR":
                        headerText = "Português do Brasil";
                        subHeaderText = "(Brazilian Portuguese) [pt-BR]";
                        break;
                    case "ja":
                        headerText = "日本語";
                        subHeaderText = "(Japanese) [ja]";
                        break;
                    case "ko":
                        headerText = "한국어";
                        subHeaderText = "(Korean) [ko]";
                        break;
                    default:
                        headerText = langCode;
                        subHeaderText = langCode;
                        break;
                }

                header.text = headerText;
                subHeader.text = subHeaderText;

                UnityEngine.UI.Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    LocalizationManager.Instance.SetCurrentLanguage(langCode);
                    ModCache.UIRoot.SettingsMenu.populateSettings();
                    SettingsMenu._tabs.ReinstantiatePreconfiguredTabs();
                    SettingsMenu._tabs.SelectTab("Languages");
                });
            }

            public void AddonDownload(string labelText, string addonId, int version, UnityAction addonInstalledCallback = null, Transform parentOverride = null)
            {
                ModdedObject moddedObject = Instantiate(SettingsMenu.AddonDownload, parentOverride ? parentOverride : SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);

                Text label = moddedObject.GetObject<Text>(4);
                label.text = labelText;

                UIElementAddonEmbed addonEmbed = moddedObject.gameObject.AddComponent<UIElementAddonEmbed>();
                addonEmbed.AddonId = addonId;
                if (addonInstalledCallback != null)
                    addonEmbed.onContentDownloaded.AddListener(addonInstalledCallback);
                addonEmbed.InitializeAsElement();
            }

            public void AddDescriptionBoxToRecentElement(string settingId)
            {
                Transform transform = SettingsMenu.PageContentsTransform.GetChild(SettingsMenu.PageContentsTransform.childCount - 1);

                UIElementMouseEventsComponent mouseEventsComponent = transform.gameObject.AddComponent<UIElementMouseEventsComponent>();
                mouseEventsComponent.InitializeAsElement();
                mouseEventsComponent.PointerEnterStateCallback = delegate (bool value)
                {
                    if (value)
                        SettingsMenu.ShowDescriptionBox(settingId, transform.transform as RectTransform);
                    else
                        SettingsMenu.HideDescription();
                };
            }

            public static string GetFillText(float value)
            {
                return $"{Mathf.Round(value * 100f)}%";
            }
        }
    }
}