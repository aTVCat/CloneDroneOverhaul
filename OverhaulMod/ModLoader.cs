﻿using OverhaulMod.Combat;
using OverhaulMod.Combat.Enemies;
using OverhaulMod.Combat.Levels;
using OverhaulMod.Combat.Upgrades;
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
            _ = ModManagers.New<ModLevelEditorManager>();

            _ = ModManagers.New<ModExclusiveContentManager>();
            _ = ModManagers.New<ModContentManager>();
            _ = ModManagers.New<ModUpdateManager>();

            _ = ModManagers.New<WeatherManager>();
        }

        private static void loadAssemblies()
        {
            _ = ModBotAPI.ModBotAPI.Initialize();
        }

        public static bool HasToLoad()
        {
            return !ModManagers.Instance;
        }
    }
}
