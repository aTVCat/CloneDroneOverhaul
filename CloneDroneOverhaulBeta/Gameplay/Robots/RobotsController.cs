namespace CDOverhaul.Gameplay
{
    public class RobotsController : ModController
    {
        public override void Initialize()
        {
            OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, onFPMSpawned);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        private void onFPMSpawned(FirstPersonMover mover)
        {
            mover.gameObject.AddComponent<RobotDataCollection>();
            mover.gameObject.AddComponent<FirstPersonMoverOverhaul>();
        }
    }
}
