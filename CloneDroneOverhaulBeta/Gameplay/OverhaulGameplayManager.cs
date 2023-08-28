using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Visuals;

namespace CDOverhaul
{
    public class OverhaulGameplayManager : OverhaulManager<OverhaulGameplayManager>
    {
        public const string FIRST_PERSON_SPAWNED_EVENT = "FPMSpawned";
        public const string FIRST_PERSON_INITIALIZED_EVENT = "FPMInitialized";

        public const string GAMEMODE_CHANGED_EVENT = "GamemodeChanged";

        public const string PLAYER_SET_EVENT = "PlayerSet_Base";

        public OverhaulGameplayEventsSystem events
        {
            get;
            private set;
        }

        public OverhaulPlayerInfosSystem playerInfos
        {
            get;
            set;
        }

        public AutoBuildSystem autoBuild
        {
            get;
            private set;
        }

        public ViewModesSystem viewModes
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            events = base.gameObject.AddComponent<OverhaulGameplayEventsSystem>();
            playerInfos = base.gameObject.AddComponent<OverhaulPlayerInfosSystem>();
            autoBuild = base.gameObject.AddComponent<AutoBuildSystem>();
            viewModes = base.gameObject.AddComponent<ViewModesSystem>();
            /*_ = firstPersonMover.gameObject.AddComponent<CharacterFixExpansion>();
_ = firstPersonMover.gameObject.AddComponent<OverhaulRobotHeadRotator>();
_ = firstPersonMover.gameObject.AddComponent<RobotControlsExpansion>();
_ = firstPersonMover.gameObject.AddComponent<RobotCameraZoomExpansion>();*/
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
        }

        protected override void OnAssetsLoaded()
        {
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public override void AddListeners()
        {
            base.AddListeners();
            OverhaulEventsController.AddEventListener<FirstPersonMover>(FIRST_PERSON_SPAWNED_EVENT, onFirstPersonMoverSpawnedInternal);
            OverhaulEventsController.AddEventListener<FirstPersonMover>(FIRST_PERSON_INITIALIZED_EVENT, onFirstPersonMoverSpawnedDelayInternal);

            events?.AddListeners();
            playerInfos?.AddListeners();
            autoBuild?.AddListeners();
            viewModes?.AddListeners();
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(FIRST_PERSON_SPAWNED_EVENT, onFirstPersonMoverSpawnedInternal);
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(FIRST_PERSON_INITIALIZED_EVENT, onFirstPersonMoverSpawnedDelayInternal);

            events?.RemoveListeners();
            playerInfos?.RemoveListeners();
            autoBuild?.RemoveListeners();
            viewModes?.RemoveListeners();
        }

        private void onFirstPersonMoverSpawnedInternal(FirstPersonMover mover)
        {
            if (!mover)
                return;

            OnFirstPersonMoverSpawned(mover, false);
        }

        private void onFirstPersonMoverSpawnedDelayInternal(FirstPersonMover mover)
        {
            if (!mover || !mover.HasCharacterModel())
                return;

            OnFirstPersonMoverSpawned(mover, true);
        }

        public void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool initializedModel)
        {
            events.OnFirstPersonMoverSpawned(firstPersonMover, initializedModel);
            autoBuild.OnFirstPersonMoverSpawned(firstPersonMover, initializedModel);
            playerInfos.OnFirstPersonMoverSpawned(firstPersonMover, initializedModel);
            viewModes.OnFirstPersonMoverSpawned(firstPersonMover, initializedModel);
        }
    }
}