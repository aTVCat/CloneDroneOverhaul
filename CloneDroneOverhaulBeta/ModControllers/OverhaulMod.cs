using CDOverhaul.Gameplay;
using CDOverhaul.LevelEditor;
using ModLibrary;
using ModLibrary.YieldInstructions;
using System.Collections;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of the mod. Starts up the mod
    /// </summary>
    [MainModClass]
    public class OverhaulMod : Mod
    {
        /// <summary>
        /// An event that is called when mod was deactivated by user
        /// </summary>
        public const string ModDeactivatedEventString = "ModDeactivated";

        /// <summary>
        /// Returns <b>True</b> if <b><see cref="OverhaulMod.Core"/></b> is not <b>Null</b>
        /// </summary>
        public static bool IsCoreCreated => Core != null;

        /// <summary>
        /// The instance of the core
        /// </summary>
        public static OverhaulCore Core { get; internal set; }

        /// <summary>
        /// The instance of main mod class
        /// </summary>
        public static OverhaulMod Base { get; internal set; }

        /// <summary>
        /// Create core when mod was loaded
        /// </summary>
        protected override void OnModLoaded()
        {
            Base = this;
            TryCreateCore();
        }

        /// <summary>
        /// Create core when mod was loaded or enabled
        /// </summary>
        protected override void OnModEnabled()
        {
            Base = this;
            TryCreateCore();
        }

        /// <summary>
        /// Destroy the when mod was deactivated
        /// </summary>
        protected override void OnModDeactivated()
        {
            Base = null;
            DeconstructCore();
        }

        /// <summary>
        /// Currently used to make modded level editor objects real
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override UnityEngine.Object OnResourcesLoad(string path)
        {
            if (!IsCoreCreated)
            {
                return null;
            }

            return LevelEditorObjectsController.GetObject(path);
        }

        /// <summary>
        /// Used for events
        /// </summary>
        /// <param name="firstPersonMover"></param>
        protected override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            if (!IsCoreCreated)
            {
                return;
            }

            // An event that is usually called before FPM full initialization
            OverhaulEventManager.DispatchEvent(MainGameplayController.FirstPersonMoverSpawnedEventString, firstPersonMover);
            _ = StaticCoroutineRunner.StartStaticCoroutine(waitFPMToInitialize(firstPersonMover));
        }

        /// <summary>
        /// Create the instance of mod core monobehaviour
        /// </summary>
        internal void TryCreateCore()
        {
            if (IsCoreCreated)
            {
                return;
            }
            // Make a window that appear when mod was loaded incorrectly (put core creation into try block)

            GameObject gameObject = new GameObject("CloneDroneOverhaul_Core");
            OverhaulCore core = gameObject.AddComponent<OverhaulCore>();
            _ = core.Initialize(out string errors);

            if (errors != null)
            {
                OverhaulExceptions.OnModCrashedWhileLoading(errors);
            }
        }

        /// <summary>
        /// Destroy the instance of the core
        /// </summary>
        internal void DeconstructCore()
        {
            if (!IsCoreCreated)
            {
                return;
            }

            OverhaulEventManager.DispatchEvent(ModDeactivatedEventString);

            Object.Destroy(Core.gameObject);
            Core = null;
        }

        /// <summary>
        /// Wait until all things are initiliazed in <see cref="FirstPersonMover"/> and dispatch event if robot isn't null
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <returns></returns>
        private IEnumerator waitFPMToInitialize(FirstPersonMover firstPersonMover)
        {
            yield return new WaitForCharacterModelAndUpgradeInitialization(firstPersonMover);
            if (firstPersonMover != null)
            {
                OverhaulEventManager.DispatchEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, firstPersonMover);
            }
        }
    }
}
