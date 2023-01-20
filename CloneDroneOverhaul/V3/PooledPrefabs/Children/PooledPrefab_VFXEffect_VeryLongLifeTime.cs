namespace CloneDroneOverhaul.V3.Gameplay
{
    public class PooledPrefab_VFXEffect_VeryLongLifeTime : PooledPrefab_VFXEffect_ParticleSystemBase
    {
        protected override float LifeTime()
        {
            return 10f;
        }
    }
}
