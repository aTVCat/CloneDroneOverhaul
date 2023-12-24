using OverhaulMod.Combat;
using OverhaulMod.Combat.Enemies;
using OverhaulMod.Combat.Levels;
using OverhaulMod.Combat.Upgrades;
using OverhaulMod.Visuals.Environment;
using System;
using LevelEditorPatch;
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
            _ = ModManagers.New<ModContentRepositoryManager>();
            _ = ModManagers.New<ModExclusiveContentManager>();
            _ = ModManagers.New<ModResources>();
            _ = ModManagers.New<ModUIManager>();
            _ = ModManagers.New<ModLevelManager>();
            _ = ModManagers.New<ModWeaponsManager>();
            _ = ModManagers.New<ModUpgradesManager>();
            _ = ModManagers.New<ModEnemiesManager>();
            _ = ModManagers.New<ModGameModifiersManager>();

            _ = ModManagers.New<ModExclusiveContentManager>();
            _ = ModManagers.New<ModContentManager>();
            _ = ModManagers.New<ModUpdateManager>();

            _ = ModManagers.New<WeatherManager>();
        }

        private static void loadAssemblies()
        {
            LevelEditorPatch.Patch.Apply();
            _ = ModBotAPI.ModBotAPI.Initialize();
        }

        private static void addLevelEditorObjects()
        {
            Patch.AddObject("WeatherSettingsOverride", "OverhaulMod", "", GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform, null, null);

            Patch.AddObject("Axe1", "OverhaulMod", "Enemies", PrefabStorage.axe1Spawner, null, null);
            Patch.AddObject("Axe2", "OverhaulMod", "Enemies", PrefabStorage.axe2Spawner, null, null);
            Patch.AddObject("Axe3", "OverhaulMod", "Enemies", PrefabStorage.axe3Spawner, null, null);
            Patch.AddObject("Axe4", "OverhaulMod", "Enemies", PrefabStorage.axe4Spawner, null, null);

            Patch.AddObject("Scythe1", "OverhaulMod", "Enemies", PrefabStorage.scythe1Spawner, null, null);
            Patch.AddObject("Scythe2", "OverhaulMod", "Enemies", PrefabStorage.scythe2Spawner, null, null);
            Patch.AddObject("Scythe3", "OverhaulMod", "Enemies", PrefabStorage.scythe3Spawner, null, null);
            Patch.AddObject("Scythe4", "OverhaulMod", "Enemies", PrefabStorage.scythe4Spawner, null, null);
            Patch.AddObject("SprinterScythe1", "OverhaulMod", "Enemies", PrefabStorage.scytheSprinter1Spawner, null, null);
            Patch.AddObject("SprinterScythe2", "OverhaulMod", "Enemies", PrefabStorage.scytheSprinter2Spawner, null, null);

            Patch.AddObject("Halberd1", "OverhaulMod", "Enemies", PrefabStorage.halberd1Spawner, null, null);
            Patch.AddObject("Halberd2", "OverhaulMod", "Enemies", PrefabStorage.halberd2Spawner, null, null);
            Patch.AddObject("Halberd3", "OverhaulMod", "Enemies", PrefabStorage.halberd3Spawner, null, null);
        }

        public static bool HasToLoad()
        {
            return !ModManagers.Instance;
        }
    }
}
