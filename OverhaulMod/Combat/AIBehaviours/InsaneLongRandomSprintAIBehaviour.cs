namespace OverhaulMod.Combat.AIBehaviours
{
    public class InsaneLongRandomSprintAIBehaviour : RandomSprintAIBehaviour
    {
        public override float sprintMultiplier
        {
            get
            {
                return 1.35f;
            }
        }

        public override float sprintActivationCooldownOverride
        {
            get
            {
                return 5f;
            }
        }

        public override float sprintMaxIntervalValue
        {
            get
            {
                return 12f;
            }
        }
    }
}
