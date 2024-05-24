using InternalModBot;
using ModBotWebsiteAPI;
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
        [UIElement("DropdownImage169Prefab", false)]
        public Dropdown DropdownWithImage169Prefab;

        [UIElement("SliderPrefab", false)]
        public Slider SliderPrefab;

        [UIElement("TogglePrefab", false)]
        public ModdedObject TogglePrefab;

        [UIElement("ButtonPrefab", false)]
        public ModdedObject ButtonPrefab;

        [UIElement("KeyBindPrefab", false)]
        public ModdedObject KeyBindPrefab;

        [TabManager(typeof(UIElementSettingsTab), nameof(m_tabPrefab), nameof(m_tabContainer), nameof(OnTabCreated), nameof(OnTabSelected), new string[] { "Quick setup", "Gameplay", "Graphics", "Sounds", "Controls", "Multiplayer", "Mod-Bot", "Advanced" })]
        private readonly TabManager m_tabs;
        [UIElement("TabPrefab", false)]
        private readonly ModdedObject m_tabPrefab;
        [UIElement("TabsContainer")]
        private readonly Transform m_tabContainer;

        [UIElement("PanelNew")]
        private readonly RectTransform m_panelTransform;

        [UIElement("Shading")]
        private readonly GameObject m_shadingObject;

        [UIElement("BG")]
        private readonly GameObject m_normalBgObject;

        [UIElement("BGSetup")]
        private readonly GameObject m_setupBgObject;

        private bool m_hasSelectedTab;

        private string m_selectedTabId;

        public override bool hideTitleScreen => true;

        public bool disallowUsingKey
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (settingsMenu)
                settingsMenu.populateSettings();
        }

        public void ShowRegularElements()
        {
            disallowUsingKey = false;
            m_panelTransform.anchorMax = new Vector2(1f, 1f);
            m_panelTransform.anchorMin = new Vector2(0f, 0f);
            m_panelTransform.sizeDelta = new Vector2(0f, 0f);
            m_shadingObject.SetActive(true);
            m_normalBgObject.SetActive(true);
            m_setupBgObject.SetActive(false);

            if (!m_hasSelectedTab)
            {
                m_tabs.SelectTab("Quick setup");
                m_hasSelectedTab = true;
            }
        }

        public void ShowSetupElements()
        {
            disallowUsingKey = true;
            m_panelTransform.anchorMax = new Vector2(0.5f, 0.5f);
            m_panelTransform.anchorMin = new Vector2(0.5f, 0.5f);
            m_panelTransform.sizeDelta = new Vector2(325f, 450f);
            m_shadingObject.SetActive(false);
            m_normalBgObject.SetActive(false);
            m_setupBgObject.SetActive(true);
            PopulatePage("setup");
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

        public void PopulatePageIfSelected(string id)
        {
            if (m_selectedTabId != id)
                return;

            PopulatePage(id);
        }

        public void PopulatePage(string id)
        {
            m_selectedTabId = id;
            ClearPageContents();

            UIElementTab oldTab = m_tabs.prevSelectedTab;
            UIElementTab newTab = m_tabs.selectedTab;
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

            SettingsMenu settingsMenu = ModCache.settingsMenu;
            if (!settingsMenu)
                return;

            switch (id)
            {
                case "setup":
                    populateSetupPage(settingsMenu);
                    break;
                case "Quick setup":
                    populateQuickSetupPage(settingsMenu);
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
                case "Mod-Bot":
                    populateModBotPage(settingsMenu);
                    break;
                case "Advanced":
                    populateAdvancedPage(settingsMenu);
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
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_SSAO), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_SSAO, value, true);
            }, "Ambient occlusion");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_DITHERING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_DITHERING, value, true);
            }, "Dithering");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_VIGNETTE), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_VIGNETTE, value, true);
            }, "Vignette");

            _ = pageBuilder.Header3("Camera effects");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_CAMERA_ROLLING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_ROLLING, value, true);
            }, "Camera rolling");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING, value, true);
            }, "Camera bobbing");

            _ = pageBuilder.Header3("Particle effects");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_NEW_PARTICLES), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_NEW_PARTICLES, value, true);
            }, "Enable particles");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_GARBAGE_PARTICLES), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_GARBAGE_PARTICLES, value, true);
            }, "Enable sparks");

            _ = pageBuilder.Header1("Gameplay");
            _ = pageBuilder.Header3("Fun");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE, value, true);
            }, "First person mode");
            _ = pageBuilder.KeyBind("Camera mode toggle", (KeyCode)ModSettingsManager.GetIntValue(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND), KeyCode.Y, delegate (KeyCode value)
            {
                ModSettingsManager.SetIntValue(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND, (int)value, true);
            });

            _ = pageBuilder.Header1("Game interface");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.SHOW_VERSION_LABEL), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.SHOW_VERSION_LABEL, value, true);
            }, "Show Overhaul mod version");
            _ = pageBuilder.Button("Configure Overhaul mod UIs", delegate
            {
                ModUIConstants.ShowOverhaulUIManagementPanel(base.transform);
            });

            _ = pageBuilder.Button("Done", delegate
            {
                Hide();
                ModSettingsManager.SetBoolValue(ModSettingsConstants.SHOW_MOD_SETUP_SCREEN_ON_START, false);
            });
        }

        private void populateQuickSetupPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Game interface");
            _ = pageBuilder.DropdownWithImage169(ModLocalizationManager.Instance.GetLanguageOptions(false), getCurrentLanguageIndex(), OnLanguageDropdownChanged);
            _ = pageBuilder.Toggle(!settingsMenu.HideGameUIToggle.isOn, OnHideGameUIToggleChanged, "Show game UI");
            _ = pageBuilder.Toggle(settingsMenu.SubtitlesToggle.isOn, OnSubtitlesToggleChanged, "Show subtitles");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.SHOW_VERSION_LABEL), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.SHOW_VERSION_LABEL, value, true);
            }, "Show Overhaul mod version");
            _ = pageBuilder.Button("Configure Overhaul mod UIs", delegate
            {
                ModUIConstants.ShowOverhaulUIManagementPanel(base.transform);
            });

            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE, value, true);
            }, "First person mode");
            _ = pageBuilder.KeyBind("Camera mode toggle", (KeyCode)ModSettingsManager.GetIntValue(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND), KeyCode.Y, delegate (KeyCode value)
            {
                ModSettingsManager.SetIntValue(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND, (int)value, true);
            });
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING, value, true);
            }, "Camera bobbing");

            _ = pageBuilder.Header1("Multiplayer settings");
            _ = pageBuilder.Header3("Preferred region");
            _ = pageBuilder.Dropdown(settingsMenu.RegionDropdown.options, settingsMenu.RegionDropdown.value, OnRegionChanged);
            _ = pageBuilder.Header3("Player");
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
        }

        private void populateGraphicsPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Graphics");
            _ = pageBuilder.Header3("Window");
            _ = pageBuilder.Dropdown(settingsMenu.ScreenResolutionDropDown.options, settingsMenu.ScreenResolutionDropDown.value, OnScreenResolutionChanged);
            _ = pageBuilder.Toggle(settingsMenu.FullScreenToggle.isOn, OnFullScreenChanged, "Fullscreen");
            _ = pageBuilder.Toggle(settingsMenu.VsyncOnToggle.isOn, OnVSyncChanged, "V-Sync");

            if (ModSpecialUtils.SupportsTitleBarOverhaul())
            {
                _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL), delegate (bool value)
                {
                    ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL, value, true);
                }, "Enable title bar changes");
            }

            _ = pageBuilder.Header3("Render");
            _ = pageBuilder.Dropdown(settingsMenu.QualityDropDown.options, settingsMenu.QualityDropDown.value, OnQualityDropdownChanged);
            _ = pageBuilder.Dropdown(settingsMenu.AntiAliasingDropdown.options, settingsMenu.AntiAliasingDropdown.value, OnAntiAliasingDropdownChanged);

            _ = pageBuilder.Header3("Post effects");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_SSAO), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_SSAO, value, true);
            }, "Ambient occlusion");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_DITHERING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_DITHERING, value, true);
            }, "Dithering");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_VIGNETTE), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_VIGNETTE, value, true);
            }, "Vignette");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_BLOOM), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_BLOOM, value, true);
            }, "Bloom");


            _ = pageBuilder.Header1("Camera");
            _ = pageBuilder.Header3("Field of view");
            _ = pageBuilder.Slider(-10f, 25f, false, ModSettingsManager.GetFloatValue(ModSettingsConstants.CAMERA_FOV_OFFSET), delegate (float value)
            {
                ModSettingsManager.SetFloatValue(ModSettingsConstants.CAMERA_FOV_OFFSET, value, true);
            }, true);
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_CAMERA_ROLLING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_ROLLING, value, true);
            }, "Camera rolling");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING, value, true);
            }, "Camera bobbing");
            _ = pageBuilder.Button("Reset camera settings", delegate
            {
                ModSettingsManager.SetFloatValue(ModSettingsConstants.CAMERA_FOV_OFFSET, 0f, true);
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_ROLLING, true, true);
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING, true, true);
                PopulatePage("Graphics");
            });


            _ = pageBuilder.Header1("Environment");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_LIGHTNING_TRANSITION), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_LIGHTNING_TRANSITION, value, true);
            }, "Lightning transitions");
            _ = pageBuilder.Header4("The lightning changes smoothly as level switches");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_ARENA_REMODEL), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_ARENA_REMODEL, value, true);
            }, "Arena remodel");
            _ = pageBuilder.Header4("Made by @water2977");

            _ = pageBuilder.Header1("Voxel engine");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_VOXEL_FIRE_FADING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_VOXEL_FIRE_FADING, value, true);
            }, "Better fire spreading");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_VOXEL_BURNING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_VOXEL_BURNING, value, true);
            }, "Always burn voxels");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_NEW_PARTICLES), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_NEW_PARTICLES, value, true);
            }, "Enable particles");

            _ = pageBuilder.Header1("Garbage");
            _ = pageBuilder.Dropdown(settingsMenu.GarbageSettingsDropdown.options, settingsMenu.GarbageSettingsDropdown.value, OnGarbageSettingsChanged);
            _ = pageBuilder.Toggle(settingsMenu.PlayerPushesGarbageToggle.isOn, OnPlayerPushesGarbageToggleChanged, "Collisions");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_GARBAGE_PARTICLES), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_GARBAGE_PARTICLES, value, true);
            }, "Enable sparks");
        }

        private void populateGameplayPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Gameplay settings");
            _ = pageBuilder.Header3("Difficulty");
            _ = pageBuilder.Dropdown(settingsMenu.StoryModeDifficultyDropDown.options, settingsMenu.StoryModeDifficultyDropDown.value, OnStoryDifficultyIndexChanged);
            _ = pageBuilder.Header4("Change what enemies spawn");

            _ = pageBuilder.Header3("Fun");
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE, value, true);
            }, "First person mode");
            _ = pageBuilder.KeyBind("Camera mode toggle", (KeyCode)ModSettingsManager.GetIntValue(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND), KeyCode.Y, delegate (KeyCode value)
            {
                ModSettingsManager.SetIntValue(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND, (int)value, true);
            });
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_CAMERA_BOBBING, value, true);
            }, "Camera bobbing");

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
            _ = pageBuilder.Toggle(ModSettingsManager.GetBoolValue(ModSettingsConstants.ENABLE_REVERB_FILTER), delegate (bool value)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_REVERB_FILTER, value, true);
            }, "Reverb");
            _ = pageBuilder.Header3("Reverb intensity");
            _ = pageBuilder.Slider(0.1f, 1.5f, false, ModSettingsManager.GetFloatValue(ModSettingsConstants.REVERB_FILTER_INTENSITY), delegate (float value)
            {
                ModSettingsManager.SetFloatValue(ModSettingsConstants.REVERB_FILTER_INTENSITY, value, true);
            });
        }

        private void populateMultiplayerPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
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

            _ = pageBuilder.Header3("Personalization");
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

        private void populateControlsPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
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

        private void populateModBotPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Controls");
            _ = pageBuilder.KeyBind("Open console", ModBotInputManager.GetKeyCode(ModBotInputType.OpenConsole), KeyCode.F1, delegate (KeyCode value)
            {
                ModBotInputManager.InputOptions[0].Key = value;
            });
            _ = pageBuilder.KeyBind("Toggle FPS label", ModBotInputManager.GetKeyCode(ModBotInputType.ToggleFPSLabel), KeyCode.F3, delegate (KeyCode value)
            {
                ModBotInputManager.InputOptions[1].Key = value;
            });

            _ = pageBuilder.Header1("Website integration");
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
                    GameUIRoot.Instance.SettingsMenu.Hide();
                    ModBotUIRoot.Instance.ModBotSignInUI.OpenSignInForm();
                });
            }
        }

        private void populateDefaultPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Page not implemented.");
            _ = pageBuilder.Header2("Try using original menu");
            _ = pageBuilder.Button("Open original settings menu", OnLegacyUIButtonClicked);
        }

        private void populateAdvancedPage(SettingsMenu settingsMenu)
        {
            PageBuilder pageBuilder = new PageBuilder(this);
            _ = pageBuilder.Header1("Advanced settings");
            _ = pageBuilder.Button("Switch save data", null);
            _ = pageBuilder.Button("Reset game settings", null);
            _ = pageBuilder.Button("Reset Overhaul settings", null);
            _ = pageBuilder.Button("Reset ALL settings", null);
            _ = pageBuilder.Header1("First person mode");
            _ = pageBuilder.Header3("Shield transparency");
            _ = pageBuilder.Slider(0f, 1f, false, 1f, null);
            _ = pageBuilder.Toggle(false, null, "Damage indicator");
        }

        private int getCurrentLanguageIndex()
        {
            SettingsManager settingsManager = SettingsManager.Instance;
            if (settingsManager && settingsManager._data != null)
            {
                string langId = settingsManager.GetCurrentLanguageID();

                int index = 0;
                foreach (LanguageDefinition lang in LocalizationManager.Instance.SupportedLanguages)
                {
                    if (lang.LanguageCode == langId)
                        return index;

                    index++;
                }
            }
            return -1;
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI && GameModeManager.IsOnTitleScreen())
            {
                Hide();
                titleScreenUI.OnOptionsButtonClicked();
                return;
            }

            SettingsMenu settingsMenu = ModCache.gameUIRoot.SettingsMenu;
            if (settingsMenu)
            {
                Hide();

                ModActionUtils.DoInFrame(delegate
                {
                    settingsMenu.Show();
                });
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

        public void OnLanguageDropdownChanged(int value)
        {
            LocalizationManager localizationManager = LocalizationManager.Instance;
            if (localizationManager)
            {
                localizationManager.SetCurrentLanguage(localizationManager.SupportedLanguages[value].LanguageCode);
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

        private static void onSignedOut(JsonObject jsonObject)
        {
            ModBotUIRoot.Instance.ModBotSignInUI.SetSession("");
            VersionLabelManager.Instance.SetLine(2, "Not signed in");

            UISettingsMenuRework settingsMenuRework = ModUIManager.Instance.Get<UISettingsMenuRework>(AssetBundleConstants.UI, ModUIConstants.UI_SETTINGS_MENU);
            if (settingsMenuRework)
                settingsMenuRework.PopulatePage("Mod-Bot");
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

            public Dropdown DropdownWithImage169(List<Dropdown.OptionData> list, int value, UnityAction<int> callback)
            {
                return instantiateDropdown(list, value, callback, SettingsMenu.DropdownWithImage169Prefab);
            }

            public Slider Slider(float min, float max, bool wholeNumbers, float value, UnityAction<float> callback, bool noBetterSlider = false)
            {
                Slider slider = Instantiate(SettingsMenu.SliderPrefab, SettingsMenu.PageContentsTransform);
                slider.gameObject.SetActive(true);
                slider.minValue = min;
                slider.maxValue = max;
                slider.wholeNumbers = wholeNumbers;
                slider.value = value;
                if (callback != null)
                    slider.onValueChanged.AddListener(callback);

                if (!noBetterSlider)
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

            public UIElementKeyBindSetter KeyBind(string name, KeyCode keyCode, KeyCode defaultKey, Action<KeyCode> onChanged)
            {
                _ = Header3(name);

                ModdedObject moddedObject = Instantiate(SettingsMenu.KeyBindPrefab, SettingsMenu.PageContentsTransform);
                moddedObject.gameObject.SetActive(true);

                UIElementKeyBindSetter elementKeyBind = moddedObject.gameObject.AddComponent<UIElementKeyBindSetter>();
                elementKeyBind.InitializeElement();
                elementKeyBind.key = keyCode;
                elementKeyBind.defaultKey = defaultKey;
                elementKeyBind.onValueChanged.AddListener(new UnityAction<KeyCode>(onChanged));

                return elementKeyBind;
            }

            public void Dispose()
            {
                SettingsMenu = null;
            }
        }
    }
}
