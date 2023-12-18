namespace OverhaulMod.Combat.AIBehaviours
{
    public class InsaneBackwardDashAIBehaviour : BackwardDashAIBehaviour
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
