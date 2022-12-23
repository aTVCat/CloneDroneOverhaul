using UnityEngine;
using System.Collections.Generic;
using CloneDroneOverhaul.V3Tests.Base;
using CloneDroneOverhaul;
using CloneDroneOverhaul.Modules;
using ModLibrary;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class OverhaulGraphicsController : V3_ModControllerBase
    {
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
        public const string ID_VFX_HAMMER_HIT = "VFX_Cut";
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
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBurn").transform, 10, ID_VFX_BURNING);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ExplosionCubes").transform, 5, ID_VFX_EXPLOSION_VOXELS);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_ExplosionNew").transform, 5, ID_VFX_EXPLOSION);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FloatingLava").transform, 10, ID_VFX_FLOATING_MAGMA);
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
            bool isMindspace = GameStatisticsController.GameStatistics.PlayerRobotInformation.IsFPMMindspace;

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
            if(light != null) light.SetLightSettings(range, hexColor.hexToColor(), speed);
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
            if (Random.Range(0, 10) > 5)
            {
                UseVFX(ID_VFX_BURNING, position, Vector3.zero);
            }
        }

        public static void Simulate_Explosion(in Vector3 position)
        {
            UseVFX(ID_VFX_EXPLOSION_VOXELS, position, Vector3.zero);
            Simulate_Smoke(position);
            Simulate_Light_Hex(position, 100, "FFB59C", 03f);
        }

        #endregion

        #region Effects

        static ReflectionProbe _reflectionProbe;

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

        #region Non-static Stuff

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if(settingName == "Graphics.Additions.Shading")
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
        }

        #endregion
    }
}