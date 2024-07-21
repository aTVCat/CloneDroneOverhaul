using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class ParticleManager : Singleton<ParticleManager>
    {
        public const string FIRE_PARTICLES_ID = "VFX_FireParticles";

        public const string FIRE_CUT_PARTICLES_ID = "VFX_CutFire";

        public const string LASER_CUT_PARTICLES_ID = "VFX_CutLaser";

        public const string SPARKS_PARTICLES_ID = "VFX_Sparks";

        [ModSetting(ModSettingsConstants.ENABLE_NEW_PARTICLES, true)]
        public static bool EnableParticles;

        private void Start()
        {
            addParticles();
        }

        private void addParticles()
        {
            PooledPrefabManager pooledPrefabManager = PooledPrefabManager.Instance;
            pooledPrefabManager.MakePooledPrefab(FIRE_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_FireParticles", 5f, 50);
            pooledPrefabManager.MakePooledPrefab(FIRE_CUT_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_CutFire", 0.5f, 100);
            pooledPrefabManager.MakePooledPrefab(LASER_CUT_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_CutLaser", 0.5f, 100);
            pooledPrefabManager.MakePooledPrefab(SPARKS_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_Sparks", 0.3f, 100);
        }

        public void SpawnFireParticles(Vector3 position)
        {
            PooledPrefabManager.Instance.SpawnObject(FIRE_PARTICLES_ID, position);
        }

        public void SpawnFireCutParticles(Vector3 position)
        {
            PooledPrefabManager.Instance.SpawnObject(FIRE_CUT_PARTICLES_ID, position);
        }

        public void SpawnLaserCutParticles(Vector3 position)
        {
            PooledPrefabManager.Instance.SpawnObject(LASER_CUT_PARTICLES_ID, position);
        }

        public void SpawnSparksParticles(Vector3 position)
        {
            PooledPrefabManager.Instance.SpawnObject(SPARKS_PARTICLES_ID, position);
        }
    }
}
