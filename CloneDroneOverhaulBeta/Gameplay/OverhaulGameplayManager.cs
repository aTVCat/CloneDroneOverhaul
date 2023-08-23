using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.QualityOfLife;

namespace CDOverhaul
{
    public class OverhaulGameplayManager : OverhaulManager<OverhaulGameplayManager>
    {
        public AutoBuildSystem autoBuild
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            autoBuild = base.gameObject.AddComponent<AutoBuildSystem>();
        }

        public override void OnSceneReloaded()
        {
            base.OnAssetsLoaded();
        }

        protected override void OnAssetsLoaded()
        {
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            OverhaulEventsController.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawnedInternal);
            OverhaulEventsController.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelayInternal);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawnedInternal);
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelayInternal);
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
            autoBuild.OnFirstPersonMoverSpawned(firstPersonMover, initializedModel);
        }
    }
}