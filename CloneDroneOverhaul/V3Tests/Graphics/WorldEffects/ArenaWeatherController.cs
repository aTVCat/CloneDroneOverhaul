using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.V3Tests.Base;
using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class ArenaWeatherController : V3_ModControllerBase
    {
        public const string METADATA_LEVELWEATHER_KEY = LevelEditor.MetaDataController.OVERHAUL_MOD_KEY_PREFIX + "Overhaul_WeatherState";
        public const string METADATA_LEVELWEATHER_RAINY_VALUE = "Rainy";

        private static bool _hasSpawnedStaticEnvironmentEffects;
        private static ParticleSystem _environmentDust_Normal;
        private static ParticleSystem _environmentDust_Mindspace_Zero;
        private static ParticleSystem _environmentDust_Mindspace_One;
        public static bool IsDustEnabled;

        private float _timeLeftToNextRefresh;

        private string _currentWeather;
        /// <summary>
        /// Get current level weather
        /// </summary>
        public string CurrentWeather
        {
            get => _currentWeather;
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
        private ParticleSystem Weather_Rain;

        private void Awake()
        {
            Weather_Snow = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustNormalWinter")).GetComponent<ParticleSystem>();
            Weather_Snow.transform.position = new Vector3(0, 50, 0);
            Weather_Snow.Stop();

            Weather_Rain = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("overhaulstuff_p2", "Weather_Rain")).GetComponent<ParticleSystem>();
            Weather_Rain.transform.position = new Vector3(0, 50, 0);
            Weather_Rain.Stop();

            if (!_hasSpawnedStaticEnvironmentEffects)
            {
                _hasSpawnedStaticEnvironmentEffects = true;
                _environmentDust_Mindspace_One = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM1")).GetComponent<ParticleSystem>();
                _environmentDust_Mindspace_Zero = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM0")).GetComponent<ParticleSystem>();
                _environmentDust_Normal = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustNormal")).GetComponent<ParticleSystem>();

                _environmentDust_Mindspace_One.Stop();
                _environmentDust_Mindspace_Zero.Stop();
                _environmentDust_Normal.Stop();

                DontDestroyOnLoad(_environmentDust_Mindspace_One);
                DontDestroyOnLoad(_environmentDust_Mindspace_Zero);
                DontDestroyOnLoad(_environmentDust_Normal);
            }
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "level.data")
            {
                CurrentWeather = string.Empty;
                if (args[0] != null)
                {
                    LevelEditorLevelData data = args[0] as LevelEditorLevelData;
                    if (data == null || data.ModdedMetadata == null)
                    {
                        WeatherEnabled = false;
                        return;
                    }

                    string weather = string.Empty;
                    LevelEditorModdedMetadataManager.TryGetMetadata(METADATA_LEVELWEATHER_KEY, out weather);

                    WeatherEnabled = !data.ArenaIsHidden && data.LevelType == LevelType.ArenaLevel;
                    CurrentWeather = weather;
                }
                else
                {
                    WeatherEnabled = false;
                }
            }
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Graphics.World.Floating dust")
            {
                IsDustEnabled = (bool)value;
            }
        }

        public void RefreshWeather()
        {
            Weather_Rain.Stop();
            Weather_Snow.Stop();
            Weather_Snow.Clear(true);

            if (GameModeManager.IsOnTitleScreen())
            {
                Weather_Snow.Play();
            }

            if(CurrentWeather == METADATA_LEVELWEATHER_RAINY_VALUE)
            {
                Weather_Rain.Play();
            }

            /*
            if (WeatherEnabled && IsDustEnabled)
            {
                Weather_Snow.Play();
            }*/
        }

        private void Update()
        {
            _timeLeftToNextRefresh -= Time.deltaTime;
            if (_timeLeftToNextRefresh <= 0f && _hasSpawnedStaticEnvironmentEffects && Time.timeSinceLevelLoad > 5)
            {
                _timeLeftToNextRefresh = 0.5f;

                _environmentDust_Mindspace_One.Stop();
                _environmentDust_Mindspace_Zero.Stop();
                _environmentDust_Normal.Stop();

                if (!IsDustEnabled)
                {
                    return;
                }

                RobotShortInformation information = GameStatisticsController.GameStatistics.PlayerRobotInformation;
                if (information == null || information.IsNull || GameModeManager.IsOnTitleScreen())
                {
                    _environmentDust_Normal.Play();
                    _environmentDust_Normal.transform.position = Vector3.zero;
                    return;
                }

                Character player = information.Instance;
                if (information.IsFPMMindspace)
                {
                    _environmentDust_Mindspace_One.Play();
                    _environmentDust_Mindspace_Zero.Play();

                    _environmentDust_Mindspace_One.transform.position = player.transform.position;
                    _environmentDust_Mindspace_Zero.transform.position = player.transform.position;
                }
                else
                {
                    _environmentDust_Normal.Play();

                    _environmentDust_Normal.transform.position = player.transform.position;
                }

            }
        }
    }
}