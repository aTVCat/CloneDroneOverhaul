using CDOverhaul.Gameplay.QualityOfLife;
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
        };

        public static readonly Dictionary<string, Hashtable> LightSettingOverrides = new Dictionary<string, Hashtable>()
        {
            // LBS
            { "BR_PyramidsInSpace", new Hashtable() { { "FogColor", Color.white * 0.15f }, { "DLColor", Color.white * 0.15f }, { "AmbientColor", Color.clear } } },
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
            string levelId = string.Empty;
            try
            {
                levelId = LevelManager.Instance.GetCurrentLevelID();
            }
            catch
            {
                return;
            }

            if (string.IsNullOrEmpty(levelId))
                return;

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
                    else if (key == "DLColor")
                    {
                        DirectionalLightManager.Instance.DirectionalLight.color = (Color)hashtable[key];
                    }
                    else if (key == "AmbientColor")
                    {
                        RenderSettings.ambientLight = (Color)hashtable[key];
                    }
                }
            }
        }
    }
}
