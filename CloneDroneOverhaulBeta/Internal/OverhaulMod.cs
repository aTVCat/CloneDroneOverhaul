using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Overmodes;
using InternalModBot;
using ModLibrary;
using ModLibrary.YieldInstructions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of the mod. Launches the mod
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
        public static bool IsModInitialized => !IsLoadedIncorrectly && Core;
        public static bool IsHUDInitialized => IsModInitialized && Core.CanvasController;
        public static bool HasBootProcessEnded;

        /// <summary>
        /// Define if we got errors while starting up the mod
        /// </summary>
        internal static bool IsLoadedIncorrectly;

        /// <summary>
        /// The instance of the core
        /// </summary>
        public static OverhaulCore Core
        {
            get;
            internal set;
        }

        /// <summary>
        /// The instance of main mod class
        /// </summary>
        public static OverhaulMod Base
        {
            get;
            internal set;
        }

        /// <summary>
        /// Create core when mod was loaded
        /// </summary>
        public override void OnModLoaded()
        {
            Base = this;
            if (IsModInitialized)
                return;

            TryInstantiateCore();
        }

        /// <summary>
        /// Create core when mod was loaded or enabled
        /// </summary>
        public override void OnModEnabled()
        {
            Base = this;
            if (IsModInitialized)
                return;

            TryInstantiateCore();
        }

        /// <summary>
        /// Destroy the when mod was deactivated
        /// </summary>
        public override void OnModDeactivated()
        {
            if (!IsModInitialized)
                return;

            Base = null;
            DestroyCore();
        }

        /// <summary>
        /// Used for events
        /// </summary>
        /// <param name="firstPersonMover"></param>
        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            if (!IsModInitialized && !firstPersonMover)
                return;

            // An event that is usually called before FPM full initialization
            OverhaulEventsController.DispatchEvent(OverhaulGameplayManager.FIRST_PERSON_SPAWNED_EVENT, firstPersonMover);
            _ = StaticCoroutineRunner.StartStaticCoroutine(waitForRobotInitializationAndDispatchEvent(firstPersonMover));
        }

        public override Object OnResourcesLoad(string path)
        {
            if (OvermodesController.IsOvermode() && path.Contains("Overmodes/"))
            {
                path = path.Replace("Data/LevelEditorLevels/", string.Empty);
                List<LevelDescription> list = OvermodesController.Instance.CurrentOvermode.GetLevelDescriptions();
                foreach (LevelDescription description in list)
                {
                    if (description.PrefabName == path)
                        return new TextAsset(OverhaulCore.ReadText(description.PrefabName));
                }
            }

            return null;
        }

        /// <summary>
        /// Create the instance of mod core
        /// </summary>
        internal void TryInstantiateCore()
        {
            if (IsModInitialized)
                return;

            try
            {
                ModsPanelManager.Instance.closeModsMenu();
            }
            catch { }

            GameObject gameObject = new GameObject("OverhaulCore");
            OverhaulCore core = gameObject.AddComponent<OverhaulCore>();
            _ = core.TryInitialize(out string errors);

            if (!string.IsNullOrEmpty(errors))
                OverhaulExceptions.OnModEarlyCrash(errors);
        }

        /// <summary>
        /// Destroy the instance of the core
        /// </summary>
        internal void DestroyCore()
        {
            if (!IsModInitialized)
                return;

            OverhaulEventsController.DispatchEvent(ModDeactivatedEventString);
            GameObject.Destroy(Core.gameObject);
            Core = null;
        }

        /// <summary>
        /// Wait until all things are initialized in <see cref="FirstPersonMover"/> and dispatch event if robot isn't null
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <returns></returns>
        private IEnumerator waitForRobotInitializationAndDispatchEvent(FirstPersonMover firstPersonMover)
        {
            yield return new WaitForCharacterModelAndUpgradeInitialization(firstPersonMover);
            yield return null;
            if (firstPersonMover && firstPersonMover.HasCharacterModel())
                OverhaulEventsController.DispatchEvent(OverhaulGameplayManager.FIRST_PERSON_INITIALIZED_EVENT, firstPersonMover);
        }

        public static bool IsModEnabled(string modID)
        {
            LoadedModInfo loadedModInfo = ModsManager.Instance.GetLoadedModWithID(modID);
            return loadedModInfo != null && loadedModInfo.OwnerModInfo != null && loadedModInfo.IsEnabled;
        }

        public static System.Type[] GetAllTypes() => Assembly.GetExecutingAssembly().GetTypes();
    }
}
