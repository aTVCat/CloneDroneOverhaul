using CDOverhaul.Gameplay.Combat_Update;

namespace CDOverhaul.Gameplay
{
    public class RobotsController : ModController
    {
        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, onFPMSpawned);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        private void onFPMSpawned(FirstPersonMover mover)
        {
            _ = mover.gameObject.AddComponent<FirstPersonMoverData>();
            _ = mover.gameObject.AddComponent<FirstPersonMoverOverhaul>();
            _ = mover.gameObject.AddComponent<DropThrowNTakeWeapons>();
        }
    }
}
