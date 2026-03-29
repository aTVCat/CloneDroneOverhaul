using LevelEditorPatch;
using OverhaulMod.Combat;
using OverhaulMod.Content;
using OverhaulMod.Content.LevelEditor;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Patches.Behaviours;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using OverhaulMod.Visuals.Environment;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod
{
    public static class ModLoader
    {
        private static bool s_hasAddedObjects;

        public static void Load()
        {
            ModDebug.Log("Attempted to load the mod");
            if (ModManagers.Instance)
            {
                ModManagers.Instance.TriggerModLoadedEvent();
                return;
            }

            loadAssemblies();
            createDirectories();
            loadGameUIThemeData();

            ModBuild.Load();
            ModFeatures.CacheValues();
            ModLaunchOptions.Initialize();
            ModUserInfo.Load();

            addManagers();

            loadMiscellaneousAssets();
            addLevelEditorObjects();
            addListeners();

            QualitySettings.softParticles = true;
            FPSManager.RefreshFPSCap();

            ModCore.RefreshCursor();

            ModManagers.Instance.TriggerModLoadedEvent();
        }

        public static void Unload()
        {
            ModManagers modManagers = ModManagers.Instance;
            if (modManagers && modManagers.gameObject)
            {
                UnityEngine.Object.Destroy(modManagers.gameObject);
            }
        }

        private static void addManagers()
        {
            GameObject managersObject = new GameObject("Overhaul Mod Managers");
            ModManagers modManagers = managersObject.AddComponent<ModManagers>();
            UnityEngine.Object.DontDestroyOnLoad(modManagers);

            GameObject coreManagers = new GameObject("Core");
            coreManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<ModResources>(coreManagers);
            modManagers.AddSingleton<ModDataManager>(coreManagers);
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
            modManagers.AddSingleton<DifficultyTierManager>(gameplayManagers);
            modManagers.AddSingleton<ModGameModifiersManager>(gameplayManagers);
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.RevertUpgrades)) modManagers.AddSingleton<UpgradeModesManager>(gameplayManagers);
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
            modManagers.AddSingleton<PersonalizationEditorManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorObjectManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorCopyPasteManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorGuideManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorTemplateManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationEditorScreenshotManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationItemVerificationManager>(personalizationManagers);
            modManagers.AddSingleton<PersonalizationMultiplayerManager>(personalizationManagers);

            GameObject miscManagers = new GameObject("Misc.");
            miscManagers.transform.SetParent(managersObject.transform, false);
            modManagers.AddSingleton<AdvancedPhotoModeManager>(miscManagers);
            modManagers.AddSingleton<UseKeyTriggerManager>(miscManagers);
            modManagers.AddSingleton<RichPresenceManager>(miscManagers);
        }

        private static void loadAssemblies()
        {
            Patch.Apply();
            ModIntegrationUtils.Load();
        }

        private static void loadMiscellaneousAssets()
        {
            ModConstants.CursorSkinOptions[1].image = ModUnityUtils.ToSprite(ModResources.Texture2D(AssetBundleConstants.UI, "Cursor"));
            ModConstants.CursorSkinOptions[2].image = ModUnityUtils.ToSprite(ModResources.Texture2D(AssetBundleConstants.UI, "Cursor2"));
        }

        private static void addLevelEditorObjects()
        {
            if (!s_hasAddedObjects)
            {
                Patch.AddObject("WeatherSettingsOverride", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform, new Type[] { typeof(LevelEditorWeatherSettingsOverride) }, Path.Combine(ModCore.EditorTexturesFolder, "WeatherSettingsOverride.png"));

                /*
                if (ModBuild.IsDebugBuild)
                    Patch.AddObject("ArenaAudienceLinePoint", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Sphere).transform, new Type[] { typeof(ArenaAudienceLinePoint) }, null);*/

                s_hasAddedObjects = true;
            }
        }

        private static void createDirectories()
        {
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.ModUserDataFolder);
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.ContentFolder);
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.SavesFolder);
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.AddonsFolder);
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.CustomizationFolder);
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.CustomizationPersistentFolder);
            _ = ModFileUtils.CreateDirectoryIfNotExists(ModCore.DeveloperFolder);
        }

        private static void addListeners()
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
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.BLOO_MODE);
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
            modSettingsManager.AddSettingValueChangedListener(ModCore.RefreshCursor, ModSettingsConstants.CURSOR_SKIN);
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

        private static void loadGameUIThemeData()
        {
            if (ModCache.gameUIThemeData) return;

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
                ModCache.gameUIThemeData = gameUIThemeData;
            }
        }
    }
}