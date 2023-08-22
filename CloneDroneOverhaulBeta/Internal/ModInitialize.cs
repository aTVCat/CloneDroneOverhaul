using CDOverhaul.DevTools;
using CDOverhaul.Gameplay.QualityOfLife;
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
                InitDebugManagers,
                InitGraphicsManagers,
                InitQOLManagers,
                InitLocalizationManagers,
            };
            foreach (Action action in toInit)
            {
                action();
                OverhaulDebug.Log("Initialized: " + action.Method.Name, EDebugType.ModInit);
            }
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

            _ = OverhaulController.AddController<OverhaulRenderManager>(graphicsObject.transform);
            _ = OverhaulController.AddController<OverhaulCameraManager>(graphicsObject.transform);
            _ = OverhaulController.AddController<OverhaulGraphicsManager>(graphicsObject.transform);
            _ = OverhaulController.AddController<OverhaulUIEffectsManager>(graphicsObject.transform);
        }

        public void InitLocalizationManagers()
        {
            _ = OverhaulController.AddController<OverhaulLocalizationManager>();
        }

        public void InitQOLManagers()
        {
            _ = OverhaulController.AddController<AutoBuildController>();
            _ = OverhaulController.AddController<LevelEditorFixes>();
            _ = OverhaulController.AddController<ModBotTagDisabler>();
        }
    }
}
