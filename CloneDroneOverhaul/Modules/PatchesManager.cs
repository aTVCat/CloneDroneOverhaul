using CloneDroneOverhaul.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class PatchesManager : ModuleBase
    {
        public static PatchesManager Instance;

        public override void Start()
        {
            Instance = this;
        }

        public override void OnSettingRefreshed(string ID, object value)
        {
            if(ID == "Patches.QoL.Fix sounds")
            {
                bool val = (bool)value;
                UpdateAudioSettings(val);
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
