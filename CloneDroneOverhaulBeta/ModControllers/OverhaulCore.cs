using CDOverhaul.Gameplay;
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
    public sealed class OverhaulCore : MonoBehaviour
    {
        /// <summary>
        /// The mod directory path
        /// </summary>
        public string ModFolder => OverhaulMod.Base.ModInfo.FolderPath;

        /// <summary>
        /// The UI controller instance
        /// </summary>
        public HUDControllers HUDController { get; private set; }

        /// <summary>
        /// The gameplay features controler instance
        /// </summary>
        public MainGameplayController GameplayController { get; private set; }

        /// <summary>
        /// Misc features controller instance
        /// </summary>
        public SharedControllers Shared { get; private set; }

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

            EnableCursorController.Reset();
            OverhaulEventManager.Initialize();
            ModControllerManager.Initialize(controllers);
            LevelEditorObjectsController.Initialize();

            Shared = ModControllerManager.NewController<SharedControllers>();
            HUDController = ModControllerManager.NewController<HUDControllers>();
            GameplayController = ModControllerManager.NewController<MainGameplayController>();
            _ = ModControllerManager.NewController<LevelEditorMultipleObjectsController>();

            SettingsController.Initialize();
            OverhaulDebugController.Initialize();
            OverhaulGraphicsController.Initialize();
            RobotAccessoriesController.Initialize();
            ExclusivityController.Initialize();

            ReplacementBase.CreateReplacements();
        }
    }
}
