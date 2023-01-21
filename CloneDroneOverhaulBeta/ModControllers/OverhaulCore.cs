using CDOverhaul.Gameplay;
using CDOverhaul.HUD;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The core of the mod. Contains important variables
    /// </summary>
    public sealed class OverhaulCore : MonoBehaviour
    {
        public string ModFolder { get; private set; }

        public HUDControllers HUDController { get; private set; }
        public MainGameplayController GameplayController { get; private set; }

        internal void InitializeCore()
        {
            // Set some references up
            OverhaulBase.Core = this;
            ModFolder = OverhaulBase.Base.ModInfo.FolderPath;

            // Initialize controllers
            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            EnableCursorController.Reset();
            OverhaulEventManager.Initialize();
            ModControllerManager.Initialize(controllers);
            LevelEditorObjectsController.Initialize();

            HUDController = ModControllerManager.NewController<HUDControllers>();
            GameplayController = ModControllerManager.NewController<MainGameplayController>();

            ReplacementBase.CreateReplacements();
        }
    }
}
