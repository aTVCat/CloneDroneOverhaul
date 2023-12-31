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
                ModManagers.Instance.DispatchModLoadedEvent();
                return;
            }

            loadAssemblies();

            ModBuildInfo.Load();

            GameObject gameObject = new GameObject("OverhaulManagers", new Type[] { typeof(ModManagers) });
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            addManagers();
            ModManagers.Instance.DispatchModLoadedEvent();

            addLevelEditorObjects();
        }

        private static void addManagers()
        {
            ModUserDataManager._instance = ModManagers.New<ModUserDataManager>();
            _ = ModManagers.New<ContentRepositoryManager>();
            _ = ModManagers.New<ExclusiveContentManager>();
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
        }

        private static void loadAssemblies()
        {
            LevelEditorPatch.Patch.Apply();
            _ = ModBotAPI.ModBotAPI.Initialize();
        }

        private static void addLevelEditorObjects()
        {
            Patch.AddObject("WeatherSettingsOverride", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform, new Type[] { typeof(LevelEditorWeatherSettingsOverride)}, null);

            Patch.AddObject("Axe1", "OverhaulMod", "Enemies", ModPrefabUtils.axe1Spawner, null, null);
            Patch.AddObject("Axe2", "OverhaulMod", "Enemies", ModPrefabUtils.axe2Spawner, null, null);
            Patch.AddObject("Axe3", "OverhaulMod", "Enemies", ModPrefabUtils.axe3Spawner, null, null);
            Patch.AddObject("Axe4", "OverhaulMod", "Enemies", ModPrefabUtils.axe4Spawner, null, null);

            Patch.AddObject("Scythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scythe1Spawner, null, null);
            Patch.AddObject("Scythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scythe2Spawner, null, null);
            Patch.AddObject("Scythe3", "OverhaulMod", "Enemies", ModPrefabUtils.scythe3Spawner, null, null);
            Patch.AddObject("Scythe4", "OverhaulMod", "Enemies", ModPrefabUtils.scythe4Spawner, null, null);
            Patch.AddObject("SprinterScythe1", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter1Spawner, null, null);
            Patch.AddObject("SprinterScythe2", "OverhaulMod", "Enemies", ModPrefabUtils.scytheSprinter2Spawner, null, null);

            Patch.AddObject("Halberd1", "OverhaulMod", "Enemies", ModPrefabUtils.halberd1Spawner, null, null);
            Patch.AddObject("Halberd2", "OverhaulMod", "Enemies", ModPrefabUtils.halberd2Spawner, null, null);
            Patch.AddObject("Halberd3", "OverhaulMod", "Enemies", ModPrefabUtils.halberd3Spawner, null, null);
        }

        public static bool HasToLoad()
        {
            return !ModManagers.Instance;
        }
    }
}
