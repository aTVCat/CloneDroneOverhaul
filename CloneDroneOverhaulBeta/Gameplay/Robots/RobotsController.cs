namespace CDOverhaul.Gameplay
{
    public class RobotsController : ModController
    {
        public override void Initialize()
        {
            OverhaulEventManager.AddListenerToEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, onFPMSpawned);
            OverhaulEventManager.AddEvent(RobotDataCollection.DataCollectionInitializedEventString);

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
