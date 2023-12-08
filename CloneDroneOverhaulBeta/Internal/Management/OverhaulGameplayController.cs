using CDOverhaul.Gameplay;

namespace CDOverhaul
{
    public class OverhaulGameplayController : OverhaulController
    {
        public override void Initialize()
        {
            OverhaulEvents.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawned);
            OverhaulEvents.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelay);
        }

        protected override void OnDisposed()
        {
            OverhaulEvents.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawned);
            OverhaulEvents.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelay);
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