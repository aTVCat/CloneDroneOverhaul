using CDOverhaul.Gameplay.Combat;
using CDOverhaul.Gameplay.Mindspace;
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

        public NewWeaponsController NewWeapons
        {
            get;
            private set;
        }

        public WeaponSkinsController WeaponSkins
        {
            get;
            private set;
        }

        public AdvancedGarbageController AdvancedGarbageController
        {
            get;
            private set;
        }

        public MindspaceOverhaulController MindspaceOverhaul
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            Core = this;

            WeaponSkins = OverhaulController.AddController<WeaponSkinsController>();
            AdditionalAnimations = OverhaulController.AddController<AdditionalAnimationsController>();
            NewWeapons = OverhaulController.AddController<NewWeaponsController>();
            AdvancedGarbageController = OverhaulController.AddController<AdvancedGarbageController>();
            MindspaceOverhaul = OverhaulController.AddController<MindspaceOverhaulController>();
            _ = OverhaulController.AddController<Outfits.OutfitsController>();

            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);
        }

        protected override void OnDisposed()
        {
            Core = null;
            m_MainCamera = null;
            m_CurrentCamera = null;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            GameMode currentGamemode = GameFlowManager.Instance.GetCurrentGameMode();
            if (!currentGamemode.Equals(m_GameModeLastTimeCheck))
            {
                sendGamemodeWasUpdateEvent();
            }
            m_GameModeLastTimeCheck = currentGamemode;

            Camera mainCamera = Camera.main;
            if (!Equals(mainCamera, m_MainCamera))
            {
                OverhaulEventsController.DispatchEvent(MainCameraSwitchedEventString, mainCamera);
            }
            m_MainCamera = mainCamera;

            Camera currentCamera = Camera.current;
            if (!Equals(currentCamera, m_CurrentCamera))
            {
                OverhaulEventsController.DispatchEvent(CurrentCameraSwitchedEventString, currentCamera);
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

            OverhaulEventsController.DispatchEvent(GamemodeChangedEventString);
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