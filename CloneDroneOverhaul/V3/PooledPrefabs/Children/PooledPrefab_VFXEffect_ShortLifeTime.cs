namespace CloneDroneOverhaul.V3.Gameplay
{
    public class PooledPrefab_VFXEffect_ShortLifeTime : PooledPrefab_VFXEffect_ParticleSystemBase
    {
        protected override float LifeTime()
        {
            return 0.75f;
        }
    }
}
