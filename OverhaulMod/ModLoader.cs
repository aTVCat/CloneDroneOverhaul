﻿using LevelEditorPatch;
using OverhaulMod.Combat;
using OverhaulMod.Content;
using OverhaulMod.Content.LevelEditor;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
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

            ModBuildInfo.Load();

            Physics.reuseCollisionCallbacks = true;
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
            if (ModManagers.Instance)
            {
                UnityEngine.Object.Destroy(ModManagers.Instance.gameObject);
            }
        }

        private static void addManagers()
        {
            _ = ModManagers.New<ModSettingsManager>();
            _ = ModManagers.New<ModDataManager>();
            _ = ModManagers.New<RepositoryManager>();
            _ = ModManagers.New<ExclusiveContentManager>();
            _ = ModManagers.New<ModLocalizationManager>();
            _ = ModManagers.New<ModAudioManager>();
            _ = ModManagers.New<ModResources>();
            _ = ModManagers.New<ModUIManager>();
            _ = ModManagers.New<ModLevelManager>();
            _ = ModManagers.New<ModWeaponsManager>();
            _ = ModManagers.New<ModUpgradesManager>();
            _ = ModManagers.New<ModEnemiesManager>();
            _ = ModManagers.New<DifficultyTierManager>();
            _ = ModManagers.New<ModGameModifiersManager>();
            _ = ModManagers.New<TitleScreenCustomizationManager>();

            _ = ModManagers.New<ModWebhookManager>();
            _ = ModManagers.New<ExclusiveContentManager>();
            _ = ModManagers.New<ContentManager>();
            _ = ModManagers.New<UpdateManager>();
            _ = ModManagers.New<NewsManager>();
            _ = ModManagers.New<PersonalizationManager>();
            _ = ModManagers.New<PersonalizationEditorManager>();
            _ = ModManagers.New<PersonalizationEditorObjectManager>();
            _ = ModManagers.New<PersonalizationItemVerificationManager>();

            _ = ModManagers.New<WeatherManager>();
            _ = ModManagers.New<FloatingDustManager>();
            _ = ModManagers.New<FadingVoxelManager>();

            _ = ModManagers.New<ArenaRemodelManager>();
            _ = ModManagers.New<ArenaAudienceManager>();
            _ = ModManagers.New<LightningTransitionManager>();
            _ = ModManagers.New<AdvancedPhotoModeManager>();
            _ = ModManagers.New<UpgradeModesManager>();
            _ = ModManagers.New<TransitionManager>();
            _ = ModManagers.New<CameraManager>();
            _ = ModManagers.New<RichPresenceManager>();
            _ = ModManagers.New<PooledPrefabManager>();
            _ = ModManagers.New<RealisticLightningManager>();
            _ = ModManagers.New<ParticleManager>();
            _ = ModManagers.New<PostEffectsManager>();
            _ = ModManagers.New<QuickResetManager>();
            _ = ModManagers.New<AutoBuildManager>();

            _ = ModManagers.New<ModPhysicsManager>();
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
                Patch.AddObject("WeatherSettingsOverride", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform, new Type[] { typeof(LevelEditorWeatherSettingsOverride) }, ModCore.editorTexturesFolder + "WeatherSettingsOverride.png");

                if (ModBuildInfo.debug)
                    Patch.AddObject("ArenaAudienceLinePoint", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Sphere).transform, new Type[] { typeof(ArenaAudienceLinePoint) }, null);

                Patch.AddObject("Scythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scythe1Spawner, null, ModCore.editorTexturesFolder + "ScytheBot.png");
                Patch.AddObject("Scythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scythe2Spawner, null, ModCore.editorTexturesFolder + "ScytheBot.png");
                Patch.AddObject("Scythe3", "OverhaulMod", "Enemies", ModPrefabUtils.scythe3Spawner, null, ModCore.editorTexturesFolder + "ScytheBot.png");
                Patch.AddObject("Scythe4", "OverhaulMod", "Enemies", ModPrefabUtils.scythe4Spawner, null, ModCore.editorTexturesFolder + "ScytheBot.png");
                Patch.AddObject("SprinterScythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter1Spawner, null, ModCore.editorTexturesFolder + "SprinterScytheBot.png");
                Patch.AddObject("SprinterScythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter2Spawner, null, ModCore.editorTexturesFolder + "SprinterScytheBot.png");

                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.AllNewEnemies))
                {
                    Patch.AddObject("Axe1", "OverhaulMod", "Enemies", ModPrefabUtils.axe1Spawner, null, ModCore.editorTexturesFolder + "AxeBot.png");
                    Patch.AddObject("Axe2", "OverhaulMod", "Enemies", ModPrefabUtils.axe2Spawner, null, ModCore.editorTexturesFolder + "AxeBot.png");
                    Patch.AddObject("Axe3", "OverhaulMod", "Enemies", ModPrefabUtils.axe3Spawner, null, ModCore.editorTexturesFolder + "AxeBot.png");
                    Patch.AddObject("Axe4", "OverhaulMod", "Enemies", ModPrefabUtils.axe4Spawner, null, ModCore.editorTexturesFolder + "AxeBot.png");

                    Patch.AddObject("Halberd1", "OverhaulMod", "Enemies", ModPrefabUtils.halberd1Spawner, null, ModCore.editorTexturesFolder + "HalberdBot.png");
                    Patch.AddObject("Halberd2", "OverhaulMod", "Enemies", ModPrefabUtils.halberd2Spawner, null, ModCore.editorTexturesFolder + "HalberdBot.png");
                    Patch.AddObject("Halberd3", "OverhaulMod", "Enemies", ModPrefabUtils.halberd3Spawner, null, ModCore.editorTexturesFolder + "HalberdBot.png");

                    Patch.AddObject("GuardBot", "OverhaulMod", "Enemies", ModPrefabUtils.guardBotSpawner, null, null);

                    Patch.AddObject("ChibiSword2", "OverhaulMod", "Enemies", ModPrefabUtils.chibiSword2Spawner, null, null);
                }
                s_hasAddedObjects = true;
            }
        }

        private static void createDirectories()
        {
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
            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                ModSpecialUtils.SetTitleBarStateDependingOnSettings();
            }, ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                PostEffectsManager.Instance.RefreshCameraPostEffects();
            }, ModSettingsConstants.ENABLE_SSAO);

            modSettingsManager.AddSettingValueChangedListener(delegate (object obj)
            {
                PostEffectsManager.Instance.RefreshCameraPostEffects();
            }, ModSettingsConstants.ENABLE_BLOOM);
        }

        public static bool HasToLoad()
        {
            return !ModManagers.Instance;
        }
    }
}
