using OverhaulMod.Content.LevelEditor;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Visuals.Environment
{
    public class WeatherManager : Singleton<WeatherManager>, IGameLoadListener
    {
        [ModSetting(ModSettingIDs.ENABLE_WEATHER, true)]
        public static bool EnableWeather;

        [ModSetting(ModSettingIDs.FORCE_WEATHER_TYPE, 0)]
        public static int ForceWeatherType;

        public static readonly List<Dropdown.OptionData> WeatherOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("None"),
            new Dropdown.OptionData("Rainy"),
            new Dropdown.OptionData("Snowy"),
        };

        private bool _hasAddedEventListeners;

        private float _timeToUpdate;

        public List<WeatherInfo> WeatherInfos;

        private Dictionary<string, ParticleSystem> _nameToVFX;

        public static Vector3 vectorOffset
        {
            get
            {
                return Vector3.up * 180f;
            }
        }

        public Transform weatherVFXHolder
        {
            get;
            private set;
        }

        public LevelEditorWeatherSettingsOverride weatherOverrideObject
        {
            get;
            set;
        }

        private void Start()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.WeatherSystem))
            {
                base.enabled = false;
                return;
            }

            createWeatherInfos();
            createHolderIfNull();
            populateHolder();
        }

        private void OnDestroy()
        {
            if (_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);
                GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.LevelEditorLevelOpened, onLevelSpawned);
                _hasAddedEventListeners = false;
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            _timeToUpdate -= deltaTime;
            if (_timeToUpdate < 0f)
            {
                _timeToUpdate = 3f;
                Transform transform = weatherVFXHolder;
                if (transform)
                {
                    Camera camera = Camera.main;
                    LevelEditorWeatherSettingsOverride levelEditorWeatherSettingsOverride = weatherOverrideObject;
                    Transform target = GameModeManager.IsInLevelEditor() ? (levelEditorWeatherSettingsOverride ? levelEditorWeatherSettingsOverride.transform : null) : (camera ? camera.transform : null);
                    bool targetIsNotNull = target;

                    if (targetIsNotNull && Physics.Raycast(target.transform.position, Vector3.up, out RaycastHit hitInfo, 1000f, PhysicsManager.GetEnvironmentLayerMask(), QueryTriggerInteraction.UseGlobal))
                    {
                        transform.position = hitInfo.transform.position + vectorOffset;
                    }
                    else
                    {
                        transform.position = targetIsNotNull ? target.transform.position + vectorOffset : vectorOffset;
                    }
                }
            }
        }

        public void RefreshWeather()
        {
            refreshWeatherBasedOnLevel();
        }

        private void createWeatherInfos()
        {
            if (WeatherInfos != null)
                return;

            WeatherInfos = new List<WeatherInfo>
            {
                new WeatherInfo()
                {
                    Name = "Clear",
                    IsDefault = true,
                },
                new WeatherInfo()
                {
                    Name = "Snowy",
                    EmissionRate = 75f,
                    BundleAndAsset = new KeyValuePair<string, string>(ModAssetBundles.VFX, "WeatherVFX_Snow"),
                },
                new WeatherInfo()
                {
                    Name = "Rainy",
                    EmissionRate = 125f,
                    BundleAndAsset = new KeyValuePair<string, string>(ModAssetBundles.VFX, "WeatherVFX_Rain"),
                },
            };
        }

        private void createHolderIfNull()
        {
            if (weatherVFXHolder)
                return;

            GameObject newHolder = new GameObject("OverhaulWeather");
            newHolder.transform.position = vectorOffset;
            DontDestroyOnLoad(newHolder);
            weatherVFXHolder = newHolder.transform;
        }

        private void populateHolder()
        {
            Transform holderTransform = weatherVFXHolder;
            if (!holderTransform)
                return;

            if (_nameToVFX == null)
                _nameToVFX = new Dictionary<string, ParticleSystem>();
            else
                _nameToVFX.Clear();

            if (holderTransform.childCount != 0)
                TransformUtils.DestroyAllChildren(holderTransform);

            foreach (WeatherInfo info in WeatherInfos)
            {
                if (info.IsDefault)
                    continue;

                Transform instantiatedObjectTransform = Instantiate(info.GetVFXPrefab(), holderTransform).transform;
                instantiatedObjectTransform.localPosition = Vector3.zero;
                instantiatedObjectTransform.localEulerAngles = Vector3.zero;
                instantiatedObjectTransform.localScale = Vector3.one;
                instantiatedObjectTransform.gameObject.SetActive(true);
                ParticleSystem particleSystem = instantiatedObjectTransform.GetComponent<ParticleSystem>();
                particleSystem.SetEmissionEnabled(false);
                _nameToVFX.Add(info.Name, particleSystem);
            }
        }

        private void refreshWeatherBasedOnLevel()
        {
            DeactivateAllParticles();

            if (!EnableWeather)
                return;

            LevelEditorWeatherSettingsOverride levelEditorWeatherSettingsOverride = weatherOverrideObject;

            bool forceWeatherValueEnabled = ForceWeatherType != 0;

            int max = 250;
            float rate = 75f;
            float intensity = 1f;
            string weatherValue = forceWeatherValueEnabled ? WeatherOptions[Mathf.Clamp(ForceWeatherType, 0, 2)].text : string.Empty;
            bool isVanillaLevel = !levelEditorWeatherSettingsOverride && (!LevelManager.Instance || !LevelManager.Instance.IsCurrentLevelHidingTheArena() || GameModeManager.IsBattleRoyale());
            if (!forceWeatherValueEnabled && isVanillaLevel)
            {
                DateTime now = DateTime.Now;
                DateTime start = new DateTime(now.Year, 11, 25);
                DateTime end = new DateTime(now.Year, 1, 25);
                bool shouldBeSnowy = end < start ? (now >= start || now <= end) : (now <= end && now >= start);
                if (shouldBeSnowy)
                {
                    weatherValue = "Snowy";
                }
            }

            if (levelEditorWeatherSettingsOverride)
            {
                weatherValue = levelEditorWeatherSettingsOverride.WeatherType;
                intensity = levelEditorWeatherSettingsOverride.Intensity;
                max = levelEditorWeatherSettingsOverride.MaxParticles;
            }

            WeatherInfo weatherInfo = GetWeatherInfo(weatherValue);
            if (weatherInfo != null)
            {
                rate = weatherInfo.EmissionRate * intensity;
            }

            ActivateParticles(weatherValue, rate, max);
        }

        public List<Dropdown.OptionData> GetTranslatedWeatherOptions()
        {
            string langCode = LocalizationManager.Instance.GetCurrentLanguageCode();
            string key = $"weather_options_{langCode}";
            if (ModAdvancedCache.TryGet(key, out List<Dropdown.OptionData> options))
                return options;

            List<Dropdown.OptionData> list = WeatherOptions.CloneList();
            foreach (Dropdown.OptionData option in list)
            {
                option.text = LocalizationManager.Instance.GetTranslatedString($"settings_option_{option.text.ToLower()}");
            }
            ModAdvancedCache.Add(key, list);
            return list;
        }

        public WeatherInfo GetWeatherInfo(string name)
        {
            if (WeatherInfos.IsNullOrEmpty())
                return null;

            foreach (WeatherInfo info in WeatherInfos)
                if (info != null && info.Name == name)
                    return info;

            return null;
        }

        public void DeactivateAllParticles()
        {
            if (_nameToVFX == null || _nameToVFX.Count == 0)
                return;

            foreach (ParticleSystem value in _nameToVFX.Values)
            {
                if (value)
                    value.SetEmissionEnabled(false);
            }
        }

        public void ActivateParticles(string name, float emissionRate, int maxParticles = 250)
        {
            if (_nameToVFX != null && _nameToVFX.ContainsKey(name))
            {
                ParticleSystem ps = _nameToVFX[name];
                if (ps)
                {
                    ParticleSystem.EmissionModule emission = ps.emission;
                    emission.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRate);
                    emission.enabled = true;
                    ParticleSystem.MainModule main = ps.main;
                    main.maxParticles = Mathf.Clamp(maxParticles, 1, 2000);
                }
            }
        }

        public void OnGameLoaded()
        {
            DeactivateAllParticles();

            if (!_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);
                GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelEditorLevelOpened, onLevelSpawned);
                _hasAddedEventListeners = true;
            }
        }

        private void onLevelSpawned()
        {
            waitThenRefreshWeather().Run();
        }

        private IEnumerator waitThenRefreshWeather()
        {
            yield return new WaitForSeconds(3f);
            refreshWeatherBasedOnLevel();
            yield break;
        }
    }
}
