namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefab_VFXEffect_ShortLifeTime : PooledPrefab_VFXEffect_ParticleSystemBase
    {
        protected override float LifeTime()
        {
            return 0.75f;
        }
    }
}
