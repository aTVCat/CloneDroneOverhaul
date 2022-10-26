using System;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CloneDroneOverhaul
{
    public static class OverhaulCacheManager
    {
        private static bool _hasCached;

        private static Dictionary<string, object> _cachedStuff;

        static Dictionary<string, Transform> _cachedLevelEditorAssets = new Dictionary<string, Transform>();
        static List<ComponentCache> _cachedModdedObjects = new List<ComponentCache>();

        public const string OverhaulMainAssetBundle = "cdo_rw_stuff";

        public static void CacheStuff()
        {
            if (_hasCached)
            {
                return;
            }

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

            _hasCached = true;
        }

        public static T GetCached<T>(string id) where T : class
        {
            if (_cachedStuff.ContainsKey(id))
            {
                object rawObj = _cachedStuff[id];
                if(!(rawObj is T))
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
        public static T GetCachedComponent<T>(this Transform transform, int index) where T : Component
        {
            throw new NotImplementedException();
            bool requireCache = false;
            T comp = null;
            foreach (ComponentCache cached in _cachedModdedObjects)
            {

            }
            if (requireCache)
            {
                comp = transform.GetComponent<T>();
                if (comp != null)
                {
                    _cachedModdedObjects.Add(new ComponentCache() { Component = (T)comp, Index = index, Transform = transform });
                }
            }
            return comp;
        }
        private static void getCached(Transform transform, int index)
        {

        }

        public static Transform GetAndCacheLevelEditorObject(string path)
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

        private class ComponentCache
        {
            public int Index;
            public Component Component;
            public Transform Transform;
        }
    }
}
