using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class PatchesManager : ModuleBase
    {
        public static PatchesManager Instance;
        public static float FPSCap;

        public override void Start()
        {
            Instance = this;
            SkyBoxManager instance = Singleton<SkyBoxManager>.Instance;
            instance.LevelConfigurableSkyboxes[7] = OverhaulCacheManager.GetCached<Material>("SkyboxMaterial_Stars2");
            instance.LevelConfigurableSkyboxes[2] = OverhaulCacheManager.GetCached<Material>("SkyboxMaterial_StarsChapter4");
        }

        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Patches.QoL.Fix sounds")
            {
                bool val = (bool)value;
                UpdateAudioSettings(val);
            }
            if (ID == "Graphics.Additions.HUD Scale")
            {
                float val = (float)value;
                GameUIRoot.Instance.GetComponent<Canvas>().scaleFactor = 1.7f + val;
                OverhaulMain.ModGUICanvas.scaleFactor = 1f + val;
            }
            if(ID == "Patches.GUI.Pixel perfect HUD")
            {
                GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = (bool)value;
                OverhaulMain.ModGUICanvas.pixelPerfect = (bool)value;
            }
            if (ID == "Graphics.Settings.FPS Cap")
            {
                try // InvalidCastException
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
            if (ID == "Graphics.Settings.Light limit")
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
