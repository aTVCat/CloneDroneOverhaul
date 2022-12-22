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

        static Dictionary<string, Transform> _cachedLevelEditorAssets = new Dictionary<string, Transform>();

        public const string OverhaulMainAssetBundle = "cdo_rw_stuff";
        public const string OverhaulAssetBundleP2 = "overhaulstuff_p2";

        internal static void ClearTemporal()
        {
            if (_temporalStuff != null)
                _temporalStuff.Clear();
        }
        public static void CacheStuff()
        {
            if (_hasCached)
            {
                return;
            }

            _temporalStuff = new Dictionary<string, object>();

            _cachedStuff = new Dictionary<string, object>();
            _cachedStuff.Add("LBSInviteScreenBG_1", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_1"));
            _cachedStuff.Add("LBSInviteScreenBG_2", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_2"));
            _cachedStuff.Add("LBSInviteScreenBG_3", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_3"));
            _cachedStuff.Add("LBSInviteScreenBG_4", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "LBSInviteBG_4"));
            _cachedStuff.Add("LevelEditor_Grid", AssetLoader.GetObjectFromFile<GameObject>(OverhaulMainAssetBundle, "LevelEditorGrid"));

            _cachedStuff.Add("KillMethodType_" + DamageSourceType.Sword.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_CommonSword256x256"));
            _cachedStuff.Add("KillMethodType_" + "Fire" + DamageSourceType.Sword.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireSword"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.Arrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_CommonArrow256x256"));
            _cachedStuff.Add("KillMethodType_" + "Fire" + DamageSourceType.Arrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireArrow"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.Hammer.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Hammer"));
            _cachedStuff.Add("KillMethodType_" + "Fire" + DamageSourceType.Hammer.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireHammer"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.Spear.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_CommonSpear256x256"));
            _cachedStuff.Add("KillMethodType_" + "Fire" + DamageSourceType.Spear.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireSpear"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.FlameBreath.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FlameBruh"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.AutoLaser.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Laser"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.SpikeTrap.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_SpikeTrap"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.SawBlade.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Sawblade"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.DeflectedArrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_DeflectedArrow256x256"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.SpawnCampDeflectedArrow.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_DeflectedArrow256x256"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.Laser.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Laser"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.EnergyBeam.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_BeamV216x16"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.Lava.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Lava"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.FireTrap.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireTrap"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.EnvironmentFire.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_FireTrap"));
            _cachedStuff.Add("KillMethodType_" + DamageSourceType.SpeedHackBanFire.ToString(), AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "Kill_Banned"));

            _cachedStuff.Add("Shader_BleedingColors", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "BleedingColors"));
            _cachedStuff.Add("Shader_BWDiffuse", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "BWDiffuse"));
            _cachedStuff.Add("Shader_Distortion", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "Distortion"));
            _cachedStuff.Add("Shader_Scanlines", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "Scanlines"));
            _cachedStuff.Add("Shader_Tint", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "Tint"));
            _cachedStuff.Add("Shader_VUnsync", AssetLoader.GetObjectFromFile<Shader>("effect_glitches", "VUnsync"));

            _cachedStuff.Add("SceneTransition_LevelEditorStyle", AssetLoader.GetObjectFromFile<GameObject>(OverhaulMainAssetBundle, "SceneTransitionScreen"));
            _cachedStuff.Add("placeholderLoadSprite", AssetLoader.GetObjectFromFile<Sprite>(OverhaulMainAssetBundle, "placeholderLoad"));

            _cachedStuff.Add("SkyboxMaterial_Stars", AssetLoader.GetObjectFromFile<Material>("cdo_rw_stuff", "StarsSkyboxV2"));
            _cachedStuff.Add("SkyboxMaterial_Stars2", AssetLoader.GetObjectFromFile<Material>("cdo_rw_stuff", "StarSkyBox"));
            _cachedStuff.Add("SkyboxMaterial_StarsChapter4", AssetLoader.GetObjectFromFile<Material>("cdo_rw_stuff", "Chapter4NewSkybox"));

            _cachedStuff.Add("objects_normalMap_dark_tile_2_normalMap", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_tile_2_normalMap"));
            _cachedStuff.Add("MatHeightMap_Tile_BattleCruiser_Dark_Tile_2", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_tile_2_heightMapV4"));
            _cachedStuff.Add("MatHeightMap_Tile_BattleCruiser_Dark_Floor_Detail_1", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_floor_detail_1_heightMap"));
            _cachedStuff.Add("MatHeightMap_HumanFleetShips", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "HumanFleetShips_heightMap"));
            _cachedStuff.Add("MatHeightMap_DarkHallwayParts", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "DarkHallwayParts_heightMap"));
            _cachedStuff.Add("MatHeightMap_Tile_BattleCruiser_Dark_Tile_4", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_tile_4_heightMap"));
            _cachedStuff.Add("MatHeightMap_Tile_BattleCruiser_Dark_Floor_1", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_floor_1_heightMap"));
            _cachedStuff.Add("MatHeightMap_Tile_BattleCruiser_SmallDark_Tile_1", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "TILES_SMALLER_01_heightMap"));
            _cachedStuff.Add("MatHeightMap_Tile_BattleCruiser_Dark_Floor_Detail_2", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "dark_floor_detail_2_heightMap"));
            _cachedStuff.Add("MatHeightMap_RampBasic", AssetLoader.GetObjectFromFile<Texture>("cdo_rw_stuff", "Ramp_texture_v2_heightMap"));

            _cachedStuff.Add("UI_TransitionScreen", AssetLoader.GetObjectFromFile<GameObject>(OverhaulAssetBundleP2, "SceneTranstionUI"));

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
