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
    }
}
