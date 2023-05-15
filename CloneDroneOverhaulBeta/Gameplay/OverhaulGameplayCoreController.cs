using CDOverhaul.Gameplay.Combat;
using CDOverhaul.Gameplay.Mindspace;
using CDOverhaul.Graphics;
using CDOverhaul.Graphics.Robots;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class OverhaulGameplayCoreController : OverhaulGameplayController
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

        #region Variables

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

        public override void Initialize()
        {
            base.Initialize();
            Core = this;

            _ = OverhaulController.AddController<WeaponSkinsController>();
            _ = OverhaulController.AddController<FirstPersonMoverModdedAnimationsController>();
            _ = OverhaulController.AddController<MindspaceOverhaulController>();
            _ = OverhaulController.AddController<Outfits.OutfitsController>();
            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Core = null;

            m_MainCamera = null;
            m_CurrentCamera = null;
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel || firstPersonMover == null)
            {
                return;
            }

            if (!OverhaulVersion.Upd2Hotfix) firstPersonMover.gameObject.AddComponent<RobotEffectsBehaviour>();

            Camera camera = firstPersonMover.GetPlayerCamera();
            if (camera == null)
            {
                return;
            }

            camera.gameObject.AddComponent<CameraRollingBehaviour>().Initialize(firstPersonMover, camera);
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
    }
}