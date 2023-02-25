﻿using CDOverhaul.Gameplay;
using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using CDOverhaul.Shared;
using System;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The core of the mod. Contains important variables
    /// </summary>
    public class OverhaulCore : OverhaulMonoBehaviour
    {
        /// <summary>
        /// The mod directory path.
        /// Ends with '/'
        /// </summary>
        public string ModDirectory => OverhaulMod.Base.ModInfo.FolderPath;

        /// <summary>
        /// The UI controller instance
        /// </summary>
        public OverhaulCanvasController HUDController
        { 
            get;
            private set;
        }

        /// <summary>
        /// The gameplay features controler instance
        /// </summary>
        public OverhaulGameplayCoreController GameplayController 
        { 
            get;
            private set;
        }

        /// <summary>
        /// Misc features controller instance
        /// </summary>
        public SharedControllers Shared 
        {
            get; 
            private set;
        }

        internal bool Initialize(out string errorString)
        {
            try
            {
                initialize();
            }
            catch (Exception ex)
            {
                errorString = ex.ToString();
                return false;
            }
            errorString = null;
            return true;
        }

        private void initialize()
        {
            OverhaulMod.Core = this;
            _ = OverhaulAPI.API.LoadAPI();

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            OverhaulConsoleController.Initialize();
            EnableCursorController.Reset();
            OverhaulEventManager.Initialize();
            OverhaulController.InitializeStatic(controllers);
            LevelEditorObjectsController.Initialize();

            Shared = OverhaulController.InitializeController<SharedControllers>();
            HUDController = OverhaulController.InitializeController<OverhaulCanvasController>();
            GameplayController = OverhaulController.InitializeController<OverhaulGameplayCoreController>();
            _ = OverhaulController.InitializeController<LevelEditorMultipleObjectsController>();

            FirstPersonMoverExtention.InitializeStatic();
            SettingsController.Initialize();
            OverhaulDebugController.Initialize();
            OverhaulGraphicsController.Initialize();
            ExclusivityController.Initialize();

            ReplacementBase.CreateReplacements();
        }

        protected override void OnDisposed()
        {
            HUDController = null;
            GameplayController = null;
            Shared = null;
            OverhaulMod.Core = null;
        }
    }
}
