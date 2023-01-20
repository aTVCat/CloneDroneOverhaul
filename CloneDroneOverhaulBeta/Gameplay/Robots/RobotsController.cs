namespace CDOverhaul.Gameplay
{
    public class RobotsController : ModController
    {
        public override void Initialize()
        {
            OverhaulEventManager.AddListenerToEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, onFPMSpawned);
        }

        private void onFPMSpawned(FirstPersonMover mover)
        {
            mover.gameObject.AddComponent<RobotDataCollection>();
        }
    }
}
