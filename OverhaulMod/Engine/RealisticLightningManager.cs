﻿using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class RealisticLightningManager : Singleton<RealisticLightningManager>
    {
        public const string LIGHTNING_INFO_LIST_FILE = "lightningInfo.json";

        private static Material[] s_skyboxes;

        private RealisticLightningInfoList m_lightningInfoList;

        private void Start()
        {
            if (ContentManager.Instance.HasContent(ContentManager.REALISTIC_SKYBOXES_CONTENT_FOLDER_NAME))
            {
                ContentManager.Instance.SetContentIsLoading(this, true);
                ModResources.Instance.LoadBundleAsync("overhaulassets_skyboxes", delegate (AssetBundle bundle)
                {
                    _ = ModActionUtils.RunCoroutine(loadAllSkyboxesCoroutine(bundle));
                }, null, $"{ModCore.addonsFolder}{ContentManager.REALISTIC_SKYBOXES_CONTENT_FOLDER_NAME}/");

                RealisticLightningInfoList realisticLightningInfoList = null;
                try
                {
                    realisticLightningInfoList = ModJsonUtils.DeserializeStream<RealisticLightningInfoList>($"{ModCore.addonsFolder}{ContentManager.REALISTIC_SKYBOXES_CONTENT_FOLDER_NAME}/{LIGHTNING_INFO_LIST_FILE}");
                    realisticLightningInfoList.FixValues();
                }
                catch
                {
                    realisticLightningInfoList = new RealisticLightningInfoList();
                    realisticLightningInfoList.FixValues();
                }
                m_lightningInfoList = realisticLightningInfoList;
            }
        }

        private IEnumerator loadAllSkyboxesCoroutine(AssetBundle bundle)
        {
            if (ModAdvancedCache.TryGet("RealisticSkyboxes", out Material[] array))
                s_skyboxes = array;
            else
            {
                AssetBundleRequest r = bundle.LoadAllAssetsAsync<Material>();
                yield return r;
                if (!r.allAssets.IsNullOrEmpty())
                {
                    s_skyboxes = new Material[r.allAssets.Length];
                    int i = 0;
                    foreach (Object obj in r.allAssets)
                    {
                        s_skyboxes[i] = r.allAssets[i] as Material;
                        i++;
                    }
                }
                ModAdvancedCache.Add("RealisticSkyboxes", s_skyboxes);
            }

            PatchLightning(true);
            ContentManager.Instance.SetContentIsLoading(this, false);
            yield break;
        }

        public void SaveLightningInfo()
        {
            RealisticLightningInfoList realisticLightningInfoList = m_lightningInfoList;
            if (realisticLightningInfoList == null || realisticLightningInfoList.LightningInfos.IsNullOrEmpty())
                return;

            ModJsonUtils.WriteStream($"{ModCore.addonsFolder}{ContentManager.REALISTIC_SKYBOXES_CONTENT_FOLDER_NAME}/{LIGHTNING_INFO_LIST_FILE}", realisticLightningInfoList);
        }

        public void SaveCurrentLightningInfo(int skyboxIndex)
        {
            RealisticLightningInfoList realisticLightningInfoList = m_lightningInfoList;
            if (realisticLightningInfoList == null)
            {
                ModUIUtils.MessagePopupOK("Could not save current level lightning info", "info list is missing");
                return;
            }

            string prefabName = GetLevelPrefabName(null);
            if (prefabName.IsNullOrEmpty())
            {
                ModUIUtils.MessagePopupOK("Could not save current level lightning info", "prefab name is missing");
                return;
            }

            RealisticLightningInfo realisticLightningInfo = realisticLightningInfoList.GetLightningInfo(prefabName);
            if (realisticLightningInfo == null)
            {
                realisticLightningInfo = new RealisticLightningInfo();
                realisticLightningInfoList.LightningInfos.Add(realisticLightningInfo);
            }
            realisticLightningInfo.FixValues();

            realisticLightningInfo.Lightning.SetValuesUsingEnvironmentSettings();
            realisticLightningInfo.LevelPrefabName = prefabName;
            realisticLightningInfo.SkyboxIndex = skyboxIndex;
            SaveLightningInfo();
        }

        public string GetLevelPrefabName(LevelDescription levelDescription)
        {
            if (levelDescription == null)
            {
                try
                {
                    levelDescription = LevelManager.Instance?.GetCurrentLevelDescription();
                }
                catch
                {
                    return null;
                }
            }

            if (levelDescription == null)
                return null;

            string name = levelDescription.PrefabName;
            return name.IsNullOrEmpty() ? null : name.Substring(name.LastIndexOf("/") + 1);
        }

        public void SetSkybox(int index, bool refreshSkyBox = true)
        {
            if (index < 0 || s_skyboxes.IsNullOrEmpty())
            {
                if (!refreshSkyBox)
                    return;

                LevelLightSettings lightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
                if (lightSettings)
                {
                    SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(lightSettings);
                }
                return;
            }

            Material material = s_skyboxes[index];
            SkyBoxManager.Instance._currentSkybox = material;
            RenderSettings.skybox = material;
        }

        public void PatchLightning(bool refresh, LevelLightSettings lightSettings = null, bool refreshRLightning = true)
        {
            AdvancedPhotoModeManager.useRealisticSkyBoxes = false;
            AdvancedPhotoModeManager.realisticSkyBoxIndex = -1;
            ModLevelManager.Instance.currentRealisticSkyBoxIndex = -1;

            if (!lightSettings)
                lightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();

            RealisticLightningInfoList realisticLightningInfoList = m_lightningInfoList;
            if (realisticLightningInfoList != null && !realisticLightningInfoList.LightningInfos.IsNullOrEmpty())
            {
                string prefabName = GetLevelPrefabName(null);
                if (prefabName != null)
                {
                    RealisticLightningInfo realisticLightningInfo = null;
                    foreach (RealisticLightningInfo l in realisticLightningInfoList.LightningInfos)
                        if (l.LevelPrefabName == prefabName)
                        {
                            realisticLightningInfo = l;
                            break;
                        }

                    if (realisticLightningInfo != null)
                    {
                        realisticLightningInfo.Lightning.ApplyValues(lightSettings);
                        if (refreshRLightning)
                        {
                            SetSkybox(realisticLightningInfo.SkyboxIndex);
                            AdvancedPhotoModeManager.realisticSkyBoxIndex = realisticLightningInfo.SkyboxIndex;
                            AdvancedPhotoModeManager.useRealisticSkyBoxes = true;
                            ModLevelManager.Instance.currentRealisticSkyBoxIndex = realisticLightningInfo.SkyboxIndex;
                        }
                    }
                }
            }

            if (lightSettings && refresh)
                LevelEditorLightManager.Instance.RefreshLightInScene();
        }
    }
}
