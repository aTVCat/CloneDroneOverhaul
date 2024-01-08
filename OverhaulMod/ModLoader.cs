using LevelEditorPatch;
using OverhaulMod.Combat;
using OverhaulMod.Content;
using OverhaulMod.Content.LevelEditor;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals.Environment;
using System;
using UnityEngine;

namespace OverhaulMod
{
    public static class ModLoader
    {
        public static void Load()
        {
            if (!HasToLoad())
            {
                ModManagers.Instance.TriggerModLoadedEvent();
                return;
            }

            loadAssemblies();

            ModBuildInfo.Load();

            GameObject gameObject = new GameObject("OverhaulManagers", new Type[] { typeof(ModManagers) });
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            addManagers();
            ModManagers.Instance.TriggerModLoadedEvent();

            addLevelEditorObjects();
        }

        private static void addManagers()
        {
            ModDataManager._instance = ModManagers.New<ModDataManager>();
            _ = ModManagers.New<ContentRepositoryManager>();
            _ = ModManagers.New<ExclusiveContentManager>();
            _ = ModManagers.New<ModLocalizationManager>();
            _ = ModManagers.New<ModResources>();
            _ = ModManagers.New<ModUIManager>();
            _ = ModManagers.New<ModLevelManager>();
            _ = ModManagers.New<ModWeaponsManager>();
            _ = ModManagers.New<ModUpgradesManager>();
            _ = ModManagers.New<ModEnemiesManager>();
            _ = ModManagers.New<ModGameModifiersManager>();

            _ = ModManagers.New<ExclusiveContentManager>();
            _ = ModManagers.New<ContentManager>();
            _ = ModManagers.New<UpdateManager>();
            _ = ModManagers.New<NewsManager>();
            _ = ModManagers.New<PersonalizationManager>();

            _ = ModManagers.New<WeatherManager>();
            _ = ModManagers.New<FloatingDustManager>();
            _ = ModManagers.New<FadingVoxelManager>();

            _ = ModManagers.New<ArenaRemodelManager>();
            _ = ModManagers.New<AdvancedPhotoModeManager>();
            _ = ModManagers.New<WebhookManager>();
            _ = ModManagers.New<UpgradeModeManager>();
            _ = ModManagers.New<TransitionManager>();
        }

        private static void loadAssemblies()
        {
            LevelEditorPatch.Patch.Apply();
            _ = ModBotAPI.ModBotAPI.Initialize();
        }

        private static void addLevelEditorObjects()
        {
            Patch.AddObjectAlt("WeatherSettingsOverride", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform, new Type[] { typeof(LevelEditorWeatherSettingsOverride) }, ModCore.texturesFolder + "WeatherSettingsOverride.png");

            Patch.AddObjectAlt("Axe1", "OverhaulMod", "Enemies", ModPrefabUtils.axe1Spawner, null, ModCore.texturesFolder + "AxeBot.png");
            Patch.AddObjectAlt("Axe2", "OverhaulMod", "Enemies", ModPrefabUtils.axe2Spawner, null, ModCore.texturesFolder + "AxeBot.png");
            Patch.AddObjectAlt("Axe3", "OverhaulMod", "Enemies", ModPrefabUtils.axe3Spawner, null, ModCore.texturesFolder + "AxeBot.png");
            Patch.AddObjectAlt("Axe4", "OverhaulMod", "Enemies", ModPrefabUtils.axe4Spawner, null, ModCore.texturesFolder + "AxeBot.png");

            Patch.AddObjectAlt("Scythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scythe1Spawner, null, ModCore.texturesFolder + "ScytheBot.png");
            Patch.AddObjectAlt("Scythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scythe2Spawner, null, ModCore.texturesFolder + "ScytheBot.png");
            Patch.AddObjectAlt("Scythe3", "OverhaulMod", "Enemies", ModPrefabUtils.scythe3Spawner, null, ModCore.texturesFolder + "ScytheBot.png");
            Patch.AddObjectAlt("Scythe4", "OverhaulMod", "Enemies", ModPrefabUtils.scythe4Spawner, null, ModCore.texturesFolder + "ScytheBot.png");
            Patch.AddObjectAlt("SprinterScythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter1Spawner, null, ModCore.texturesFolder + "SprinterScytheBot.png");
            Patch.AddObjectAlt("SprinterScythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter2Spawner, null, ModCore.texturesFolder + "SprinterScytheBot.png");

            Patch.AddObjectAlt("Halberd1", "OverhaulMod", "Enemies", ModPrefabUtils.halberd1Spawner, null, ModCore.texturesFolder + "HalberdBot.png");
            Patch.AddObjectAlt("Halberd2", "OverhaulMod", "Enemies", ModPrefabUtils.halberd2Spawner, null, ModCore.texturesFolder + "HalberdBot.png");
            Patch.AddObjectAlt("Halberd3", "OverhaulMod", "Enemies", ModPrefabUtils.halberd3Spawner, null, ModCore.texturesFolder + "HalberdBot.png");

            Patch.AddObjectPathOverrideV1("WeatherSettingsOverride", "OverhaulMod", string.Empty);
            Patch.AddObjectPathOverrideV1("Axe1", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Axe2", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Axe3", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Axe4", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Scythe1", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Scythe2", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Scythe3", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Scythe4", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("SprinterScythe1", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("SprinterScythe2", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Halberd1", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Halberd2", "OverhaulMod", "Enemies");
            Patch.AddObjectPathOverrideV1("Halberd3", "OverhaulMod", "Enemies");
        }

        public static bool HasToLoad()
        {
            return !ModManagers.Instance;
        }
    }
}
