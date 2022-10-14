using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

namespace CloneDroneOverhaul.Modules
{
    public class VisualsModule : ModuleBase
    {
        private bool isInitialized;

        private ReflectionProbe probe;
        private ParticleSystem worldDustMS1;
        private ParticleSystem worldDustMS0;
        private ParticleSystem worldDustNormal;
        private SimplePooledPrefab swordBlockPooled;
        private SimplePooledPrefab swordFireBlockPooled;
        private SimplePooledPrefab swordBlockMSPooled;
        private SimplePooledPrefab msBodyPartDamagedVFX;
        private SimplePooledPrefab bodyPartDamagedVFX;
        private SimplePooledPrefab bodyPartDamagedWithFireVFX;
        private SimplePooledPrefab bodyPartBurning;
        private SimplePooledPrefab newExplosionVFX;
        private SimplePooledPrefab lavaVoxelsVFX;
        private SimplePooledPrefab floatingLavaParticlesVFX; //VFX_FloatingLava //VFX_HammerHit
        private SimplePooledPrefab hammerHitVFX;
        private SimplePooledPrefab lightVFX;
        private SimplePooledPrefab longLiveightVFX;
        private Camera lastSpottedCamera;

        private bool isWaitingNextFrame;

        public override bool IsEnabled()
        {
            return isInitialized; // SETTINGS!!
        }
        public override void OnActivated()
        {
            GameObject gameObject = new GameObject("CDO_Visuals");
            probe = gameObject.AddComponent<ReflectionProbe>();
            probe.size = new Vector3(4096f, 4096f, 4096f);
            probe.resolution = 64;
            probe.shadowDistance = 1024f;
            probe.intensity = 0.75f;
            probe.nearClipPlane = 0.01f;
            probe.nearClipPlane = 0.3f;
            probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            probe.mode = ReflectionProbeMode.Realtime;
            probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
            Singleton<GlobalEventManager>.Instance.AddEventListener(GlobalEvents.SectionVisibilityChanged, renderReflections);

            worldDustMS1 = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM1")).GetComponent<ParticleSystem>();
            worldDustMS0 = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM0")).GetComponent<ParticleSystem>();
            worldDustNormal = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustNormal")).GetComponent<ParticleSystem>();

            swordBlockPooled = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Block").transform, 10, "VFX_SwordBlock", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            swordFireBlockPooled = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBlock").transform, 50, "VFX_SwordFireBlock", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            swordBlockMSPooled = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_BlockMS").transform, 10, "VFX_SwordBlockMS", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            msBodyPartDamagedVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_CutMS").transform, 10, "VFX_MSCut", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            bodyPartDamagedVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Cut_Normal").transform, 15, "VFX_Cut", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            bodyPartDamagedWithFireVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Cut_Fire").transform, 15, "VFX_FireCut", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            bodyPartBurning = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBurn").transform, 5, "VFX_Burning", 0.25f, SimplePooledPrefabInstance.ParticleSystemTag);
            lavaVoxelsVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ExplosionCubes").transform, 5, "VFX_ExplosionCubes", 0.25f, SimplePooledPrefabInstance.ParticleSystemTag);
            newExplosionVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ExplosionNew").transform, 5, "VFX_NewExplosion", 0.25f, SimplePooledPrefabInstance.ParticleSystemTag);
            floatingLavaParticlesVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FloatingLava").transform, 15, "VFX_FloatingLava", 0.5f, SimplePooledPrefabInstance.ParticleSystemTag);
            hammerHitVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_HammerHit").transform, 10, "VFX_HammerHit", 0.3f, SimplePooledPrefabInstance.ParticleSystemTag);
            lightVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "EmitableLight").transform, 10, "VFX_EmitLight", 1f, SimplePooledPrefabInstance.LightTag);
            longLiveightVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "EmitableLight").transform, 10, "VFX_EmitLongLiveLight", 4f, SimplePooledPrefabInstance.LongLiveLightTag);

            RefreshDustMaterials();

            isInitialized = true;
        }

        private void renderReflections()
        {
            isWaitingNextFrame = true;
        }

        public override void OnNewFrame()
        {
            if (isWaitingNextFrame)
            {
                isWaitingNextFrame = false;
                probe.RenderProbe();
            }

            Camera newCam = Camera.main;
            if (lastSpottedCamera != newCam)
            {
                if (newCam != null)
                {
                    Bloom bloom = newCam.GetComponent<Bloom>();
                    bloom.bloomBlurIterations = 10;
                    bloom.bloomIntensity = 0.7f;
                    bloom.bloomThreshold = 1.25f;
                    bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);
                }
            }
            lastSpottedCamera = Camera.main;
        }
        public override void OnManagedUpdate()
        {
            if (Camera.main != null)
            {
                probe.gameObject.transform.position = Camera.main.transform.position;
                worldDustMS1.gameObject.transform.position = Camera.main.transform.position;
                worldDustMS0.gameObject.transform.position = Camera.main.transform.position;
                worldDustNormal.gameObject.transform.position = Camera.main.transform.position;
            }
            else
            {
                probe.gameObject.transform.position = Vector3.zero;
                worldDustMS1.gameObject.transform.position = Vector3.zero;
                worldDustMS0.gameObject.transform.position = Vector3.zero;
                worldDustNormal.gameObject.transform.position = Vector3.zero;
            }
        }
        public override void OnSecond(float time)
        {
            if (time == 0.5f)
            {
                RefreshDustMaterials();
            }
        }

        private void RefreshDustMaterials()
        {
            RobotShortInformation info = PlayerUtilities.GetPlayerRobotInfo();
            bool useMindspace = false;
            if (!info.IsNull)
            {
                useMindspace = info.IsFPMMindspace;
            }

            worldDustNormal.Stop();
            worldDustMS0.Stop();
            worldDustMS1.Stop();

            if (useMindspace)
            {
                worldDustMS0.Play();
                worldDustMS1.Play();
                worldDustNormal.Stop();
            }
            else
            {
                worldDustMS0.Stop();
                worldDustMS1.Stop();
                worldDustNormal.Play();
            }
        }

        public void EmitSwordBlockVFX(Vector3 pos, bool isFire = false)
        {
            RobotShortInformation info = PlayerUtilities.GetPlayerRobotInfo();
            bool useMindspace = false;
            if (!info.IsNull)
            {
                useMindspace = info.IsFPMMindspace;
            }

            if (useMindspace)
            {
                swordBlockMSPooled.SpawnObject(pos, Vector3.zero, Color.clear);
            }
            else
            {
                if (isFire)
                {
                    swordFireBlockPooled.SpawnObject(pos, Vector3.zero, Color.clear);
                    return;
                }
                swordBlockPooled.SpawnObject(pos + new Vector3(0, 0.1f, 0), Vector3.zero, Color.clear);
            }
        }

        public void EmitMSBodyPartDamage(Vector3 pos)
        {
            msBodyPartDamagedVFX.SpawnObject(pos, Vector3.zero, Color.clear);
        }

        public void EmitBodyPartCutVFX(Vector3 pos, bool isFire)
        {
            if (isFire)
            {
                bodyPartDamagedWithFireVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            }
            else
            {
                bodyPartDamagedVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            }
        }

        public void EmitBurningVFX(Vector3 pos)
        {
            if(Random.Range(0, 10) > 5)
            bodyPartBurning.SpawnObject(pos, Vector3.zero, Color.clear);
        }

        public void EmitExplosion(Vector3 pos)
        {
            newExplosionVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            lavaVoxelsVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            EmitFloatingLavaDust(pos);
            Color col = "FFB59C".hexToColor();
            EmitLongLivingLightVFX(pos, col, 100);
        }

        public void EmitFloatingLavaDust(Vector3 pos)
        {
            floatingLavaParticlesVFX.SpawnObject(pos, Vector3.zero, Color.clear);
        }

        public void EmitHammerHitVFX(Vector3 pos)
        {
            hammerHitVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            Color col = "258AFF".hexToColor();
            EmitLightVFX(pos, col, 15);
        }

        public void EmitLightVFX(Vector3 pos, Color color, float range)
        {
            lightVFX.SpawnObject(pos, Vector3.zero, color).GetComponent<Light>().range = range;
        }

        public void EmitLongLivingLightVFX(Vector3 pos, Color color, float range)
        {
            longLiveightVFX.SpawnObject(pos, Vector3.zero, color).GetComponent<Light>().range = range;
        }
    }

    public class PointLightDust : MonoBehaviour
    {
        public Transform Target;
        private Transform Dust;

        private void Start()
        {
            Dust = Instantiate<Transform>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustLight").transform);
        }

        private void FixedUpdate()
        {
            Dust.transform.position = Target.position;
        }

        private void OnDestroy()
        {
            if (Dust != null)
            {
                Destroy(Dust.gameObject);
            }
        }

        private void OnDisable()
        {
            if (Dust != null)
            {
                Dust.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (Dust != null)
            {
                Dust.gameObject.SetActive(true);
            }
        }
    }
}
