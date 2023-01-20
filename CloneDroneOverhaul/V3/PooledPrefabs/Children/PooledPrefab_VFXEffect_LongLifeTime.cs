namespace CloneDroneOverhaul.V3.Gameplay
{
    public class PooledPrefab_VFXEffect_LongLifeTime : PooledPrefab_VFXEffect_ParticleSystemBase
    {
        protected override float LifeTime()
        {
            return 5f;
        }
    }
}
