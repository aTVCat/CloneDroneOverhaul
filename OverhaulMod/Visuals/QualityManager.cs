using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [ModSetting(ModSettingsConstants.UNLIMITED_LIGHT_SOURCES, false)]
        public static bool UnlimitedLightSources;

        public void RefreshQualitySettings()
        {
            QualitySettings.shadowResolution = ShadowResolution;
            QualitySettings.shadowDistance = ShadowDistance;
            QualitySettings.pixelLightCount = UnlimitedLightSources ? 999 : MaxLightCount;
        }

        public List<Dropdown.OptionData> GetShadowResolutionOptions()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach (object enumValue in typeof(ShadowResolution).GetEnumValues())
            {
                string str;
                switch ((ShadowResolution)enumValue)
                {
                    case ShadowResolution.Low:
                        str = LocalizationManager.Instance.GetTranslatedString("settings_option_low");
                        break;
                    case ShadowResolution.Medium:
                        str = LocalizationManager.Instance.GetTranslatedString("settings_option_medium");
                        break;
                    case ShadowResolution.High:
                        str = LocalizationManager.Instance.GetTranslatedString("settings_option_high");
                        break;
                    case ShadowResolution.VeryHigh:
                        str = LocalizationManager.Instance.GetTranslatedString("settings_option_very_high");
                        break;
                    default:
                        str = StringUtils.AddSpacesToCamelCasedString(((ShadowResolution)enumValue).ToString());
                        break;
                }

                list.Add(new Dropdown.OptionData(str));
            }
            return list;
        }
    }
}
