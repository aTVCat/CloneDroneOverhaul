namespace OverhaulMod.Combat.Enemies
{
    public class InsaneBackwardDashAIBehaviour : BackwardDashAIBehaviour
    {
        public override float dashMultiplier
        {
            get
            {
                return -30f;
            }
        }
    }
}
