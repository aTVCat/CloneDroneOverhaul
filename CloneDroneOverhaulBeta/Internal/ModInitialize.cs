using CDOverhaul.Device;
using CDOverhaul.DevTools;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using CDOverhaul.Visuals;
using CDOverhaul.Visuals.ArenaOverhaul;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    public class ModInitialize : OverhaulDisposable
    {
        public void Load()
        {
            List<Action> toInit = new List<Action>()
            {
                InitMainControllers,
                InitDebugManagers,
                InitGraphicsManagers,
                InitGameplayManagers,
                InitLevelEditorManagers,
                InitLocalizationManagers,
                InitContentManagers,
                InitEnhancementManagers,
            };
            foreach (Action action in toInit)
            {
                string name = action.Method.Name;
                OverhaulDebug.Log("Initializing: " + name, EDebugType.Initialize);
                action();
                OverhaulDebug.Log("Initialized: " + name, EDebugType.Initialize);
            }
        }

        public void InitMainControllers()
        {
            EnableCursorController.Reset();
            DeviceSpecifics.Initialize();

            OverhaulEventsController.Initialize();
            OverhaulSettingsController.Initialize();
        }

        public void InitDebugManagers()
        {
            OverhaulDebugConsole.Initialize();
            OverhaulDebugActions.Initialize();
        }

        public void InitGraphicsManagers()
        {
            GameObject graphicsObject = new GameObject("Graphics");
            graphicsObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulRenderManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulCameraManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulGraphicsManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulUIEffectsManager>(graphicsObject.transform);
        }

        public void InitLocalizationManagers()
        {
            _ = OverhaulController.Add<OverhaulLocalizationManager>();
        }

        public void InitGameplayManagers()
        {
            GameObject gameplayObject = new GameObject("Gameplay");
            gameplayObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulGameplayManager>(gameplayObject.transform);
        }

        public void InitLevelEditorManagers()
        {
            GameObject levelEditorObject = new GameObject("LevelEditor");
            levelEditorObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulLevelEditorManager>(levelEditorObject.transform);
        }

        public void InitEnhancementManagers()
        {
            GameObject patchesObject = new GameObject("Enhancements");
            patchesObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<ArenaOverhaulManager>(patchesObject.transform);
            _ = OverhaulController.Add<ReplacementsManager>(patchesObject.transform);
        }

        public void InitContentManagers()
        {
            GameObject contentObject = new GameObject("Content");
            contentObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<AdditionalContentManager>(contentObject.transform);
        }
    }
}
