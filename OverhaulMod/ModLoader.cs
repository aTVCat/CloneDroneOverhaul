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
            _ = ModManagers.New<ModResources>();
            _ = ModManagers.New<ModUIManager>();
            _ = ModManagers.New<ModWeaponsManager>();
            _ = ModManagers.New<ModUpgradesManager>();
            _ = ModManagers.New<ModEnemiesManager>();
            _ = ModManagers.New<ModLevelEditorManager>();
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
