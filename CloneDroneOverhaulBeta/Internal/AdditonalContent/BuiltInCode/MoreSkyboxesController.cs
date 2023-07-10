using CDOverhaul.Gameplay.QualityOfLife;
using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.BuiltIn.AdditionalContent
{
    public class MoreSkyboxesController : OverhaulController
    {
        public const string RequiredContent = "more-skyboxes-builtin-content";

        public const string SkyboxesAssetBundle = "Content/RealisticSkyboxes/overhaulassets_skyboxes";

        public static readonly Dictionary<string, int> SkyboxOverrides = new Dictionary<string, int>()
        {
            // LBS
            { "BR_AlienIsland", 10 },
            { "BR_BlueRock", 0 },
            { "BR_Cliffside", 6 },
            { "BR_CrashedRoom", 9 },
            { "BR_Deathcubes", 1 },
            { "BR_Facility", 15 },
            { "BR_GreenHills", 0 },
            { "BR_Maze", 11 },
            { "BR_House", 6 },
            { "BR_LavaCenter", 10 },
            { "BR_MovingFloor", 13 },
            { "BR_Beach", 9 },
            { "BR_Compactor", 2 },
            { "BR_FireIsland", 4 },
            { "BR_Fleet", -1 },
            { "BR_RisingTide", 15 },
            { "BR_PyramidsInSpace", 7 },

            // Story
            { "Story1", 6 },
            { "Story2", 13 },
            { "Story3", 9 },
            { "Story4", 13 },
            { "Story5", 9 },
            { "Story6", 5 },
            { "Story7", 14 },
            { "Story8", 6 },
            { "Story9", 3 },
            { "Story10", 13 },

            { "Story11", 12 },
            { "Story12", 0 },
            { "Story13", 5 },
            { "Story14", 1 },
            { "Story15", 6 },
            { "Story16", 10 },
            { "Story17", 13 },
            { "Story18", 11 },
            { "Story19", 1 },
            { "Story20", 13 },
        };

        public static readonly Dictionary<string, Hashtable> LightSettingOverrides = new Dictionary<string, Hashtable>()
        {
            // LBS
            { "BR_PyramidsInSpace", new Hashtable() { { "FogColor", Color.white * 0.15f }, { "DLColor", Color.white * 0.15f }, { "AmbientColor", Color.clear } } },

            // Story
            { "Story1", new Hashtable() { { "DLIntensity", 0.5f }, { "FogColor", new Color(0.03f, 0.08f, 0.16f, 1f) } } },
            { "Story2", new Hashtable() { { "FogColor", new Color(0.43f, 0.41f, 0.48f, 1f) } } },
            { "Story3", new Hashtable() { { "FogColor", new Color(0.13f, 0.31f, 0.5f, 1f) } } },
            { "Story4", new Hashtable() { { "FogColor", new Color(0.34f, 0.33f, 0.35f, 1f) } } },
            { "Story5", new Hashtable() { { "FogColor", new Color(0.07f, 0.28f, 0.5f, 1f) } } },
            { "Story6", new Hashtable() { { "FogColor", new Color(0.28f, 0.35f, 0.42f, 1f) } } },
            { "Story7", new Hashtable() { { "FogColor", new Color(0.26f, 0.44f, 0.61f, 1f) } } },
            { "Story8", new Hashtable() { { "FogColor", new Color(0.11f, 0.17f, 0.28f, 1f) } } },
            { "Story9", new Hashtable() { { "FogColor", new Color(0.11f, 0.18f, 0.33f, 1f) }, { "DLColor", new Color(0.58f, 0.62f, 0.62f, 1f) } } },
            { "Story10", new Hashtable() { { "FogColor", new Color(0.43f, 0.45f, 0.53f, 1f) }, { "DLColor", new Color(1f, 0.58f, 0.45f, 1f) }, { "FogEnd", 2500f } } },

            { "Story11", new Hashtable() { { "FogColor", new Color(0.3f, 0.44f, 0.63f, 1f) } } },
            { "Story12", new Hashtable() { { "FogColor", new Color(0.22f, 0.31f, 0.53f, 1f) }, { "DLColor", new Color(0.9f, 0.83f, 0.78f, 1f) } } },
            { "Story13", new Hashtable() { { "FogColor", new Color(0.35f, 0.5f, 0.7f, 1f) }, { "FogEnd", 3000f } } },
            { "Story14", new Hashtable() { { "FogColor", new Color(0.7f, 0.56f, 0.45f, 1f) }, { "FogEnd", 4000f }, { "FogStart", 500f }, { "DLColor", new Color(0.82f, 0.63f, 0.32f, 1f) }, { "DLX", 53f }, { "DLY", 105f } } },
            //{ "Story15", new Hashtable() { { "FogColor", new Color(0.28f, 0.35f, 0.42f, 1f) } } },
            { "Story16", new Hashtable() { { "FogColor", new Color(0.25f, 0.33f, 0.5f, 1f) }, { "DLColor", new Color(0.74f, 0.72f, 0.85f, 1f) }, { "FogEnd", 2500f } } },
            { "Story17", new Hashtable() { { "FogColor", new Color(0.47f, 0.41f, 0.52f, 1f) } } },
            { "Story18", new Hashtable() { { "FogColor", new Color(0.3f, 0.25f, 0.25f, 1f) }, { "FogEnd", 2500f } } },
            { "Story19", new Hashtable() { { "FogColor", new Color(0.3f, 0.26f, 0.25f, 1f) }, { "FogEnd", 2250f }, { "DLColor", new Color(1f, 0.81f, 0.56f, 1f) }, { "DLX", 53f }, { "DLY", 105f  } } },
            { "Story20", new Hashtable() { { "FogColor", new Color(0.54f, 0.32f, 0.32f, 1f) }, { "FogEnd", 4500f }, { "DLColor", new Color(0.88f, 0.64f, 0.61f, 1f) }, { "DLIntensity", 0.35f }, { "DLX", 56f }, { "DLY", 107f } } },
        };

        public static bool HasSkyboxes
        {
            get;
            private set;
        }

        public static Material[] Skyboxes
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            HasSkyboxes = AdditionalContentLoader.HasLoadedContent(RequiredContent);
            if (!HasSkyboxes)
                return;

            Skyboxes = OverhaulAssetsController.GetAllObjects<Material>(SkyboxesAssetBundle);
            
            OverhaulEventsController.AddEventListener(GlobalEvents.LightSettingsRefreshed, refreshSkybox, true);
            OverhaulEventsController.AddEventListener(AdvancedPhotomodeController.PhotoModeSettingUpdateEvent, refreshSkyboxPhotomode);
        }

        public void SetSkybox(int index)
        {
            if (index == -1)
            {
                LevelLightSettings levelLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
                if (levelLightSettings && AdvancedPhotomodeSettings.MoreSkyboxesIndex == -1)
                {
                    Material material2 = PhotoManager.Instance.IsInPhotoMode() ? AdvancedPhotomodeSettings.SkyboxMaterial : SkyBoxManager.Instance.LevelConfigurableSkyboxes[levelLightSettings.SkyboxIndex];
                    if (material2)
                    {
                        RenderSettings.skybox = material2;
                    }
                }
                return;
            }

            Material material = Skyboxes[index];
            RenderSettings.skybox = material;
        }

        private void refreshSkyboxPhotomode()
        {
            if (!PhotoManager.Instance.IsInPhotoMode())
            {
                return;
            }
 
            SetSkybox(AdvancedPhotomodeSettings.MoreSkyboxesIndex);
        }

        private void refreshSkybox()
        {
            LevelDescription levelDescription = null;
            try
            {
                levelDescription = LevelManager.Instance.GetCurrentLevelDescription();
            }
            catch
            {
                return;
            }

            if (levelDescription == null || string.IsNullOrEmpty(levelDescription.PrefabName))
                return;

            string levelId = string.Empty;
            if (levelDescription.PrefabName.Contains("/"))
            {
                levelId = levelDescription.PrefabName.Substring(levelDescription.PrefabName.LastIndexOf("/") + 1);
            }
            else
            {
                levelId = levelDescription.PrefabName;
            }

            if(SkyboxOverrides.TryGetValue(levelId, out int value))
            {
                SetSkybox(value);
            }

            if (LightSettingOverrides.TryGetValue(levelId, out Hashtable hashtable))
            {
                foreach(string key in hashtable.Keys)
                {
                    if(key == "FogColor")
                    {
                        RenderSettings.fogColor = (Color)hashtable[key];
                    }
                    if (key == "FogEnd")
                    {
                        RenderSettings.fogEndDistance = (float)hashtable[key];
                    }
                    if (key == "FogStart")
                    {
                        RenderSettings.fogStartDistance = (float)hashtable[key];
                    }
                    else if (key == "DLColor")
                    {
                        DirectionalLightManager.Instance.DirectionalLight.color = (Color)hashtable[key];
                    }
                    else if (key == "AmbientColor")
                    {
                        RenderSettings.ambientLight = (Color)hashtable[key];
                    }
                    else if (key == "DLIntensity")
                    {
                        DirectionalLightManager.Instance.DirectionalLight.intensity = (float)hashtable[key];
                    }
                    else if (key == "DLX")
                    {
                        Vector3 eulerAngles = DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles;
                        eulerAngles.x = (float)hashtable[key];
                        DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles = eulerAngles;
                    }
                    else if (key == "DLY")
                    {
                        Vector3 eulerAngles = DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles;
                        eulerAngles.y = (float)hashtable[key];
                        DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles = eulerAngles;
                    }
                }
            }
        }
    }
}
