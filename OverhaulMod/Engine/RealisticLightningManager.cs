using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class RealisticLightningManager : Singleton<RealisticLightningManager>
    {
        public const string LIGHTNING_INFO_LIST_FILE = "lightningInfo.json";

        public static readonly string LightSettingsObjectResourcePath = "Prefabs/LevelObjects/Settings/LevelLightSettings";

        public static readonly string LightSettingsOverrideObjectResourcePath = "Prefabs/LevelObjects/Lights/LightSettingsOverride";

        private static Material[] s_skyboxes;

        private RealisticLightningInfoList m_lightningInfoList;

        private void Start()
        {
            DelegateScheduler.Instance.Schedule(loadSkyboxesAddon, 3f);
        }

        private void loadSkyboxesAddon()
        {
            if (ErrorManager.Instance.HasCrashed() || !ContentManager.Instance.HasContent(ContentManager.REALISTIC_SKYBOXES_CONTENT_FOLDER_NAME))
                return;

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

            LevelEditorLightManager.Instance.RefreshLightInScene();
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
                    levelDescription = GameFlowManager.Instance.HasWonRound() ? LevelManager.Instance.GetLastSpawnedLevelDescription() : LevelManager.Instance.GetCurrentLevelDescription();
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

        public void SetSkybox(int index)
        {
            if (index < 0 || s_skyboxes.IsNullOrEmpty())
                return;

            Material material = s_skyboxes[index];
            SkyBoxManager.Instance._currentSkybox = material;
            RenderSettings.skybox = material;
        }

        public RealisticLightningInfo GetCurrentRealisticLightningInfo()
        {
            string prefabName = GetLevelPrefabName(null);
            if (prefabName.IsNullOrEmpty())
                return null;

            RealisticLightningInfoList realisticLightningInfoList = m_lightningInfoList;
            if (realisticLightningInfoList == null || realisticLightningInfoList.LightningInfos.IsNullOrEmpty())
                return null;

            return realisticLightningInfoList.GetLightningInfo(prefabName);
        }

        public void PatchLevelLightSettings(LevelLightSettings lightSettings = null, bool refreshLight = false)
        {
            if (!lightSettings)
                lightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();

            RealisticLightSettings realisticLightSettings = lightSettings.GetComponent<RealisticLightSettings>();
            if (!realisticLightSettings || !realisticLightSettings.EnableRealisticSkybox)
                return;

            RealisticLightningInfo realisticLightningInfo = GetCurrentRealisticLightningInfo();
            if (realisticLightningInfo != null)
            {
                if (realisticLightningInfo.Lightning != null)
                    realisticLightningInfo.Lightning.ApplyValues(lightSettings);

                realisticLightSettings.RealisticSkyBoxIndex = realisticLightningInfo.SkyboxIndex;

                if (refreshLight)
                    LevelEditorLightManager.Instance.RefreshLightInScene();
            }
            else
            {
                realisticLightSettings.RealisticSkyBoxIndex = -1;
            }
            //realisticLightSettings.HasSetSkyBoxIndex = true;
        }
    }
}
