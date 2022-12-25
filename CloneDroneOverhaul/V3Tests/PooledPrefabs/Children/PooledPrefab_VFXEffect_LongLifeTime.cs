namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefab_VFXEffect_LongLifeTime : PooledPrefab_VFXEffect_ParticleSystemBase
    {
        protected override float LifeTime()
        {
            return 5f;
        }
    }
}
