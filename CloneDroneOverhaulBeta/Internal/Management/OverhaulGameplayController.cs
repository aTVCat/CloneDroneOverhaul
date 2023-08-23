using CDOverhaul.Gameplay;

namespace CDOverhaul
{
    public class OverhaulGameplayController : OverhaulController
    {
        public override void Initialize()
        {
            OverhaulEventsController.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawned);
            OverhaulEventsController.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelay);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawned);
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelay);
        }

        private void onFirstPersonMoverSpawned(FirstPersonMover mover)
        {
            if (mover == null)
                return;

            OnFirstPersonMoverSpawned(mover, false);
        }

        private void onFirstPersonMoverSpawnedDelay(FirstPersonMover mover)
        {
            if (mover == null)
                return;

            OnFirstPersonMoverSpawned(mover, true);
        }

        public virtual void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel) { }
    }
}