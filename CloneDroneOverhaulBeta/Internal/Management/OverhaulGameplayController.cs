using CDOverhaul.Gameplay;

namespace CDOverhaul
{
    public class OverhaulGameplayController : OverhaulController
    {
        public override void Initialize()
        {
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
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