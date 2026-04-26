using OverhaulMod.Content;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Patches.Behaviours;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using OverhaulMod.Visuals.Environment;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod
{
    public static class ModLoader
    {
        private static GameObject _lock;

        public static bool ShouldStartCustomizationEditor;

        public static void Load(bool gameFlowStartedThisFrame)
        {
            ModDebug.Log($"Trying to load the mod. GameFlowManager started this frame? {gameFlowStartedThisFrame}, Lock: {_lock}");
            if (_lock) return;

            if (gameFlowStartedThisFrame)
            {
                GlobalEventManager.Instance.AddEventListenerOnce(GlobalEvents.GameInitializtionCompleted, onGameInitialized);
            }

            if (ModManagers.Instance)
            {
                GamePatchBehaviour.Load();

                // for cases when transition doesnt end automatically for some reason
                DelegateScheduler.Instance.Schedule(delegate
                {
                    if (!LevelManager.Instance.IsSpawningCurrentLevel())
                        TransitionManager.Instance.EndTransition();
                }, 1f);

                if (gameFlowStartedThisFrame) ModManagers.Instance.TriggerModLoadedEvent();
                return;
            }

            GameObject sceneObject = new GameObject("Overhaul Lock Object");
            _lock = sceneObject;

            Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(ModDirectories.ModFolder, "DiscordWebhook.dll")));

            ModBuild.Load();
            ModFeatures.CacheValues();
            ModLaunchOptions.Initialize();
            ModUserInfo.Load();
            ModIntegrationUtils.Load();
            ModDirectories.CreateMissingDirectories();

            loadGameUIThemeData();
            addManagers();

            loadMiscellaneousAssets();
            addSettingsListeners();

            QualitySettings.softParticles = true;
            FPSManager.RefreshFPSCap();
            ModSpecialUtils.SetTitleBarStateDependingOnSettings();
            GamePatchBehaviour.Load();

            if (GameModeManager.IsOnTitleScreen()) _ = ModUIConstants.ShowTitleScreenReworkIfHaventBefore();

            ModManagers.Instance.TriggerModLoadedEvent();
        }

        public static void Unload()
        {
            Object.Destroy(_lock);

            tryShowVanillaPauseMenu();

            ModSpecialUtils.SetTitleBarStateDependingOnSettings();

            GamePatchBehaviour.Unload();

            ModManagers modManagers = ModManagers.Instance;
            if (modManagers && modManagers.gameObject)
            {
                Object.Destroy(modManagers.gameObject);
            }
        }

        private static void addManagers()
        {
            GameObject managersObject = new GameObject("Overhaul Mod Managers");
            ModManagers modManagers = managersObject.AddComponent<ModManagers>();
            Object.DontDestroyOnLoad(modManagers);

            GameObject coreManagers = new GameObject("Core");
            coreManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<ModResources>(coreManagers);
            modManagers.AddSingleton<ModDataManager>(coreManagers);
            modManagers.AddSingleton<ComponentCacheManager>(coreManagers);
            modManagers.AddSingleton<ModSettingsDataManager>(coreManagers);
            modManagers.AddSingleton<ModSettingsManager>(coreManagers);
            modManagers.AddSingleton<ModAudioManager>(coreManagers);
            modManagers.AddSingleton<ModAudioLibrary>(coreManagers);
            modManagers.AddSingleton<ModTime>(coreManagers);
            modManagers.AddSingleton<ModPhysicsManager>(coreManagers);
            modManagers.AddSingleton<ModUIManager>(coreManagers);
            modManagers.AddSingleton<CameraManager>(coreManagers);
            modManagers.AddSingleton<PooledPrefabManager>(coreManagers);
            modManagers.AddSingleton<ScheduledActionsManager>(coreManagers);
            modManagers.AddSingleton<ModLocalizationManager>(coreManagers);
            modManagers.AddSingleton<TransitionManager>(coreManagers);

            GameObject gameplayManagers = new GameObject("Gameplay");
            gameplayManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<ModUpgradesManager>(gameplayManagers);
            modManagers.AddSingleton<ModWeaponsManager>(gameplayManagers);
            modManagers.AddSingleton<ModLevelManager>(gameplayManagers);
            modManagers.AddSingleton<ModCharacterManager>(gameplayManagers);
            modManagers.AddSingleton<ModGameModifiersManager>(gameplayManagers);
            modManagers.AddSingleton<UpgradeModesManager>(gameplayManagers);
            modManagers.AddSingleton<AutoBuildManager>(gameplayManagers);

            GameObject visualManagers = new GameObject("Visuals");
            visualManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<QualityManager>(visualManagers);
            modManagers.AddSingleton<FPSManager>(visualManagers);
            modManagers.AddSingleton<PostEffectsManager>(visualManagers);
            modManagers.AddSingleton<ParticleManager>(visualManagers);
            modManagers.AddSingleton<VoxelFadingManager>(visualManagers);

            GameObject environmentManagers = new GameObject("Environment");
            environmentManagers.transform.SetParent(visualManagers.transform, false);
            modManagers.AddSingleton<AdditionalSkyboxesManager>(environmentManagers);
            modManagers.AddSingleton<RealisticLightingManager>(environmentManagers);
            modManagers.AddSingleton<LightingTransitionManager>(environmentManagers);
            modManagers.AddSingleton<FloatingDustManager>(environmentManagers);
            modManagers.AddSingleton<WeatherManager>(environmentManagers);
            modManagers.AddSingleton<ArenaRemodelManager>(environmentManagers);
            modManagers.AddSingleton<ArenaAudienceManager>(environmentManagers);

            GameObject contentManagers = new GameObject("Content");
            contentManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<RepositoryManager>(contentManagers);
            modManagers.AddSingleton<ModDownloadCacheManager>(contentManagers);
            modManagers.AddSingleton<GoogleDriveManager>(contentManagers);
            modManagers.AddSingleton<PostmanManager>(contentManagers);
            modManagers.AddSingleton<ExclusivePerkManager>(contentManagers);
            modManagers.AddSingleton<AddonManager>(contentManagers);
            modManagers.AddSingleton<UpdateManager>(contentManagers);
            modManagers.AddSingleton<NewsManager>(contentManagers);

            GameObject personalizationManagers = new GameObject("Customization");
            personalizationManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<TitleScreenCustomizationManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationCacheManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorDataManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorObjectManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorGuideManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorTemplateManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorScreenshotManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorClipboard>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationItemVerificationManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationMultiplayerManager>(personalizationManagers);

            GameObject miscManagers = new GameObject("Misc.");
            miscManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<AdvancedPhotoModeManager>(miscManagers);
            modManagers.AddSingleton<UseKeyTriggerManager>(miscManagers);
            modManagers.AddSingleton<RichPresenceManager>(miscManagers);
            modManagers.AddSingleton<CharacterUpdateScheduler>(miscManagers);
        }

        private static void loadMiscellaneousAssets()
        {
            ModConstants.CursorSkinOptions[1].image = ModUnityUtils.ToSprite(ModResources.Texture2D(AssetBundleConstants.UI, "Cursor"));
            ModConstants.CursorSkinOptions[2].image = ModUnityUtils.ToSprite(ModResources.Texture2D(AssetBundleConstants.UI, "Cursor2"));
        }

        private static void addSettingsListeners()
        {
            ModSettingsManager modSettingsManager = ModSettingsManager.Instance;
            modSettingsManager.AddSettingValueChangedListener(refreshEditorAmbiance, ModSettingsConstants.CUSTOMIZATION_EDITOR_AMBIANCE);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_SSAO);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.SSAO_INTENSITY);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.SSAO_SAMPLE_COUNT);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.CHROMATIC_ABERRATION_INTENSITY);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.CHROMATIC_ABERRATION_ON_SCREEN_EDGES);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.COLOR_BLINDNESS_MODE);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.COLOR_BLINDNESS_AFFECT_UI);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_DOF);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.BLOOM_MODE);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_DITHERING);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_VIGNETTE);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_SUN_SHAFTS);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION);
            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                refreshCameraPostEffects(obj);
                GlobalEventManager.Instance.Dispatch(CameraManager.FIRST_PERSON_MODE_SWITCHED_EVENT);
            }, ModSettingsConstants.ENABLE_FIRST_PERSON_MODE);
            modSettingsManager.AddSettingValueChangedListener(refreshFPSCap, ModSettingsConstants.FPS_CAP);
            modSettingsManager.AddSettingValueChangedListener(delegate
            {
                ModUIManager.RefreshCursor();
            }, ModSettingsConstants.CURSOR_SKIN);
            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                UseKeyTriggerManager manager = UseKeyTriggerManager.Instance;
                if (manager && obj is bool boolVal)
                {
                    if (!boolVal)
                        manager.HideDescription();

                    foreach (LevelEditorUseButtonTrigger trigger in Resources.FindObjectsOfTypeAll<LevelEditorUseButtonTrigger>())
                    {
                        if (boolVal)
                        {
                            if (trigger._keyboardHint)
                            {
                                trigger._keyboardHint.Show(string.Empty);
                                UseKeyTriggerManager.SetFramingBoxSelectedColor(trigger._keyboardHint.transform, false);
                            }

                            UseKeyTriggerManager.Instance.SetNearestTriggerNull();
                        }
                        else
                        {
                            if (trigger._keyboardHint)
                                UseKeyTriggerManager.SetFramingBoxSelectedColor(trigger._keyboardHint.transform, true);

                            trigger.destroyKeyboardHint();
                        }
                    }
                }
            }, ModSettingsConstants.ENABLE_PRESS_BUTTON_TRIGGER_DESCRIPTION_REWORK);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                ModActionUtils.DoInFrame(delegate
                {
                    EnergyBarPatchBehaviour energyUIPatch = GamePatchBehaviour.GetBehaviour<EnergyBarPatchBehaviour>();
                    if (energyUIPatch)
                        energyUIPatch.PatchEnergyUI();
                });
            }, ModSettingsConstants.ENERGY_UI_REWORK);

            modSettingsManager.AddSettingValueChangedListener(delegate
            {
                CloneDroneLogoParticlesBehaviour particlesPatch = GamePatchBehaviour.GetBehaviour<CloneDroneLogoParticlesBehaviour>();
                if (particlesPatch)
                    particlesPatch.RefreshVisibility();
            }, ModSettingsConstants.CLONE_DRONE_LOGO_FIRE);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                if (obj is bool boolVal && !boolVal)
                {
                    FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                    if (player && player._cameraMover)
                    {
                        player._cameraMover.ShortenedDistanceAddition = CameraFOVController.DEFAULT_SHORTENED_DISTANCE_ADDITION;
                    }
                }
            }, ModSettingsConstants.ENABLE_FOV_OVERRIDE);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                UITitleScreenRework titleScreenRework = ModUIManager.Instance.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN);
                if (titleScreenRework) titleScreenRework.RefreshPosition();
            }, ModSettingsConstants.TITLE_SCREEN_PANEL_POSITION);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                UITitleScreenRework titleScreenRework = ModUIManager.Instance.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN);
                if (titleScreenRework) titleScreenRework.RefreshFade();
            }, ModSettingsConstants.TITLE_SCREEN_BACKGROUND_FADE_POWER);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                ChunkUpdateDelayPatchBehaviour chunkUpdateDelayPatch = GamePatchBehaviour.GetBehaviour<ChunkUpdateDelayPatchBehaviour>();
                if (chunkUpdateDelayPatch)
                    chunkUpdateDelayPatch.Refresh();
            }, ModSettingsConstants.CHUNK_UPDATE_DELAY);
        }

        private static void loadGameUIThemeData()
        {
            if (ModCache.UIThemeData) return;

            GameUIThemeData gameUIThemeData = null;
            foreach (SelectableUI selectableUi in Resources.FindObjectsOfTypeAll<SelectableUI>())
                if (selectableUi.GameThemeData && selectableUi.GameThemeData.SelectionCornerPrefab)
                {
                    foreach (Graphic graphic in selectableUi.GameThemeData.SelectionCornerPrefab.GetComponentsInChildren<Graphic>(true))
                        graphic.raycastTarget = false;

                    gameUIThemeData = selectableUi.GameThemeData;
                    break;
                }

            if (gameUIThemeData)
            {
                gameUIThemeData.ButtonBackground[0].Color = new Color(0.19f, 0.37f, 0.88f, 1);
                gameUIThemeData.ButtonBackground[1].Color = new Color(0.3f, 0.5f, 1, 1f);
                gameUIThemeData.ButtonTextOutline[0].Color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
                gameUIThemeData.ButtonTextOutline[1].Color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
                ModCache.UIThemeData = gameUIThemeData;
            }
        }

        private static void onGameInitialized()
        {
            ModManagers.Instance.TriggerGameLoadedEvent();
            if (ShouldStartCustomizationEditor)
            {
                ShouldStartCustomizationEditor = false;
                if (PersonalizationEditorManager.Instance)
                    PersonalizationEditorManager.Instance.StartEditorGameMode(true);
            }
        }

        private static void tryShowVanillaPauseMenu()
        {
            GameUIRoot uiRoot = ModCache.UIRoot;
            if (!uiRoot) return;

            ModUIManager modUIManager = ModUIManager.Instance;
            if (!modUIManager || !modUIManager.IsUIVisible(AssetBundleConstants.UI, ModUIConstants.UI_PAUSE_MENU)) return;

            _ = modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_PAUSE_MENU);
            if (uiRoot.EscMenu) uiRoot.EscMenu.Show();
        }

        private static void refreshCameraPostEffects(object obj)
        {
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        private static void refreshFPSCap(object obj)
        {
            FPSManager.RefreshFPSCap();
        }

        private static void refreshEditorAmbiance(object obj)
        {
            ModAudioManager.Instance.PlayOrStopCustomizationEditorAmbiance();
        }
    }
}