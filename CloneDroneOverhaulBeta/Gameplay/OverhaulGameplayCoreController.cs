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

        private GameMode m_GamemodePrevFrame;

        private Camera m_MainCamera;
        private Camera m_CurrentCamera;

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            _ = OverhaulController.AddController<MindspaceOverhaulController>();
            _ = OverhaulController.AddController<FirstPersonMoverModdedAnimationsController>();
            _ = OverhaulController.AddController<WeaponSkinsController>();
            _ = OverhaulController.AddController<Outfits.OutfitsController>();

            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            m_MainCamera = null;
            m_CurrentCamera = null;
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (IsDisposedOrDestroyed() || !hasInitializedModel || firstPersonMover == null)
                return;

            if (!OverhaulVersion.IsUpdate2Hotfix) 
                firstPersonMover.gameObject.AddComponent<RobotEffectsBehaviour>();

            firstPersonMover.GetPlayerCamera()?.gameObject.AddComponent<CameraRollingBehaviour>().Initialize(firstPersonMover);
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
                return;

            GameMode currentGamemode = GameFlowManager.Instance.GetCurrentGameMode();
            if (!currentGamemode.Equals(m_GamemodePrevFrame))
                sendGamemodeWasUpdateEvent();
            m_GamemodePrevFrame = currentGamemode;

            Camera mainCamera = Camera.main;
            if (!Equals(mainCamera, m_MainCamera))
                OverhaulEventsController.DispatchEvent(MainCameraSwitchedEventString, mainCamera);
            m_MainCamera = mainCamera;

            Camera currentCamera = Camera.current;
            if (!Equals(currentCamera, m_CurrentCamera))
                OverhaulEventsController.DispatchEvent(CurrentCameraSwitchedEventString, currentCamera);
            m_CurrentCamera = currentCamera;
        }

        /// <summary>
        /// Send <see cref="GamemodeChangedEventString"/> event
        /// </summary>
        private void sendGamemodeWasUpdateEvent()
        {
            if (IsDisposedOrDestroyed())
                return;

            OverhaulEventsController.DispatchEvent(GamemodeChangedEventString);
        }
    }
}