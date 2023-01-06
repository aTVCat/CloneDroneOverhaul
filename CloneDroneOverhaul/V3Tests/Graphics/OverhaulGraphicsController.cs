using AmplifyOcclusion;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.V3Tests.Base;
using ModLibrary;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class OverhaulGraphicsController : V3_ModControllerBase
    {
        #region Constants

        public static readonly Color SwordBlock_Normal_LightColor = BaseUtils.ColorFromHex("#FFF4C1");

        #endregion

        #region RendererParameters

        public static AntialiasingLevel AntiAliasingLevel = AntialiasingLevel.Off;

        #endregion

        #region VFX

        public const string ID_VFX_SWORDBLOCK = "VFX_SwordBlock";
        public const string ID_VFX_SWORDBLOCK_FIRE = "VFX_FireSwordBlock";
        public const string ID_VFX_ARROW_COLLIDE = "VFX_ArrowCollide";

        public const string ID_VFX_BLOCK_MINDSPACE = "VFX_MindspaceBlock";
        public const string ID_VFX_HIT_MINDSPACE = "VFX_MindspaceHit";
        public const string ID_VFX_CUT_MINDSPACE = "VFX_MindspaceCut";

        public const string ID_VFX_CUT = "VFX_Cut";
        public const string ID_VFX_CUT_FIRE = "VFX_FireCut";
        public const string ID_VFX_HAMMER_HIT = "VFX_HammerHit";
        public const string ID_VFX_KICK = "VFX_Kick";

        public const string ID_VFX_EXPLOSION_VOXELS = "VFX_Explosion_Voxels";
        public const string ID_VFX_EXPLOSION = "VFX_Explosion";

        public const string ID_VFX_FLOATING_MAGMA = "VFX_FloatingMagma";
        public const string ID_VFX_BURNING = "VFX_Burning";

        public const string ID_VFX_LIGHT = "VFX_Light";
        public const string ID_VFX_JUMPDASH = "VFX_JumpDash";

        /// <summary>
        /// Initialize class
        /// </summary>
        public static void Initialize()
        {
            setUpPooledPrefabs();
            SpawnReflectionProbe();
        }

        /// <summary>
        /// Spawn pooled prefabs
        /// </summary>
        private static void setUpPooledPrefabs()
        {
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Block").transform, 5, ID_VFX_SWORDBLOCK);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBlock").transform, 50, ID_VFX_SWORDBLOCK_FIRE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_BlockMS").transform, 5, ID_VFX_BLOCK_MINDSPACE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_MSHit").transform, 5, ID_VFX_HIT_MINDSPACE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_CutMS").transform, 10, ID_VFX_CUT_MINDSPACE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Cut_Normal").transform, 10, ID_VFX_CUT);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Cut_Fire").transform, 10, ID_VFX_CUT_FIRE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBurn").transform, 10, ID_VFX_BURNING);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_VeryLongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ExplosionCubes").transform, 5, ID_VFX_EXPLOSION_VOXELS);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ExplosionNew").transform, 5, ID_VFX_EXPLOSION);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FloatingLava").transform, 10, ID_VFX_FLOATING_MAGMA);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_HammerHit").transform, 5, ID_VFX_HAMMER_HIT);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_Light>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "EmitableLight").transform, 5, ID_VFX_LIGHT);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Kick").transform, 5, ID_VFX_KICK);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_JumpDash").transform, 5, ID_VFX_JUMPDASH);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ArrowCollision").transform, 5, ID_VFX_ARROW_COLLIDE);
        }

        /// <summary>
        /// Spawn VFX effect in world
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void UseVFX(in string id, in Vector3 position, in Vector3 rotation)
        {
            PooledPrefabController.SpawnObject<PooledPrefabInstanceBase>(id, position, rotation);
        }

        /// <summary>
        /// Spawn VFX effect in world
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static T UseVFX<T>(in string id, in Vector3 position, in Vector3 rotation) where T : PooledPrefabInstanceBase
        {
            return PooledPrefabController.SpawnObject<T>(id, position, rotation);
        }

        #endregion

        #region VFXSpawns

        public static void Simulate_SwordBlock(in Vector3 position, in bool isFire)
        {
            GameStatistic statistic = GameStatisticsController.GameStatistics;
            CloneDroneOverhaul.Utilities.RobotShortInformation info = statistic.PlayerRobotInformation;
            if(info == null || info.IsNull)
            {
                GameStatisticsController.GetInstance<GameStatisticsController>().TrySetPlayer();
                return;
            }
            bool isMindspace = info.IsFPMMindspace;

            Vector3 rot = Vector3.zero;

            if (isMindspace)
            {
                UseVFX(ID_VFX_BLOCK_MINDSPACE, position, rot);
                UseVFX(ID_VFX_HIT_MINDSPACE, position, rot);
                return;
            }
            else
            {
                if (isFire)
                {
                    UseVFX(ID_VFX_SWORDBLOCK_FIRE, position, rot);
                }
                else
                {
                    UseVFX(ID_VFX_SWORDBLOCK, position, rot);
                }
            }
        }

        public static void Simulate_ArrowCollision(in Vector3 position)
        {
            UseVFX(ID_VFX_ARROW_COLLIDE, position, Vector3.zero);
        }

        public static void Simulate_Kick(in Vector3 position)
        {
            UseVFX(ID_VFX_KICK, position, Vector3.zero);
        }

        public static void Simulate_Mindspace_Hit(in Vector3 position)
        {
            UseVFX(ID_VFX_HIT_MINDSPACE, position, Vector3.zero);
        }

        public static void Simulate_Smoke(in Vector3 position)
        {
            UseVFX(ID_VFX_FLOATING_MAGMA, position, Vector3.zero);
        }

        public static void Simulate_Light(in Vector3 position, in float range, in Color color, in float speed = 1f)
        {
            PooledPrefab_VFXEffect_Light light = UseVFX<PooledPrefab_VFXEffect_Light>(ID_VFX_LIGHT, position, Vector3.zero);
            light.SetLightSettings(range, color, speed);
        }

        public static void Simulate_Light_Hex(in Vector3 position, in float range, in string hexColor, in float speed = 1f)
        {
            PooledPrefab_VFXEffect_Light light = UseVFX<PooledPrefab_VFXEffect_Light>(ID_VFX_LIGHT, position, Vector3.zero);
            if (light != null)
            {
                light.SetLightSettings(range, hexColor.hexToColor(), speed, LightShadows.None);
            }
        }

        public static void Simulate_HammerHit(in Vector3 position)
        {
            Simulate_Light_Hex(position, 15, "258AFF");
            UseVFX(ID_VFX_HAMMER_HIT, position, Vector3.zero);
        }

        public static void Simulate_Cut(in Vector3 position, in bool isFire)
        {
            UseVFX(isFire ? ID_VFX_CUT_FIRE : ID_VFX_CUT, position, Vector3.zero);
        }

        public static void Simulate_Burning(in Vector3 position)
        {
            if (UnityEngine.Random.Range(0, 10) > 5)
            {
                UseVFX(ID_VFX_BURNING, position, Vector3.zero);
            }
        }

        public static void Simulate_Explosion(in Vector3 position)
        {
            UseVFX(ID_VFX_EXPLOSION_VOXELS, position, Vector3.zero);
            Simulate_Smoke(position);
            Simulate_Light_Hex(position, 100, "FFB59C", 0.3f);
        }

        #endregion

        #region Effects

        private static ReflectionProbe _reflectionProbe;


        /// <summary>
        /// Spawn a reflection probe that shades the environment
        /// </summary>
        internal static void SpawnReflectionProbe()
        {
            GameObject gameObject = new GameObject("Overhaul_Shading[ReflectionProbe]");
            _reflectionProbe = gameObject.AddComponent<ReflectionProbe>();
            _reflectionProbe.size = new Vector3(4096f, 4096f, 4096f);
            _reflectionProbe.resolution = 64;
            _reflectionProbe.shadowDistance = 1024f;
            _reflectionProbe.intensity = 0.75f;
            _reflectionProbe.nearClipPlane = 0.01f;
            _reflectionProbe.nearClipPlane = 0.3f;
            _reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
            _reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            _reflectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.NoTimeSlicing;
            Singleton<GlobalEventManager>.Instance.AddEventListener(GlobalEvents.SectionVisibilityChanged, delegate
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    _reflectionProbe.RenderProbe();
                }, 0.02f);
            });
        }

        #endregion

        #region Settings

        private bool _overrideSettings;
        public static bool OverrideSettings
        {
            get => GetInstance<OverhaulGraphicsController>()._overrideSettings;
            set => GetInstance<OverhaulGraphicsController>()._overrideSettings = value;
        }

        public static (bool, bool) AmplifyOcclusionEnabled;
        public static (float, float) AmplifyOcclusionIntenisty;
        public static (SampleCountLevel, SampleCountLevel) AmplifyOcclusionSampleCount;

        public static (ShadowBias, ShadowBias) ShadowsBias;
        public static (ShadowDistance, ShadowDistance) ShadowsDistance;
        public static (Modules.ShadowResolution, Modules.ShadowResolution) ShadowsResolution;
        public static (bool, bool) ShadowsSoft;

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Graphics.Additions.Shading")
            {
                _reflectionProbe.enabled = (bool)value;
            }
            if (settingName == "Graphics.Settings.Antialiasing")
            {
                AntiAliasingLevel = (AntialiasingLevel)(int)value;
                switch (AntiAliasingLevel)
                {
                    case AntialiasingLevel.Off:
                        QualitySettings.antiAliasing = 0;
                        break;
                    case AntialiasingLevel.X2:
                        QualitySettings.antiAliasing = 2;
                        break;
                    case AntialiasingLevel.X4:
                        QualitySettings.antiAliasing = 4;
                        break;
                    case AntialiasingLevel.X8:
                        QualitySettings.antiAliasing = 8;
                        break;
                    default:
                        QualitySettings.antiAliasing = 4;
                        break;
                }
            }
            if (settingName == "Graphics.Additions.Sample count")
            {
                SetParameterValue(EGraphicsParameterNames.AmplifyOcclusionSampleCount, value, false);
            }
            if (settingName == "Graphics.Additions.Amplify occlusion")
            {
                SetParameterValue(EGraphicsParameterNames.AmplifyOcclusionEnabled, value, false);
            }
            if (settingName == "Graphics.Additions.Occlusion intensity")
            {
                SetParameterValue(EGraphicsParameterNames.AmplifyOcclusionIntensity, value, false);
            }
            if (settingName == "Graphics.Settings.Shadow resolution")
            {
                SetParameterValue(EGraphicsParameterNames.ShadowsResolution, value, false);
            }
            if (settingName == "Graphics.Settings.Shadow bias")
            {
                SetParameterValue(EGraphicsParameterNames.ShadowsBias, value, false);
            }
            if (settingName == "Graphics.Settings.Soft shadows")
            {
                SetParameterValue(EGraphicsParameterNames.ShadowsSoft, value, false);
            }
            if (settingName == "Graphics.Settings.Shadow distance")
            {
                SetParameterValue(EGraphicsParameterNames.ShadowsDistance, value, false);
            }
        }

        #region Set parameter
        /// <summary>
        /// Set and refresh graphics parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <param name="overrideSetting"></param>
        public static void SetParameterValue(in EGraphicsParameterNames parameter, in object value, in bool overrideSetting)
        {
            switch (parameter)
            {
                case EGraphicsParameterNames.AmplifyOcclusionEnabled:

                    if (overrideSetting)
                    {
                        AmplifyOcclusionEnabled.Item1 = (bool)value;
                    }
                    else
                    {
                        AmplifyOcclusionEnabled.Item2 = (bool)value;
                    }

                    break;

                case EGraphicsParameterNames.AmplifyOcclusionSampleCount:

                    if (overrideSetting)
                    {
                        AmplifyOcclusionSampleCount.Item1 = (SampleCountLevel)value;
                    }
                    else
                    {
                        AmplifyOcclusionSampleCount.Item2 = (SampleCountLevel)value;
                    }

                    break;

                case EGraphicsParameterNames.AmplifyOcclusionIntensity:

                    if (overrideSetting)
                    {
                        AmplifyOcclusionIntenisty.Item1 = (float)value;
                    }
                    else
                    {
                        AmplifyOcclusionIntenisty.Item2 = (float)value;
                    }

                    break;

                case EGraphicsParameterNames.ShadowsBias:

                    if (overrideSetting)
                    {
                        ShadowsBias.Item1 = (ShadowBias)value;
                    }
                    else
                    {
                        ShadowsBias.Item2 = (ShadowBias)value;
                    }

                    break;

                case EGraphicsParameterNames.ShadowsDistance:

                    if (overrideSetting)
                    {
                        ShadowsDistance.Item1 = (ShadowDistance)value;
                    }
                    else
                    {
                        ShadowsDistance.Item2 = (ShadowDistance)value;
                    }

                    break;

                case EGraphicsParameterNames.ShadowsResolution:

                    if (overrideSetting)
                    {
                        ShadowsResolution.Item1 = (Modules.ShadowResolution)value;
                    }
                    else
                    {
                        ShadowsResolution.Item2 = (Modules.ShadowResolution)value;
                    }

                    break;

                case EGraphicsParameterNames.ShadowsSoft:

                    if (overrideSetting)
                    {
                        ShadowsSoft.Item1 = (bool)value;
                    }
                    else
                    {
                        ShadowsSoft.Item2 = (bool)value;
                    }

                    break;
            }

            RefreshGraphics();
        }
        #endregion

        public static void RefreshGraphics()
        {
            OverhaulGraphicsController controller = GetInstance<OverhaulGraphicsController>();

            AmplifyOcclusionEffect amplifyOcclusionEffect = controller._cachedAmplifyOcclusion;
            if (amplifyOcclusionEffect)
            {
                amplifyOcclusionEffect.enabled = OverrideSettings ? AmplifyOcclusionEnabled.Item1 : AmplifyOcclusionEnabled.Item2;
                amplifyOcclusionEffect.Intensity = OverrideSettings ? AmplifyOcclusionIntenisty.Item1 : AmplifyOcclusionIntenisty.Item2;
                amplifyOcclusionEffect.SampleCount = OverrideSettings ? AmplifyOcclusionSampleCount.Item1 : AmplifyOcclusionSampleCount.Item2;
            }

            Modules.ShadowDistance shadowDistance = (OverrideSettings ? ShadowsDistance.Item1 : ShadowsDistance.Item2);
            switch (shadowDistance)
            {
                case Modules.ShadowDistance.Default:
                    QualitySettings.shadowDistance = 300;
                    break;

                case Modules.ShadowDistance.VeryLow:
                    QualitySettings.shadowDistance = 100;
                    break;

                case Modules.ShadowDistance.Low:
                    QualitySettings.shadowDistance = 200;
                    break;

                case Modules.ShadowDistance.High:
                    QualitySettings.shadowDistance = 500;
                    break;

                case Modules.ShadowDistance.VeryHigh:
                    QualitySettings.shadowDistance = 750;
                    break;

                case Modules.ShadowDistance.ExtremlyHigh:
                    QualitySettings.shadowDistance = 1000;
                    break;
            }

            PatchLight(DirectionalLightManager.Instance.DirectionalLight, true);
        }

        #endregion

        #region Trackers

        private AmplifyOcclusionEffect _cachedAmplifyOcclusion;
        private Camera _cachedCamera;
        /// <summary>
        /// Quickly get main camera component
        /// </summary>
        public static Camera CachedMainCamera
        {
            get => GetInstance<OverhaulGraphicsController>()._cachedCamera;
            set
            {
                OverhaulGraphicsController g = GetInstance<OverhaulGraphicsController>();
                g._cachedCamera = value;
                g.PatchCamera(value);
            }
        }

        private void Update()
        {
            if (CachedMainCamera == null || !CachedMainCamera.isActiveAndEnabled)
            {
                CachedMainCamera = Camera.main;
                HUD.UIImageEffects.RefreshImageEffects();
            }
        }

        #endregion

        #region Patches

        public void PatchCamera(in Camera camera)
        {
            if (camera == null || camera.orthographic)
            {
                return;
            }

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom)
            {
                bloom.bloomBlurIterations = 10;
                bloom.bloomIntensity = 0.7f;
                bloom.bloomThreshold = 1.25f;
                bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);
            }

            if (_cachedAmplifyOcclusion)
            {
                Destroy(_cachedAmplifyOcclusion);
                _cachedAmplifyOcclusion = null;
            }

            if (_cachedAmplifyOcclusion == null)
            {
                AmplifyOcclusionEffect acc = camera.gameObject.AddComponent<AmplifyOcclusionEffect>();
                acc.BlurSharpness = 4f;
                acc.FilterResponse = 0.7f;
                acc.Bias = 0.8f;
                acc.ApplyMethod = AmplifyOcclusionEffect.ApplicationMethod.Deferred;
                _cachedAmplifyOcclusion = acc;
                RefreshGraphics();
            }
        }

        public static void PatchLight(in Light light, in bool enableShadows)
        {
            Modules.ShadowResolution enumRes = (OverrideSettings ? ShadowsResolution.Item1 : ShadowsResolution.Item2);
            switch (enumRes)
            {
                case Modules.ShadowResolution.Low:
                    light.shadowCustomResolution = 1000;
                    break;

                case Modules.ShadowResolution.Default:
                    light.shadowCustomResolution = -1;
                    break;

                case Modules.ShadowResolution.High:
                    light.shadowCustomResolution = 5000;
                    break;

                case Modules.ShadowResolution.ExtremlyHigh:
                    light.shadowCustomResolution = 10000;
                    break;

            }

            ShadowBias shadowBias = (OverrideSettings ? ShadowsBias.Item1 : ShadowsBias.Item2);
            switch (shadowBias)
            {
                case ShadowBias.Minimum:
                    light.shadowBias = 0;
                    break;

                case ShadowBias.Low:
                    light.shadowBias = 0.2f;
                    break;

                case ShadowBias.Default:
                    light.shadowBias = 1f;
                    break;

            }

            LightShadows shadowsMode = LightShadows.Soft;
            int qualityLevel = SettingsManager.Instance.GetSavedQualityIndex();
            if (qualityLevel == 0)
            {
                shadowsMode = LightShadows.None;
            }
            else
            {
                shadowsMode = (OverrideSettings ? ShadowsSoft.Item1 : ShadowsSoft.Item2) ? LightShadows.Soft : LightShadows.Hard;
            }
            light.shadows = enableShadows ? shadowsMode : LightShadows.None;
        }

        #endregion
    }
}