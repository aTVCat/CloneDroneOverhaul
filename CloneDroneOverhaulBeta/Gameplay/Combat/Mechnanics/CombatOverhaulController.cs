namespace CDOverhaul.Gameplay.Combat
{
    public class CombatOverhaulController : OverhaulGameplayController
    {
        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel || !OverhaulGamemodeManager.SupportsCombatOverhaul())
            {
                return;
            }

            _ = firstPersonMover.gameObject.AddComponent<CombatSprintAndStance>();
        }
    }
}
