namespace OverhaulMod.Combat.AIBehaviours
{
    public class InsaneRandomSprintAIBehaviour : RandomSprintAIBehaviour
    {
        public override float sprintMultiplier
        {
            get
            {
                return 1.3f;
            }
        }

        public override float sprintMaxIntervalValue
        {
            get
            {
                return 10f;
            }
        }
    }
}
