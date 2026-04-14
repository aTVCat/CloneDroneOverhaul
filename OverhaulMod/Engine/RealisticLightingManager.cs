using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class RealisticLightingManager : Singleton<RealisticLightingManager>
    {
        public const string OLD_LIGHTING_INFO_LIST_FILE = "lightningInfo.json";

        public const string LIGHTING_INFO_LIST_FILE = "lightingInfo.json";

        public const string OVERHAUL_ASSETS_SKYBOXES = "overhaulassets_skyboxes";

        public static readonly string LightSettingsObjectResourcePath = "Prefabs/LevelObjects/Settings/LevelLightSettings";

        public static readonly string LightSettingsOverrideObjectResourcePath = "Prefabs/LevelObjects/Lights/LightSettingsOverride";

        private RealisticLightingInfoList _lightingInfoList;

        private Dictionary<string, bool?> _loadingSkyboxes;

        private void Start()
        {
            DelegateScheduler.Instance.Schedule(loadSkyboxesAddon, 3f);
        }

        private void loadSkyboxesAddon()
        {
            string path = null;
            bool hasAddon = AddonManager.Instance.HasInstalledAddon(AddonManager.REALISTIC_SKYBOXES_ADDON_ID, out path);

            if (!hasAddon || ErrorManager.Instance.HasCrashed())
                return;

            string[] files = Directory.GetFiles(path, "overhaul_rs*");
            if (files.IsNullOrEmpty())
                return;

            AddonManager.Instance.SetAddonIsLoading(this, true);
            _loadingSkyboxes = new Dictionary<string, bool?>();
            foreach (string file in files)
                _loadingSkyboxes.Add(Path.GetFileName(file), false);

            foreach (string file in files)
            {
                string fn = Path.GetFileName(file);
                ModResources.LoadBundleAsync(fn, delegate (bool result)
                {
                    if (!result)
                    {
                        _loadingSkyboxes[fn] = null;
                    }
                    else
                    {
                        _loadingSkyboxes[fn] = true;
                    }

                }, path);
            }

            _ = ModActionUtils.RunCoroutine(processSkyboxesCoroutine());
        }

        private IEnumerator processSkyboxesCoroutine()
        {
            while (_loadingSkyboxes.ContainsValue(false))
                yield return null;

            string addonPath = AddonManager.Instance.GetAddonPath(AddonManager.REALISTIC_SKYBOXES_ADDON_ID);
            RealisticLightingInfoList realisticLightingInfoList = null;
            try
            {
                string path = Path.Combine(addonPath, LIGHTING_INFO_LIST_FILE);
                if (!File.Exists(path))
                {
                    string oldPath = Path.Combine(addonPath, OLD_LIGHTING_INFO_LIST_FILE);
                    if (File.Exists(oldPath))
                    {
                        ModFileUtils.WriteText(ModFileUtils.ReadText(oldPath).Replace("Lightning", "Lighting"), oldPath);
                        File.Move(oldPath, path);
                    }
                    else
                    {
                        realisticLightingInfoList = new RealisticLightingInfoList();
                    }
                }
                realisticLightingInfoList = ModJsonUtils.DeserializeStream<RealisticLightingInfoList>(path);
                realisticLightingInfoList.FixValues();
            }
            catch (Exception)
            {
                realisticLightingInfoList = new RealisticLightingInfoList();
                realisticLightingInfoList.FixValues();
            }
            _lightingInfoList = realisticLightingInfoList;

            Dictionary<string, bool?> dictionary = new Dictionary<string, bool?>(_loadingSkyboxes);
            _loadingSkyboxes.Clear();
            foreach (KeyValuePair<string, bool?> kv in dictionary)
            {
                if (kv.Value.HasValue && kv.Value.Value)
                {
                    _loadingSkyboxes.Add(kv.Key, false);
                    ModActionUtils.RunCoroutine(processBundle(ModResources.AssetBundle(kv.Key, addonPath), kv.Key));
                }
            }

            while (_loadingSkyboxes.ContainsValue(false))
                yield return null;

            _loadingSkyboxes.Clear();
            _loadingSkyboxes = null;

            LevelEditorLightManager.Instance.RefreshLightInScene();
            AddonManager.Instance.SetAddonIsLoading(this, false);
            yield break;
        }

        private IEnumerator processBundle(AssetBundle assetBundle, string key)
        {
            AssetBundleRequest r = assetBundle.LoadAllAssetsAsync<Material>();
            yield return r;
            if (!r.allAssets.IsNullOrEmpty() && r.allAssets[0] is Material material)
            {
                AdditionalSkyboxesManager.Instance.AddSkybox(key, material.name, material);
                _loadingSkyboxes[key] = true;
            }
            else
            {
                _loadingSkyboxes[key] = null;
            }
            yield break;
        }

        public void SaveLightingInfo()
        {
            RealisticLightingInfoList realisticLightingInfoList = _lightingInfoList;
            if (realisticLightingInfoList == null || realisticLightingInfoList.LightingInfos.IsNullOrEmpty())
                return;

            ModJsonUtils.WriteStream(Path.Combine(AddonManager.Instance.GetAddonPath(AddonManager.REALISTIC_SKYBOXES_ADDON_ID), LIGHTING_INFO_LIST_FILE), realisticLightingInfoList);
        }

        public void SaveCurrentLighting()
        {
            RealisticLightingInfoList realisticLightingInfoList = _lightingInfoList;
            if (realisticLightingInfoList == null)
            {
                ModUIUtils.MessagePopupOK("Could not save current level lighting info", "info list is missing");
                return;
            }

            string prefabName = GetLevelPrefabName(null);
            if (prefabName.IsNullOrEmpty())
            {
                ModUIUtils.MessagePopupOK("Could not save current level lighting info", "prefab name is missing");
                return;
            }

            RealisticLightingInfo realisticLightingInfo = realisticLightingInfoList.GetLightingInfo(prefabName);
            if (realisticLightingInfo == null)
            {
                realisticLightingInfo = new RealisticLightingInfo();
                realisticLightingInfoList.LightingInfos.Add(realisticLightingInfo);
            }
            realisticLightingInfo.FixValues();

            realisticLightingInfo.Lighting.SetValuesFromEnvironment();
            realisticLightingInfo.LevelPrefabName = prefabName;
            SaveLightingInfo();
        }

        public string GetLevelPrefabName(LevelDescription levelDescription)
        {
            if (levelDescription == null)
            {
                try
                {
                    levelDescription = GameFlowManager.Instance.HasWonRound() ? LevelManager.Instance.GetLastSpawnedLevelDescription() : LevelManager.Instance.GetCurrentLevelDescription();
                }
                catch (Exception)
                {
                    return null;
                }
            }

            if (levelDescription == null)
                return null;

            string name = levelDescription.PrefabName;
            return name.IsNullOrEmpty() ? null : name.Substring(name.LastIndexOf("/") + 1);
        }

        public RealisticLightingInfo GetCurrentRealisticLightingInfo()
        {
            string prefabName = GetLevelPrefabName(null);
            if (prefabName.IsNullOrEmpty())
                return null;

            RealisticLightingInfoList realisticLightingInfoList = _lightingInfoList;
            if (realisticLightingInfoList == null || realisticLightingInfoList.LightingInfos.IsNullOrEmpty())
                return null;

            return realisticLightingInfoList.GetLightingInfo(prefabName);
        }
    }
}
