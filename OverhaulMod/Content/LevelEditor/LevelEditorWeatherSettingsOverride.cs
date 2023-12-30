using OverhaulMod.Utils;
using OverhaulMod.Visuals.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Content.LevelEditor
{
    public class LevelEditorWeatherSettingsOverride : OverhaulBehaviour
    {
        [IncludeInLevelEditor(false, false)]
        public string WeatherType;

        [IncludeInLevelEditor(false, false)]
        public float Intensity;

        [IncludeInLevelEditor(1, 2500, false, false, true)]
        public int MaxParticles = 250;

        [IncludeInLevelEditor(false, false, false, true)]
        public bool RefreshWeatherOnEnable;

        public override void Start()
        {
            if (!GameModeManager.IsInLevelEditor())
            {
                ModUnityUtils.DisableRendererAndCollider(base.gameObject);
            }
        }

        public override void OnEnable()
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
    }
}
