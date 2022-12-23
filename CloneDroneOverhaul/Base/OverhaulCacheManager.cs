using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul
{
    public class OverhaulCacheManager
    {
        private static bool _hasCached;

        private static Dictionary<string, object> _cachedStuff;
        private static Dictionary<string, object> _temporalStuff;
        private static Dictionary<string, Transform> _cachedLevelEditorAssets = new Dictionary<string, Transform>();

        public const string OverhaulMainAssetBundle = "cdo_rw_stuff";
        public const string OverhaulAssetBundleP2 = "overhaulstuff_p2";

        internal static void ClearTemporal()
        {
            if (_temporalStuff != null)
            {
                _temporalStuff.Clear();
            }
        }
        public static void CacheStuff()
        {
            if (_hasCached)
            {
                return;
            }

            _temporalStuff = new Dictionary<string, object>();

            _cachedStuff = new Dictionary<string, object>
            {
                { "LBSInviteScreenBG_1", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_1") },
                { "LBSInviteScreenBG_2", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_2") },
                { "LBSInviteScreenBG_3", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_3") },
                { "LBSInviteScreenBG_4", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_4") },
                { "LevelEditor_Grid", AssetLoader.GetObjectFromFile<GameObject>(OverhaulMainAssetBundle, "LevelEditorGrid") },

                { "KillMethodType_" + DamageSourceType.Sword.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_CommonSword256x256") },
                { "KillMethodType_" + "Fire" + DamageSourceType.Sword.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireSword") },
                { "KillMethodType_" + DamageSourceType.Arrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_CommonArrow256x256") },
                { "KillMethodType_" + "Fire" + DamageSourceType.Arrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireArrow") },
                { "KillMethodType_" + DamageSourceType.Hammer.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Hammer") },
                { "KillMethodType_" + "Fire" + DamageSourceType.Hammer.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireHammer") },
                { "KillMethodType_" + DamageSourceType.Spear.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_CommonSpear256x256") },
                { "KillMethodType_" + "Fire" + DamageSourceType.Spear.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireSpear") },
                { "KillMethodType_" + DamageSourceType.FlameBreath.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FlameBruh") },
                { "KillMethodType_" + DamageSourceType.AutoLaser.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Laser") },
                { "KillMethodType_" + DamageSourceType.SpikeTrap.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_SpikeTrap") },
                { "KillMethodType_" + DamageSourceType.SawBlade.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Sawblade") },
                { "KillMethodType_" + DamageSourceType.DeflectedArrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_DeflectedArrow256x256") },
                { "KillMethodType_" + DamageSourceType.SpawnCampDeflectedArrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_DeflectedArrow256x256") },
                { "KillMethodType_" + DamageSourceType.Laser.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Laser") },
                { "KillMethodType_" + DamageSourceType.EnergyBeam.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_BeamV216x16") },
                { "KillMethodType_" + DamageSourceType.Lava.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Lava") },
                { "KillMethodType_" + DamageSourceType.FireTrap.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireTrap") },
                { "KillMethodType_" + DamageSourceType.EnvironmentFire.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireTrap") },
                { "KillMethodType_" + DamageSourceType.SpeedHackBanFire.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Banned") },

                { "Shader_BleedingColors", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "BleedingColors") },
                { "Shader_BWDiffuse", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "BWDiffuse") },
                { "Shader_Distortion", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "Distortion") },
                { "Shader_Scanlines", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "Scanlines") },
                { "Shader_Tint", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "Tint") },
                { "Shader_VUnsync", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "VUnsync") },

                { "SceneTransition_LevelEditorStyle", AssetLoader.GetObjectFromFile<GameObject>(OverhaulMainAssetBundle, "SceneTransitionScreen") },
                { "placeholderLoadSprite", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "placeholderLoad") },

                { "SkyboxMaterial_Stars", AssetLoader.GetObjectFromFile<Material>("cdo_rw_stuff", "StarsSkyboxV2") },
                { "SkyboxMaterial_Stars2", AssetLoader.GetObjectFromFile<Material>("cdo_rw_stuff", "StarSkyBox") },
                { "SkyboxMaterial_StarsChapter4", AssetLoader.GetObjectFromFile<Material>("cdo_rw_stuff", "Chapter4NewSkybox") },

                { "objects_normalMap_dark_tile_2_normalMap", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_tile_2_normalMap") },
                { "MatHeightMap_Tile_BattleCruiser_Dark_Tile_2", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_tile_2_heightMapV4") },
                { "MatHeightMap_Tile_BattleCruiser_Dark_Floor_Detail_1", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_floor_detail_1_heightMap") },
                { "MatHeightMap_HumanFleetShips", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "HumanFleetShips_heightMap") },
                { "MatHeightMap_DarkHallwayParts", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "DarkHallwayParts_heightMap") },
                { "MatHeightMap_Tile_BattleCruiser_Dark_Tile_4", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_tile_4_heightMap") },
                { "MatHeightMap_Tile_BattleCruiser_Dark_Floor_1", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_floor_1_heightMap") },
                { "MatHeightMap_Tile_BattleCruiser_SmallDark_Tile_1", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "TILES_SMALLER_01_heightMap") },
                { "MatHeightMap_Tile_BattleCruiser_Dark_Floor_Detail_2", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_floor_detail_2_heightMap") },
                { "MatHeightMap_RampBasic", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "Ramp_texture_v2_heightMap") },

                { "UI_TransitionScreen", AssetLoader.GetObjectFromFile<GameObject>(OverhaulAssetBundleP2, "SceneTranstionUI") },

                { "Dark_Pillar1_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Dark_Pillar1_LOD0") },
                { "Dark_Pillar1_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Dark_Pillar1_LOD1") },
                { "Dark_Pillar1_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Dark_Pillar1_LOD2") },
                { "Dark_Wall1_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Dark_Wall1_LOD0") },
                { "Dark_Wall1_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Dark_Wall1_LOD1") },
                { "Dark_Wall1_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Dark_Wall1_LOD1") }, //2
                { "Terminal6_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal6_LOD0") },
                { "Terminal6_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal6_LOD1") },
                { "Terminal6_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal6_LOD2") },
                { "Terminal1_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal1_LOD0") },
                { "Terminal1_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal1_LOD1") },
                { "Terminal1_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal1_LOD2") },
                { "Terminal2_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal2_LOD0") },
                { "Terminal2_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal2_LOD1") },
                { "Terminal2_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal2_LOD2") },
                { "Terminal3_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal3_LOD0") },
                { "Terminal3_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal3_LOD1") },
                { "Terminal3_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal3_LOD2") },
                { "Terminal4_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal4_LOD0") },
                { "Terminal4_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal4_LOD1") },
                { "Terminal4_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal4_LOD2") },
                { "Terminal5_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal5_LOD0") },
                { "Terminal5_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal5_LOD1") },
                { "Terminal5_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Terminal5_LOD2") },
                { "TerminalGroup_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "TerminalGroup_LOD0") },
                { "TerminalGroup_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "TerminalGroup_LOD1") },
                { "TerminalGroup_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "TerminalGroup_LOD2") },
                { "TerminalScreenBig_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "TerminalScreenBig_LOD0") },
                { "TerminalScreenBig_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "TerminalScreenBig_LOD1") },
                { "TerminalScreenBig_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "TerminalScreenBig_LOD2") },
                { "HumanEnvoyShip_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "HumanEnvoyShip_LOD0") },
                { "HumanEnvoyShip_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "HumanEnvoyShip_LOD1") },
                { "HumanEnvoyShip_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "HumanEnvoyShip_LOD2") },
                { "ibeam_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "iBeam_LOD0") },
                { "ibeam_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "iBeam_LOD1") },
                { "ibeam_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "iBeam_LOD2") },
                { "Spidertron6000_TransportTube_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Spidertron6000_LOD0") },
                { "Spidertron6000_TransportTube_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Spidertron6000_LOD1") },
                { "Spidertron6000_TransportTube_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "Spidertron6000_LOD2") },
                { "WallSegmentWithBumpers_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "WallSegmentWithBumpers_LOD0") },
                { "WallSegmentWithBumpers_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "WallSegmentWithBumpers_LOD1") },
                { "WallSegmentWithBumpers_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "WallSegmentWithBumpers_LOD2") },
                { "ThinWhitePillar_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "ThinWhitePillar_LOD0") },
                { "ThinWhitePillar_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "ThinWhitePillar_LOD1") },
                { "ThinWhitePillar_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "ThinWhitePillar_LOD2") },
                { "White_Pillar_LOD0", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "White_Pillar_LOD0") },
                { "White_Pillar_LOD1", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "White_Pillar_LOD1") },
                { "White_Pillar_LOD2", AssetLoader.GetObjectFromFile<GameObject>("overhaul_lods", "White_Pillar_LOD2") },
            };

            _hasCached = true;
        }

        /// <summary>
        /// Save an object in memory until game closes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="name"></param>
        public static void CacheObject<T>(in T @object, in string name)
        {
            if (!HasCached(name))
            {
                _cachedStuff.Add(name, @object);
            }
        }

        /// <summary>
        /// Get saved object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetCached<T>(in string id) where T : class
        {
            if (_cachedStuff.ContainsKey(id))
            {
                object rawObj = _cachedStuff[id];
                if (!(rawObj is T))
                {
                    Modules.ModuleManagement.ShowError("Object with ID: " + id + " is not " + typeof(T).ToString());
                }
                T obj = (T)_cachedStuff[id];
                return obj;
            }
            else
            {
                Modules.ModuleManagement.ShowError("Cannot find cached asset with ID: " + id);
            }
            return null;
        }

        /// <summary>
        /// Check if we saved an object with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool HasCached(in string id)
        {
            return _cachedStuff.ContainsKey(id);
        }

        /// <summary>
        /// Save an object in memory until scene restarts/switches
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public static void AddTemporalObject<T>(in T obj, in string name)
        {
            if (!_temporalStuff.ContainsKey(name))
            {
                _temporalStuff.Add(name, obj);
            }
        }

        /// <summary>
        /// Get temporary saved object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetTemporalObject<T>(in string name)
        {
            T result = (T)_temporalStuff[name];
            return (T)result;
        }

        /// <summary>
        /// Dispose temporary saved object
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveTemporalObject(in string name)
        {
            if (_temporalStuff.ContainsKey(name))
            {
                _temporalStuff.Remove(name);
            }
        }

        /// <summary>
        /// Check if we have added a temporal object with specified id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ContainsTemporalObject(in string name)
        {
            return _temporalStuff.ContainsKey(name);
        }

        /// <summary>
        /// Get modded level editor related asset
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Transform GetModdedLevelEditorResource(in string path)
        {
            Transform trans = null;
            if (_cachedLevelEditorAssets.ContainsKey(path))
            {
                trans = _cachedLevelEditorAssets[path];
            }
            else
            {
                trans = Resources.Load<Transform>(path);
                _cachedLevelEditorAssets.Add(path, trans);
            }
            return trans;
        }
    }
}
