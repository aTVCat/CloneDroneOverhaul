using ModLibrary;
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

        public const string LOGO_EXPLOSION_PARTICLES_ID = "VFX_LogoExplosion";

        [ModSetting(ModSettingsConstants.ENABLE_NEW_PARTICLES, true)]
        public static bool EnableParticles;

        private GameObject m_logoParticles;

        private void Start()
        {
            addParticles();
        }

        private void addParticles()
        {
            PooledPrefabManager pooledPrefabManager = PooledPrefabManager.Instance;
            pooledPrefabManager.MakePooledPrefab(FIRE_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_FireParticles", 5f, 25);
            pooledPrefabManager.MakePooledPrefab(FIRE_CUT_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_CutFire", 0.5f, 50);
            pooledPrefabManager.MakePooledPrefab(LASER_CUT_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_CutLaser", 0.5f, 20);
            pooledPrefabManager.MakePooledPrefab(SPARKS_PARTICLES_ID, AssetBundleConstants.VFX, "VFX_Sparks", 0.3f, 20);
            pooledPrefabManager.MakePooledPrefab(LOGO_EXPLOSION_PARTICLES_ID, GetCloneDroneLogoExplosionParticles(), 2.5f, 20);
        }

        private void adjustParticleSettings(ParticleSystem particleSystem)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.simulationSpeed *= 0.75f;
        }

        public GameObject GetCloneDroneLogoExplosionParticles()
        {
            if (!m_logoParticles)
            {
                Transform transform = TransformUtils.FindChildRecursive(ArenaCameraManager.Instance.TitleScreenLogo.transform, "fireExplosion");
                if (transform)
                {
                    Transform transform1 = Instantiate(transform);
                    DontDestroyOnLoad(transform1.gameObject);
                    transform1.gameObject.SetActive(false);
                    transform1.gameObject.layer = Layers.Default;
                    for (int i = 0; i < transform1.childCount; i++)
                    {
                        transform1.GetChild(i).gameObject.layer = Layers.Default;
                        adjustParticleSettings(transform1.GetChild(i).GetComponent<ParticleSystem>());
                    }
                    adjustParticleSettings(transform1.GetComponent<ParticleSystem>());

                    m_logoParticles = transform1.gameObject;
                }
            }
            return m_logoParticles;
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

        public void SpawnLogoExplosionParticles(Vector3 position)
        {
            PooledPrefabManager.Instance.SpawnObject(LOGO_EXPLOSION_PARTICLES_ID, position, Vector3.zero, Vector3.one * 0.55f);
        }

        public void SpawnLogoExplosionParticles(Vector3 position, Vector3 scale)
        {
            PooledPrefabManager.Instance.SpawnObject(LOGO_EXPLOSION_PARTICLES_ID, position, Vector3.zero, scale);
        }
    }
}
