using CDOverhaul.Gameplay.Combat;
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


        private Camera m_MainCamera;

        private Camera m_CurrentCamera;

        #endregion

        public static OverhaulGameplayCoreController Core
        {
            get;
            private set;
        }

        public AdditionalAnimationsController AdditionalAnimations
        {
            get;
            private set;
        }

        public CombatOverhaulController CombatOverhaul
        {
            get;
            private set;
        }

        public NewWeaponsController NewWeapons
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

        public AdvancedGarbageController AdvancedGarbageController
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            Core = this;

            GamemodeSubstates = OverhaulController.AddController<GamemodeSubstatesController>();
            WeaponSkins = OverhaulController.AddController<WeaponSkinsController>();
            Outfits = OverhaulController.AddController<PlayerOutfitController>();
            AdditionalAnimations = OverhaulController.AddController<AdditionalAnimationsController>();
            CombatOverhaul = OverhaulController.AddController<CombatOverhaulController>();
            NewWeapons = OverhaulController.AddController<NewWeaponsController>();
            AdvancedGarbageController = OverhaulController.AddController<AdvancedGarbageController>();
            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);
        }

        protected override void OnDisposed()
        {
            Core = null;
            m_MainCamera = null;
            m_CurrentCamera = null;
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
            if (mainCamera != m_MainCamera)
            {
                OverhaulEventManager.DispatchEvent(MainCameraSwitchedEventString, mainCamera);
            }
            m_MainCamera = mainCamera;

            Camera currentCamera = Camera.current;
            if (currentCamera != m_CurrentCamera)
            {
                OverhaulEventManager.DispatchEvent(CurrentCameraSwitchedEventString, currentCamera);
            }
            m_CurrentCamera = currentCamera;
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