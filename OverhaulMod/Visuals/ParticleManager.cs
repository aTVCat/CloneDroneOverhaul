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

        public const string SWORD_BLOCK_PARTICLES_ID = "VFX_SwordBlock";

        public const string FIRE_SWORD_BLOCK_PARTICLES_ID = "VFX_FireSwordBlock";

        public const string HAMMER_HIT_ID = "VFX_HammerHit";

        public const string RED_GRENADE_EXPLOSION = "VFX_RedGrenadeExplosion";

        public const string BLUE_GRENADE_EXPLOSION = "VFX_BlueGrenadeExplosion";

        public const string LOGO_EXPLOSION_PARTICLES_ID = "VFX_LogoExplosion";

        [ModSetting(ModSettingIDs.ENABLE_HIT_PARTICLES, true)]
        public static bool EnableHitParticles;

        [ModSetting(ModSettingIDs.NEW_EXPLOSION_PARTICLES, true)]
        public static bool NewExplosionParticles;

        [ModSetting(ModSettingIDs.REDUCE_FLASHES, false)]
        public static bool ReduceFlashes;

        [ModSetting(ModSettingIDs.NEW_WELDING_PARTICLES, true)]
        public static bool ReworkWeldingParticles;

        private GameObject _logoParticles;

        private void Start()
        {
            addParticles();
        }

        private void addParticles()
        {
            PooledPrefabManager pooledPrefabManager = PooledPrefabManager.Instance;
            pooledPrefabManager.MakePooledPrefab(FIRE_PARTICLES_ID, ModAssetBundles.VFX, "VFX_FireParticles", 5f, 25);
            pooledPrefabManager.MakePooledPrefab(FIRE_CUT_PARTICLES_ID, ModAssetBundles.VFX, "VFX_CutFire", 0.5f, 50);
            pooledPrefabManager.MakePooledPrefab(LASER_CUT_PARTICLES_ID, ModAssetBundles.VFX, "VFX_CutLaser", 0.5f, 20);
            pooledPrefabManager.MakePooledPrefab(SPARKS_PARTICLES_ID, ModAssetBundles.VFX, "VFX_Sparks", 0.3f, 20);
            pooledPrefabManager.MakePooledPrefab(SWORD_BLOCK_PARTICLES_ID, ModAssetBundles.VFX, "VFX_SwordBlock", 0.45f, 20);
            pooledPrefabManager.MakePooledPrefab(FIRE_SWORD_BLOCK_PARTICLES_ID, ModAssetBundles.VFX, "VFX_FireSwordBlock", 0.45f, -1);
            pooledPrefabManager.MakePooledPrefab(HAMMER_HIT_ID, ModAssetBundles.VFX, "VFX_HammerHit", 0.6f, 20);
            pooledPrefabManager.MakePooledPrefab(RED_GRENADE_EXPLOSION, ModAssetBundles.VFX, "VFX_RedGrenadeExplosion", 1.25f, 20);
            pooledPrefabManager.MakePooledPrefab(BLUE_GRENADE_EXPLOSION, ModAssetBundles.VFX, "VFX_BlueGrenadeExplosion", 1.25f, 20);
            pooledPrefabManager.MakePooledPrefab(LOGO_EXPLOSION_PARTICLES_ID, GetCloneDroneLogoExplosionParticles(), 2.5f, 20); // todo: this no longer works. make custom particles
        }

        private void adjustParticleSettings(ParticleSystem particleSystem)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.simulationSpeed *= 0.75f;
        }

        public GameObject GetCloneDroneLogoExplosionParticles()
        {
            if (!_logoParticles)
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

                    _logoParticles = transform1.gameObject;
                }
            }
            return _logoParticles;
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

        public void SpawnSwordBlockParticles(Vector3 position)
        {
            Transform t = PooledPrefabManager.Instance.SpawnObject(SWORD_BLOCK_PARTICLES_ID, position);

            toggleFlashesForParticles(t, 1);
        }

        public void SpawnFireSwordBlockParticles(Vector3 position)
        {
            Transform t = PooledPrefabManager.Instance.SpawnObject(FIRE_SWORD_BLOCK_PARTICLES_ID, position);

            toggleFlashesForParticles(t, 1);
        }

        public void SpawnHammerHitParticles(Vector3 position)
        {
            PooledPrefabManager.Instance.SpawnObject(HAMMER_HIT_ID, position);
        }

        public void SpawnRedGrenadeExplosionParticles(Vector3 position)
        {
            Transform t = PooledPrefabManager.Instance.SpawnObject(RED_GRENADE_EXPLOSION, position);

            toggleFlashesForParticles(t, 2);
        }

        public void SpawnBlueGrenadeExplosionParticles(Vector3 position)
        {
            Transform t = PooledPrefabManager.Instance.SpawnObject(BLUE_GRENADE_EXPLOSION, position);
        }

        private void toggleFlashesForParticles(Transform particlesTransform, int flashesCount)
        {
            Transform c0 = particlesTransform.GetChild(0);
            if (c0) c0.gameObject.SetActive(!ReduceFlashes);

            if (flashesCount < 2) return;

            Transform c1 = particlesTransform.GetChild(1);
            if (c1) c1.gameObject.SetActive(!ReduceFlashes);
        }
    }
}
