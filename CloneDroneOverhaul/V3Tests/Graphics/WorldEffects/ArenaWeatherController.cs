using UnityEngine;
using CloneDroneOverhaul.V3Tests.Base;
using ModLibrary;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class ArenaWeatherController : V3_ModControllerBase
    {
        public const string METADATA_LEVELWEATHER_KEY = LevelEditor.MetaDataController.OVERHAUL_MOD_KEY_PREFIX + "_WeatherState";
        public const string METADATA_LEVELWEATHER_RAINY_VALUE = "Rainy";

        private string _currentWeather;
        /// <summary>
        /// Get current level weather
        /// </summary>
        public string CurrentWeather
        {
            get
            {
                return _currentWeather;
            }
            set
            {
                _currentWeather = value;
                RefreshWeather();
            }
        }

        /// <summary>
        /// Does current level support weather?
        /// </summary>
        public bool WeatherEnabled { get; set; }

        private ParticleSystem Weather_Snow;

        private void Start()
        {
            Weather_Snow = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustNormalWinter")).GetComponent<ParticleSystem>();
            Weather_Snow.transform.position = new Vector3(0, 50, 0);
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if(eventName == "level.data")
            {
                if(args[0] != null)
                {
                    LevelEditorLevelData data = args[0] as LevelEditorLevelData;
                    if(data == null || data.ModdedMetadata == null)
                    {
                        WeatherEnabled = false;
                        return;
                    }

                    string weather = string.Empty;
                    data.ModdedMetadata.TryGetValue(METADATA_LEVELWEATHER_KEY, out weather);

                    WeatherEnabled = !data.ArenaIsHidden && data.LevelType == LevelType.ArenaLevel;
                    CurrentWeather = weather;
                }
                else
                {
                    WeatherEnabled = false;
                }
            }
        }

        public void RefreshWeather()
        {
            if (WeatherEnabled)
            {
                Weather_Snow.Play();
            }
            else
            {
                Weather_Snow.Stop();
                Weather_Snow.Clear(true);
            }
        }
    }
}