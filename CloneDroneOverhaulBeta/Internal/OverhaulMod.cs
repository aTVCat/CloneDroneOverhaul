using CDOverhaul.Gameplay;
using CDOverhaul.LevelEditor;
using ModLibrary;
using ModLibrary.YieldInstructions;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of the mod. Starts up the mod
    /// </summary>
    /// Todo: Custom advancements system (or patch vanilla one)
    [MainModClass]
    public class OverhaulMod : Mod
    {
        /// <summary>
        /// An event that is called when mod was deactivated by user
        /// </summary>
        public const string ModDeactivatedEventString = "ModDeactivated";

        /// <summary>
        /// Define if we got errors while starting up the mod
        /// </summary>
        internal static bool IsCoreLoadedIncorrectly;

        /// <summary>
        /// Returns <b>True</b> if <b><see cref="OverhaulMod.Core"/></b> is not <b>Null</b>
        /// </summary>
        public static bool IsCoreCreated => !IsCoreLoadedIncorrectly && Core != null;

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
            if (IsCoreCreated)
            {
                return;
            }

            Base = this;
            TryCreateCore();
        }

        /// <summary>
        /// Create core when mod was loaded or enabled
        /// </summary>
        protected override void OnModEnabled()
        {
            if (IsCoreCreated)
            {
                return;
            }

            Base = this;
            TryCreateCore();
        }

        /// <summary>
        /// Destroy the when mod was deactivated
        /// </summary>
        protected override void OnModDeactivated()
        {
            if (!IsCoreCreated)
            {
                return;
            }

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

            UnityEngine.Object @object = LevelEditorObjectsController.GetObject(path);
            if(@object == null && OverhaulLevelAdder.HasLevel(path))
            {
                @object = new TextAsset(OverhaulLevelAdder.GetLevel(ref path));
            }

            return @object;
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
            OverhaulEventManager.DispatchEvent(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, firstPersonMover);
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

            GameObject gameObject = new GameObject("CloneDroneOverhaul_Core");
            OverhaulCore core = gameObject.AddComponent<OverhaulCore>();
            _ = core.Initialize(out string errors);

            if (errors != null)
            {
                OverhaulExceptions.OnModEarlyCrash(errors);
                return;
            }
            ChangeWindowTitle();
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
            GameObject.Destroy(Core.gameObject);
            Core = null;
        }

        internal void ChangeWindowTitle()
        {
            if (!OverhaulVersion.AllowWindowNameChanging)
            {
                return;
            }

            System.IntPtr windowPtr = FindWindow(null, Application.productName);
            if (windowPtr.Equals(System.IntPtr.Zero))
            {
                return;
            }

            _ = SetWindowText(windowPtr, "Clone Drone Overhaul");
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
                OverhaulEventManager.DispatchEvent<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, firstPersonMover);
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(System.IntPtr hwnd, string lpString);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern System.IntPtr FindWindow(string className, string windowName);
    }
}
