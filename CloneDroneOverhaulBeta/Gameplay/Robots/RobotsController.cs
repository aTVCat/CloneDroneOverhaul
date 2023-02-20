using CDOverhaul.Gameplay.Combat_Update;

namespace CDOverhaul.Gameplay
{
    public class RobotsController : ModController
    {
        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, onFPMSpawned);

            HasAddedEventListeners = true;
            HasInitialized = true;
        }

        private void onFPMSpawned(FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }

            _ = mover.gameObject.AddComponent<FirstPersonMoverData>();
            _ = mover.gameObject.AddComponent<FirstPersonMoverOverhaul>();
            _ = mover.gameObject.AddComponent<DropThrowNTakeWeapons>();
            _ = mover.gameObject.AddComponent<AdvancedWeaponController>();
            _ = mover.gameObject.AddComponent<FirstPersonMoverInteractionBehaviour>();
        }
    }
}
