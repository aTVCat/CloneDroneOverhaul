using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
