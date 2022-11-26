using CloneDroneOverhaul.Utilities;
using System.Collections.Generic;
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
        }

        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if(ID == "Patches.QoL.Fix sounds")
            {
                bool val = (bool)value;
                UpdateAudioSettings(val);
            }
            if(ID == "Graphics.Settings.FPS Cap")
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
