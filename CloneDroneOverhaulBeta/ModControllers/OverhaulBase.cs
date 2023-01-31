using CDOverhaul.Gameplay;
using CDOverhaul.LevelEditor;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of the mod. Allows mod startup
    /// </summary>
    [MainModClass]
    public sealed class OverhaulBase : Mod
    {
        public const string ModDeactivatedEventString = "ModDeactivated";

        /// <summary>
        /// The instance of the core
        /// </summary>
        public static OverhaulCore Core { get; internal set; }

        /// <summary>
        /// The instance of main mod class
        /// </summary>
        public static OverhaulBase Base { get; internal set; }

        /// <summary>
        /// Returns <b>True</b> if <b><see cref="OverhaulBase.Core"/></b> is not <b>Null</b>
        /// </summary>
        public static bool IsCoreCreated => Core != null;

        protected override void OnModLoaded()
        {
            Base = this;
            TryCreateCore();
        }

        protected override void OnModEnabled()
        {
            Base = this;
            TryCreateCore();
        }

        protected override void OnModDeactivated()
        {
            Base = null;
            DeconstructCore();
        }

        protected override UnityEngine.Object OnResourcesLoad(string path)
        {
            if (!IsCoreCreated)
            {
                return null;
            }

            return LevelEditorObjectsController.GetObject(path);
        }

        protected override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            if (!IsCoreCreated)
            {
                return;
            }

            OverhaulEventManager.DispatchEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, firstPersonMover);
            DelegateScheduler.Instance.Schedule(delegate
            {
                OverhaulEventManager.DispatchEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, firstPersonMover);
            }, 0.1f);
        }


        /// <summary>
        /// Create the instance of mod core monobehaviour
        /// </summary>
        internal void TryCreateCore()
        {
            if (IsCoreCreated)
            {
                debug.Log("Overhaul: There's no need to create Core class instance.");
                return;
            }

            GameObject gameObject = new GameObject("CloneDroneOverhaul_Core");
            OverhaulCore core = gameObject.AddComponent<OverhaulCore>();
            core.InitializeCore();

            debug.Log("Overhaul: Created Core class instance");
        }

        /// <summary>
        /// Destroy the instance of the core
        /// </summary>
        internal void DeconstructCore()
        {
            if (!IsCoreCreated)
            {
                debug.Log("Overhaul: There's no need to deconstruct Core class instance.");
                return;
            }

            OverhaulEventManager.DispatchEvent(ModDeactivatedEventString);

            UnityEngine.Object.Destroy(Core.gameObject);
            Core = null;
        }
    }
}
