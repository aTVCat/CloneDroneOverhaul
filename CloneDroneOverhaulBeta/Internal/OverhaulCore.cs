using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.LevelEditor;
using CDOverhaul.Localization;
using CDOverhaul.Misc;
using CDOverhaul.Patches;
using System;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The core of the mod. Contains important variables
    /// </summary>
    public class OverhaulCore : GlobalEventListener
    {
        /// <summary>
        /// The mod directory path.
        /// Ends with '/'
        /// </summary>
        public string ModDirectory => OverhaulMod.Base.ModInfo.FolderPath;

        /// <summary>
        /// The gameplay features controler instance
        /// </summary>
        public OverhaulGameplayCoreController GameplayController
        {
            get;
            private set;
        }

        /// <summary>
        /// The UI controller instance
        /// </summary>
        public OverhaulCanvasController HUDController
        { 
            get;
            private set;
        }

        /// <summary>
        /// Voxels management controller
        /// </summary>
        public VoxelsController VoxelsController
        {
            get;
            private set;
        }

        public LevelEditorLuaController EditorLua
        {
            get;
            private set;
        }

        public PhotoModeRevampController PhotoModeRevamp
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
            if (OverhaulMod.Core != null)
            {
                return;
            }
            OverhaulCompatibilityChecker.CheckGameVersion();
            OverhaulMod.Core = this;
            _ = OverhaulAPI.API.LoadAPI();

            if(ExperimentalBranchManager.Instance != null)
            {
                ExperimentalBranchManager.Instance.IsExperimentalBranch = false;
            }

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            OverhaulAudioLibrary.Initialize();
            OverhaulLevelAdder.Initialize();
            OverhaulEventManager.Initialize();
            SettingsController.Initialize();
            OverhaulConsoleController.Initialize();
            EnableCursorController.Reset();
            OverhaulController.InitializeStatic(controllers);
            LevelEditorObjectsController.Initialize();

            VoxelsController = OverhaulController.AddController<VoxelsController>();
            HUDController = OverhaulController.AddController<OverhaulCanvasController>();
            GameplayController = OverhaulController.AddController<OverhaulGameplayCoreController>();
            EditorLua = OverhaulController.AddController<LevelEditorLuaController>();
            PhotoModeRevamp = OverhaulController.AddController<PhotoModeRevampController>();
            _ = OverhaulController.AddController<LevelEditorMultipleObjectsController>();
            _ = OverhaulController.AddController<OverhaulModdedPlayerInfoController>();

            SettingsController.PostInitialize();
            OverhaulDebugger.Initialize();
            OverhaulGraphicsController.Initialize();
            ExclusivityController.Initialize();
            OverhaulTransitionController.Initialize();
            OverhaulLocalizationController.Initialize();

            if(OverhaulDiscordController.Instance == null)
            {
                _ = new GameObject("OverhaulDiscordRPCController").AddComponent<OverhaulDiscordController>();
            }

            ReplacementBase.CreateReplacements();
        }

        private void OnDestroy()
        {
            HUDController = null;
            GameplayController = null;
            VoxelsController = null;
            OverhaulMod.Core = null;
        }
    }
}
