using CDOverhaul.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    public class ModInitialize : OverhaulDisposable
    {
        public ModInitialize()
        {
            LoadFramework();
        }

        public void LoadFramework()
        {
            List<Action> toInit = new List<Action>()
            {
                InitDebugManager,
                InitGraphicsManager
            };
            foreach (Action action in toInit)
            {
                action();
                OverhaulDebug.Log("Initialized: " + action.Method.Name, EDebugType.ModInit);
            }
        }

        public void InitDebugManager()
        {
            OverhaulDebugConsole.Initialize();
        }

        public void InitGraphicsManager()
        {
            OverhaulController.AddController<OverhaulCameraController>();

            GameObject graphicsObject = new GameObject("Graphics");
            graphicsObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);
            OverhaulController.AddController<OverhaulGraphicsManager>(graphicsObject.transform);
        }
    }
}
