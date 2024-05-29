using OverhaulMod.Utils;
using OverhaulMod.Visuals.Environment;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.Content.LevelEditor
{
    public class LevelEditorWeatherSettingsOverride : SimpleTriggerReceiver, IDropdownOptions
    {
        public static readonly List<Dropdown.OptionData> Options = new List<Dropdown.OptionData>()
        {
            new DropdownStringOptionData()
            {
                text = "None",
                StringValue = string.Empty
            },

            new DropdownStringOptionData()
            {
                text = "Rainy",
                StringValue = "Rainy"
            },

            new DropdownStringOptionData()
            {
                text = "Snowy",
                StringValue = "Snowy"
            },
        };

        [IncludeInLevelEditor(false, false)]
        public string WeatherType;

        [IncludeInLevelEditor(false, false)]
        public float Intensity = 1.5f;

        [IncludeInLevelEditor(1, 2500, false, false, true)]
        public int MaxParticles = 250;

        [IncludeInLevelEditor(false, false, false, true)]
        public bool RefreshWeatherOnEnable = true;

        public override void Start()
        {
            if (!GameModeManager.IsInLevelEditor())
            {
                ModUnityUtils.DisableRendererAndCollider(base.gameObject);
            }
        }

        public void OnEnable()
        {
            if (RefreshWeatherOnEnable)
                RefreshWeather();
        }

        [CallFromAnimation]
        [IncludeInLevelEditor]
        public void RefreshWeather()
        {
            WeatherManager.Instance.weatherOverrideObject = this;
            WeatherManager.Instance.RefreshWeather();
        }

        public override void activateFromTrigger()
        {
            RefreshWeather();
        }

        public List<Dropdown.OptionData> GetDropdownOptions(string fieldName)
        {
            return Options;
        }

        public bool ShouldShowDropdownOptions(string fieldName)
        {
            return fieldName == nameof(WeatherType);
        }

        public bool HasDropDownForValue(string fieldName)
        {
            return fieldName == nameof(WeatherType);
        }
    }
}
