using CDOverhaul.Gameplay;
using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using CDOverhaul.Shared;
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
        public string ModFolder { get; private set; }

        public HUDControllers HUDController { get; private set; }
        public MainGameplayController GameplayController { get; private set; }
        public SharedControllers Shared { get; private set; }

        internal void InitializeCore()
        {
            // Set some references up
            OverhaulBase.Core = this;
            OverhaulAPI.API.LoadAPI();

            ModFolder = OverhaulBase.Base.ModInfo.FolderPath;

            // Initialize controllers
            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            EnableCursorController.Reset();
            OverhaulEventManager.Initialize();
            ModControllerManager.Initialize(controllers);
            LevelEditorObjectsController.Initialize();

            Shared = ModControllerManager.NewController<SharedControllers>();
            HUDController = ModControllerManager.NewController<HUDControllers>();
            GameplayController = ModControllerManager.NewController<MainGameplayController>();

            SettingsController.Initialize();
            OverhaulDebugController.Initialize();
            OverhaulGraphicsController.Initialize();

            ReplacementBase.CreateReplacements();
        }
    }
}
