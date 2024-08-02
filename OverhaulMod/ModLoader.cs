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

namespace OverhaulMod
{
    public static class ModLoader
    {
        private static bool s_hasAddedObjects;

        public static void Load()
        {
            if (!HasToLoad())
            {
                ModManagers.Instance.TriggerModLoadedEvent();
                return;
            }

            loadAssemblies();
            loadGameUIThemeData();

            ModBuildInfo.Load();
            ModFeatures.CacheValues();

            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadBufferSize = 16;
            QualitySettings.asyncUploadPersistentBuffer = true;
            QualitySettings.softParticles = true;

            GameObject gameObject = new GameObject("OverhaulManagers", new Type[] { typeof(ModManagers) });
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            createDirectories();
            addManagers();
            ModManagers.Instance.TriggerModLoadedEvent();

            addLevelEditorObjects();
            addListeners();
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
            _ = ModManagers.NewSingleton<ModSettingsDataManager>();
            _ = ModManagers.NewSingleton<ModSettingsManager>();
            _ = ModManagers.NewSingleton<ModDataManager>();
            _ = ModManagers.NewSingleton<RepositoryManager>();
            _ = ModManagers.NewSingleton<GoogleDriveManager>();
            _ = ModManagers.NewSingleton<ExclusiveContentManager>();
            _ = ModManagers.NewSingleton<ModLocalizationManager>();
            _ = ModManagers.NewSingleton<ModAudioManager>();
            _ = ModManagers.NewSingleton<ModResources>();
            _ = ModManagers.NewSingleton<ModUIManager>();
            _ = ModManagers.NewSingleton<ModLevelManager>();
            _ = ModManagers.NewSingleton<ModWeaponsManager>();
            _ = ModManagers.NewSingleton<ModUpgradesManager>();
            _ = ModManagers.NewSingleton<ModEnemiesManager>();
            _ = ModManagers.NewSingleton<DifficultyTierManager>();
            _ = ModManagers.NewSingleton<ModGameModifiersManager>();
            _ = ModManagers.NewSingleton<TitleScreenCustomizationManager>();

            _ = ModManagers.NewSingleton<ModWebhookManager>();
            _ = ModManagers.NewSingleton<ExclusiveContentManager>();
            _ = ModManagers.NewSingleton<ContentManager>();
            _ = ModManagers.NewSingleton<UpdateManager>();
            _ = ModManagers.NewSingleton<NewsManager>();
            _ = ModManagers.NewSingleton<PersonalizationCacheManager>();
            _ = ModManagers.NewSingleton<PersonalizationManager>();
            _ = ModManagers.NewSingleton<PersonalizationEditorManager>();
            _ = ModManagers.NewSingleton<PersonalizationEditorObjectManager>();
            _ = ModManagers.NewSingleton<PersonalizationEditorCopyPasteManager>();
            _ = ModManagers.NewSingleton<PersonalizationItemVerificationManager>();
            _ = ModManagers.NewSingleton<PersonalizationEditorGuideManager>();
            _ = ModManagers.NewSingleton<PersonalizationMultiplayerManager>();
            _ = ModManagers.NewSingleton<PersonalizationEditorTemplateManager>();

            _ = ModManagers.NewSingleton<WeatherManager>();
            _ = ModManagers.NewSingleton<FloatingDustManager>();
            _ = ModManagers.NewSingleton<FadingVoxelManager>();

            _ = ModManagers.NewSingleton<ArenaRemodelManager>();
            _ = ModManagers.NewSingleton<ArenaAudienceManager>();
            _ = ModManagers.NewSingleton<LightingTransitionManager>();
            _ = ModManagers.NewSingleton<AdvancedPhotoModeManager>();
            _ = ModManagers.NewSingleton<UpgradeModesManager>();
            _ = ModManagers.NewSingleton<TransitionManager>();
            _ = ModManagers.NewSingleton<CameraManager>();
            _ = ModManagers.NewSingleton<RichPresenceManager>();
            _ = ModManagers.NewSingleton<PooledPrefabManager>();
            _ = ModManagers.NewSingleton<RealisticLightingManager>();
            _ = ModManagers.NewSingleton<ParticleManager>();
            _ = ModManagers.NewSingleton<PostEffectsManager>();
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.QuickReset)) _ = ModManagers.NewSingleton<QuickResetManager>();
            _ = ModManagers.NewSingleton<AutoBuildManager>();

