using CDOverhaul.Device;
using CDOverhaul.DevTools;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using CDOverhaul.Visuals;
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
                InitControllers,
                InitDebugManagers,
                InitGraphicsManagers,
                InitGameplayManagers,
                InitLevelEditorManagers,
                InitLocalizationManagers,
                InitPatchManagers,
            };
            foreach (Action action in toInit)
            {
                action();
                OverhaulDebug.Log("Initialized: " + action.Method.Name, EDebugType.Initialize);
            }
        }

        public void InitControllers()
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

        public void InitPatchManagers()
        {
            GameObject patchesObject = new GameObject("Patches");
            patchesObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<ReplacementsManager>(patchesObject.transform);
        }
    }
}
