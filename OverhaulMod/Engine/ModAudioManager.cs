using OverhaulMod.Utils;

namespace OverhaulMod.Engine
{
    public class ModAudioManager : Singleton<ModAudioManager>
    {
        [ModSetting(ModSettingConstants.ENABLE_REVERB_FILTER, true)]
        public static bool EnableReverbFilter;

        [ModSetting(ModSettingConstants.REVERB_FILTER_INTENSITY, 1f)]
        public static float ReverbIntensity;
    }
}
