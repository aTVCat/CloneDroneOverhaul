namespace OverhaulMod.Combat.Enemies
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
