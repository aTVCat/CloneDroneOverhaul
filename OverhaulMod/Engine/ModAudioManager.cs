using OverhaulMod.Utils;

namespace OverhaulMod.Engine
{
    public class ModAudioManager : Singleton<ModAudioManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_REVERB_FILTER, true)]
        public static bool EnableReverbFilter;

        [ModSetting(ModSettingsConstants.REVERB_FILTER_INTENSITY, 1f)]
        public static float ReverbIntensity;
    }
}
