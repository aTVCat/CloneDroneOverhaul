using System;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class OverhaulGameplayCoreController : OverhaulController
    {
        #region Events
        public const string GamemodeChangedEventString = "GamemodeChanged";
        public const string MainCameraSwitchedEventString = "CameraMainSwitched";
        public const string CurrentCameraSwitchedEventString = "CameraCurrentSwitched";

        public const string FirstPersonMoverSpawnedEventString = "FPMSpawned";
        public const string FirstPersonMoverSpawned_DelayEventString = "FPMSpawned_Delay";

        public const string PlayerSetAsCharacter = "PlayerSet_Base";
        public const string PlayerSetAsFirstPersonMover = "PlayerSet_FirstPersonMover";
        #endregion

        #region Some variables

        private GameMode m_GameModeLastTimeCheck;

        /// <summary>
        /// The start index of mod gamemodes
        /// </summary>
        public const int GamemodeStartIndex = 2000;

        #endregion

        public static OverhaulGameplayCoreController Core
        {
            get;
            private set;
        }

        public Camera MainCamera
        {
            get;
            private set;
        }

        public Camera CurrentCamera
        {
            get;
            private set;
        }

        public PlayerOutfitController Outfits
        {
            get;
            private set;
        }
        public WeaponSkinsController WeaponSkins
        {
            get;
            private set;
        }

        public GamemodeSubstatesController GamemodeSubstates
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            Core = this;

            GamemodeSubstates = OverhaulController.InitializeController<GamemodeSubstatesController>();
            WeaponSkins = OverhaulController.InitializeController<WeaponSkinsController>();
            Outfits = OverhaulController.InitializeController<PlayerOutfitController>();
            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);
        }

        protected override void OnDisposed()
        {
            Core = null;
            MainCamera = null;
            CurrentCamera = null;
            Outfits = null;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            GameMode currentGamemode = GameFlowManager.Instance.GetCurrentGameMode();
            if (currentGamemode != m_GameModeLastTimeCheck)
            {
                sendGamemodeWasUpdateEvent();
            }
            m_GameModeLastTimeCheck = currentGamemode;

            Camera mainCamera = Camera.main;
            if (mainCamera != MainCamera)
            {
                OverhaulEventManager.DispatchEvent(MainCameraSwitchedEventString, mainCamera);
            }
            MainCamera = mainCamera;

            Camera currentCamera = Camera.current;
            if (currentCamera != CurrentCamera)
            {
                OverhaulEventManager.DispatchEvent(CurrentCameraSwitchedEventString, currentCamera);
            }
            CurrentCamera = currentCamera;
        }

        /// <summary>
        /// Send <see cref="GamemodeChangedEventString"/> event
        /// </summary>
        private void sendGamemodeWasUpdateEvent()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            OverhaulEventManager.DispatchEvent(GamemodeChangedEventString);
        }

        public override string[] Commands()
        {
            throw new NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}