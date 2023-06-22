using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using InternalModBot;
using ModLibrary;
using ModLibrary.YieldInstructions;
using System.Collections;
using System.Collections.Generic;
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
        /// Define if we got errors while starting up the mod
        /// </summary>
        internal static bool IsCoreLoadedIncorrectly;

        /// <summary>
        /// Returns <b>True</b> if <b><see cref="OverhaulMod.Core"/></b> is not <b>Null</b>
        /// </summary>
        public static bool IsModInitialized => !IsCoreLoadedIncorrectly && Core != null;

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
            if (IsModInitialized)
                return;

            Base = this;
            TryCreateCore();
        }

        /// <summary>
        /// Create core when mod was loaded or enabled
        /// </summary>
        protected override void OnModEnabled()
        {
            if (IsModInitialized)
                return;

            Base = this;
            TryCreateCore();
        }

        /// <summary>
        /// Destroy the when mod was deactivated
        /// </summary>
        protected override void OnModDeactivated()
        {
            if (!IsModInitialized)
                return;

            Base = null;
            DeconstructCore();
        }

#if DEBUG
        protected override bool ImplementsSettingsWindow()
        {
            return true;
        }

        protected override void CreateSettingsWindow(ModOptionsWindowBuilder builder)
        {
            ModOptionsWindowBuilder.Page page1 = builder.AddPage("Dev Cheats");
            page1.AddButton("Reset other players data", delegate
            {
                if (MultiplayerPlayerInfoManager.Instance == null)
                    return;

                List<MultiplayerPlayerInfoState> list = MultiplayerPlayerInfoManager.Instance.GetAllPlayerInfoStates();
                foreach (MultiplayerPlayerInfoState statee in list)
                {
                    if (!statee.IsDetached() && statee.state.PlayFabID != PlayFabDataController.GetLocalPlayFabID())
                    {
                        GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients);
                        newEvent.EventData = "[OverhaulPlayerInfoAnswer]@" + statee.state.PlayFabID + "@" + OverhaulModdedPlayerInfo.SerializeData(true, statee.state.PlayFabID);
                        newEvent.Send();
                    }
                }
            });
        }
#endif

        /// <summary>
        /// Used for events
        /// </summary>
        /// <param name="firstPersonMover"></param>
        protected override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            if (!IsModInitialized)
                return;

            // An event that is usually called before FPM full initialization
            OverhaulEventsController.DispatchEvent(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, firstPersonMover);
            _ = StaticCoroutineRunner.StartStaticCoroutine(waitForRobotInitializationAndDispatchEvent(firstPersonMover));
        }

        /// <summary>
        /// Create the instance of mod core
        /// </summary>
        internal void TryCreateCore()
        {
            if (IsModInitialized)
                return;

            GameObject gameObject = new GameObject("CloneDroneOverhaul_Core");
            OverhaulCore core = gameObject.AddComponent<OverhaulCore>();
            _ = core.Initialize(out string errors);

            if (errors != null)
                OverhaulExceptions.OnModEarlyCrash(errors);
        }

        /// <summary>
        /// Destroy the instance of the core
        /// </summary>
        internal void DeconstructCore()
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
            yield return new WaitForSecondsRealtime(0.15f);
            if (firstPersonMover != null && firstPersonMover.HasCharacterModel())
                OverhaulEventsController.DispatchEvent<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, firstPersonMover);
        }

        public static bool IsModEnabled(string modID)
        {
            List<ModInfo> infos = ModsManager.Instance.GetActiveModInfos();
            if (infos.IsNullOrEmpty())
                return false;

            foreach (ModInfo info in infos)
                if (info.UniqueID.Equals(modID))
                    return true;

            return false;
        }
    }
}
