using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public static class OverModesController
    {
        public static void InitializeForCurrentScene()
        {
            GameObject overModesGameObject = new GameObject("Overhaul_OverModes");
            OverModesController._overModesGameObject = overModesGameObject;

            OverModesController.InstantiateManager<EndlessModeOverhaul>("Endless_Overmode");
            OverModesController.InstantiateManager<StoryModeOverhaul>("Story_Overmode");
            OverModesController.InstantiateManager<ExplorationMode>("Exploration_Overmode");
            OverModesController.InstantiateManager<SurvivalMode>("Survival_Overmode");
            OverModesController.InstantiateManager<SandboxMode>("Sandbox_Overmode");
        }

        public static bool CurrentGamemodeIsOvermode()
        {
            foreach (GameMode mode in OverModesController._gameModes.Keys)
            {
                if (GameModeManager.Is(mode))
                {
                    return true;
                }
            }
            return false;
        }

        public static OverModeBase GetCurrentOvermode()
        {
            if (!OverModesController.CurrentGamemodeIsOvermode())
            {
                return null;
            }
            return OverModesController._gameModes[Singleton<GameFlowManager>.Instance.GetCurrentGameMode()];
        }

        public static void ProcessEvent(in OverModeBase.EventNames eventName, in object[] args)
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                OverModesController.GetCurrentOvermode().ProcessEvent(eventName, args);
            }
        }

        public static T ProcessEventAndReturn<T>(in OverModeBase.EventNames eventName, in object[] args) where T : class
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                return OverModesController.GetCurrentOvermode().ProcessEventAndReturn<T>(eventName, args);
            }
            return default(T);
        }

        public static T InstantiateManager<T>(in string name) where T : OverModeBase
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(OverModesController._overModesGameObject.transform);
            T t = gameObject.AddComponent<T>();
            t.Initialize();
            if (!OverModesController._gameModes.ContainsKey(t.GetGamemode()))
            {
                OverModesController._gameModes.Add(t.GetGamemode(), t);
            }
            return t;
        }

        private static GameObject _overModesGameObject;

        private static Dictionary<GameMode, OverModeBase> _gameModes = new Dictionary<GameMode, OverModeBase>();
    }
}
