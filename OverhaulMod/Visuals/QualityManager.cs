using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class QualityManager : Singleton<QualityManager>
    {
        [ModSetting(ModSettingsConstants.SHADOW_RESOLUTION, 3)]
        public static ShadowResolution ShadowResolution;

        [ModSetting(ModSettingsConstants.SHADOW_DISTANCE, 300f)]
        public static float ShadowDistance;

        [ModSetting(ModSettingsConstants.MAX_LIGHT_COUNT, 6)]
        public static int MaxLightCount;

        public void RefreshQualitySettings()
        {
            QualitySettings.shadowResolution = ShadowResolution;
            QualitySettings.shadowDistance = ShadowDistance;
            QualitySettings.pixelLightCount = MaxLightCount;
        }
    }
}
