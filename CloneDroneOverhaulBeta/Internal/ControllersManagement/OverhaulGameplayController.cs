using CDOverhaul.Gameplay;

namespace CDOverhaul
{
    public class OverhaulGameplayController : OverhaulController
    {
        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawned);
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelay);
        }

        protected override void OnDisposed()
        {
            OverhaulEventManager.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFirstPersonMoverSpawned);
            OverhaulEventManager.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, onFirstPersonMoverSpawnedDelay);
        }

        private void onFirstPersonMoverSpawned(FirstPersonMover mover)
        {
            if(mover == null)
            {
                return;
            }
            OnFirstPersonMoverSpawned(mover, false);
        }

        private void onFirstPersonMoverSpawnedDelay(FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }
            OnFirstPersonMoverSpawned(mover, true);
        }

        public virtual void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {

        }

        public override string[] Commands()
        {
            return null;
        }
        public override string OnCommandRan(string[] command)
        {
            return null;
        }
    }
}