            _ = ModManagers.NewSingleton<ModPhysicsManager>();
        }

        private static void loadAssemblies()
        {
            LevelEditorPatch.Patch.Apply();
            _ = ModBotAPI.ModBotAPI.Initialize();
            ModIntegrationUtils.Load();
        }

        private static void addLevelEditorObjects()
        {
            if (!s_hasAddedObjects)
            {
                Patch.AddObject("WeatherSettingsOverride", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform, new Type[] { typeof(LevelEditorWeatherSettingsOverride) }, Path.Combine(ModCore.editorTexturesFolder, "WeatherSettingsOverride.png"));

                if (ModBuildInfo.debug)
                    Patch.AddObject("ArenaAudienceLinePoint", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Sphere).transform, new Type[] { typeof(ArenaAudienceLinePoint) }, null);

                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.NewEnemies))
                {
                    Patch.AddObject("Scythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scythe1Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "ScytheBot.png"));
                    Patch.AddObject("Scythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scythe2Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "ScytheBot.png"));
                    Patch.AddObject("Scythe3", "OverhaulMod", "Enemies", ModPrefabUtils.scythe3Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "ScytheBot.png"));
                    Patch.AddObject("Scythe4", "OverhaulMod", "Enemies", ModPrefabUtils.scythe4Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "ScytheBot.png"));
                    Patch.AddObject("SprinterScythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter1Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "SprinterScytheBot.png"));
                    Patch.AddObject("SprinterScythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter2Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "SprinterScytheBot.png"));

                    Patch.AddObject("Axe1", "OverhaulMod", "Enemies", ModPrefabUtils.axe1Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "AxeBot.png"));
                    Patch.AddObject("Axe2", "OverhaulMod", "Enemies", ModPrefabUtils.axe2Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "AxeBot.png"));
                    Patch.AddObject("Axe3", "OverhaulMod", "Enemies", ModPrefabUtils.axe3Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "AxeBot.png"));
                    Patch.AddObject("Axe4", "OverhaulMod", "Enemies", ModPrefabUtils.axe4Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "AxeBot.png"));

                    Patch.AddObject("Halberd1", "OverhaulMod", "Enemies", ModPrefabUtils.halberd1Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "HalberdBot.png"));
                    Patch.AddObject("Halberd2", "OverhaulMod", "Enemies", ModPrefabUtils.halberd2Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "HalberdBot.png"));
                    Patch.AddObject("Halberd3", "OverhaulMod", "Enemies", ModPrefabUtils.halberd3Spawner, null, Path.Combine(ModCore.editorTexturesFolder, "HalberdBot.png"));

                    Patch.AddObject("GuardBot", "OverhaulMod", "Enemies", ModPrefabUtils.guardBotSpawner, null, null);

                    Patch.AddObject("ChibiSword2", "OverhaulMod", "Enemies", ModPrefabUtils.chibiSword2Spawner, null, null);
                }
                s_hasAddedObjects = true;
            }
        }

        private static void createDirectories()
        {
            _ = ModIOUtils.CreateDirectoryIfNotExists(ModCore.modDataFolder);
            _ = ModIOUtils.CreateDirectoryIfNotExists(ModCore.contentFolder);
            _ = ModIOUtils.CreateDirectoryIfNotExists(ModCore.savesFolder);
            _ = ModIOUtils.CreateDirectoryIfNotExists(ModCore.addonsFolder);
            _ = ModIOUtils.CreateDirectoryIfNotExists(ModCore.customizationFolder);
            _ = ModIOUtils.CreateDirectoryIfNotExists(ModCore.customizationPersistentFolder);

            if (!Directory.Exists(ModCore.modDataFolder))
                _ = Directory.CreateDirectory(ModCore.modDataFolder);

            if (!Directory.Exists(ModCore.contentFolder))
                _ = Directory.CreateDirectory(ModCore.contentFolder);

            if (!Directory.Exists(ModCore.savesFolder))
                _ = Directory.CreateDirectory(ModCore.savesFolder);

            if (!Directory.Exists(ModCore.addonsFolder))
                _ = Directory.CreateDirectory(ModCore.addonsFolder);

            if (!Directory.Exists(ModCore.customizationFolder))
                _ = Directory.CreateDirectory(ModCore.customizationFolder);
        }

        private static void addListeners()
        {
            ModSettingsManager modSettingsManager = ModSettingsManager.Instance;
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_SSAO);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.SSAO_INTENSITY);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.SSAO_SAMPLE_COUNT);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.CHROMATIC_ABERRATION_INTENSITY);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.CHROMATIC_ABERRATION_ON_SCREEN_EDGES);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.COLOR_BLINDNESS_MODE);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.COLOR_BLINDNESS_AFFECT_UI);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.ENABLE_BLOOM);
            modSettingsManager.AddSettingValueChangedListener(refreshCameraPostEffects, ModSettingsConstants.TWEAK_BLOOM);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                ModActionUtils.DoInFrame(delegate
                {
                    EnergyUIPatchBehaviour energyUIPatch = GamePatchBehaviour.GetBehaviour<EnergyUIPatchBehaviour>();
                    if (energyUIPatch)
                        energyUIPatch.RefreshPatch();
                });
            }, ModSettingsConstants.ENERGY_UI_REWORK);
        }

        private static void refreshCameraPostEffects(object obj)
        {
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        private static void loadGameUIThemeData()
        {
            if (ModCache.gameUIThemeData)
                return;

            GameUIThemeData gameUIThemeData = null;
            foreach (SelectableUI selectableUi in Resources.FindObjectsOfTypeAll<SelectableUI>())
                if (selectableUi.GameThemeData)
                {
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

        public static bool HasToLoad()
        {
            return !ModManagers.Instance;
        }
    }
}
