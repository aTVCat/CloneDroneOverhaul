namespace OverhaulMod.Combat.AIBehaviours
{
    public class InsaneBackDashAIBehaviour : BackDashAIBehaviour
    {
        public virtual float dashMultiplier
        {
            get
            {
                return -30f;
            }
        }
    }
}
