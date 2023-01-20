using UnityEngine;

namespace CloneDroneOverhaul.V3.Base
{
    public class PatchesManager : V3.Base.V3_ModControllerBase
    {
        public static float FPSCap;

        void Start()
        {
            SkyBoxManager instance = Singleton<SkyBoxManager>.Instance;
            instance.LevelConfigurableSkyboxes[7] = OverhaulCacheAndGarbageController.GetCached<Material>("SkyboxMaterial_Stars2");
            instance.LevelConfigurableSkyboxes[2] = OverhaulCacheAndGarbageController.GetCached<Material>("SkyboxMaterial_StarsChapter4");
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Patches.QoL.Fix sounds")
            {
                bool val = (bool)value;
                UpdateAudioSettings(val);
            }
            if (settingName == "Graphics.Additions.HUD Scale")
            {
                float val = (float)value;
                GameUIRoot.Instance.GetComponent<Canvas>().scaleFactor = 1.7f + val;
                V3_MainModController.ModGUICanvas.scaleFactor = 1f + val;
            }
            if (settingName == "Patches.GUI.Pixel perfect HUD")
            {
                GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = (bool)value;
                V3_MainModController.ModGUICanvas.pixelPerfect = (bool)value;
            }
            if (settingName == "Graphics.Settings.FPS Cap")
            {
                try
                {
                    FPSCap = (float)value * 30f;
                    SettingsManager.Instance.SetVsyncOn(FPSCap == 60f);
                    if (FPSCap == 600f)
                    {
                        Application.targetFrameRate = -1;
                        return;
                    }
                    Application.targetFrameRate = UnityEngine.Mathf.RoundToInt(FPSCap) - 1;
                }
                catch
                {
                    return;
                }
            }
            if (settingName == "Graphics.Settings.Light limit")
            {
                switch ((int)value)
                {
                    case 0:
                        QualitySettings.pixelLightCount = 3;
                        return;
                    case 1:
                        QualitySettings.pixelLightCount = 6;
                        return;
                    case 2:
                        QualitySettings.pixelLightCount = 12;
                        return;
                    case 3:
                        QualitySettings.pixelLightCount = 30;
                        break;
                    default:
                        return;
                }
            }
        }

        public void UpdateAudioSettings(bool value)
        {
            if (value)
            {
                AudioConfiguration config = AudioSettings.GetConfiguration();
                config.numVirtualVoices += 7;
                config.numRealVoices += 5;
            }
            else
            {
                AudioConfiguration config = AudioSettings.GetConfiguration();
                config.numVirtualVoices = VanillaPrefs.AudioConfig.numVirtualVoices;
                config.numRealVoices = VanillaPrefs.AudioConfig.numRealVoices;
            }
        }
    }
}
