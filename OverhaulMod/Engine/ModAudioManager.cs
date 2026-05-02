using OverhaulMod.Content.Personalization;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ModAudioManager : Singleton<ModAudioManager>, IGameLoadListener
    {
        [ModSetting(ModSettingIDs.ENABLE_REVERB_FILTER, true)]
        public static bool EnableReverbFilter;

        [ModSetting(ModSettingIDs.REVERB_FILTER_INTENSITY, 0.6f)]
        public static float ReverbIntensity;

        [ModSetting(ModSettingIDs.MUTE_SOUND_WHEN_UNFOCUSED, false)]
        public static bool MuteSoundWhenUnfocused;

        [ModSetting(ModSettingIDs.MUTE_MASTER_VOLUME_WHEN_UNFOCUSED, true)]
        public static bool MuteMasterVolumeWhenUnfocused;

        [ModSetting(ModSettingIDs.MUTE_MUSIC_WHEN_UNFOCUSED, true)]
        public static bool MuteMusicWhenUnfocused;

        [ModSetting(ModSettingIDs.MUTE_COMMENTATORS_WHEN_UNFOCUSED, true)]
        public static bool MuteCommentatorsWhenUnfocused;

        [ModSetting(ModSettingIDs.MUTE_SOUND_INSTANTLY_WHEN_UNFOCUSED, false)]
        public static bool MuteSoundInstantlyWhenUnfocused;

        [ModSetting(ModSettingIDs.MUTE_SPEED_MULTIPLIER, 0.6f)]
        public static float MuteSpeedMultiplier;

        [ModSetting(ModSettingIDs.UI_SOUNDS, true)]
        public static bool UISounds;

        private AudioSource _musicAudioSource;

        private AudioSource[] _commentatorAudioSources;

        private AudioSource _oneShotGlobalAudioSource;

        private GameObject _loadingSoundSourcePrefab, _customizationEditorAmbianceSourcePrefab;

        private float _updateVolumeUntilTime;

        private float _volumeMultiplier, _prevVolumeMultiplier;

        private float _masterVolume, _musicVolume, _commentatorsVolume;

        private bool _focused;

        private AudioSource _customizationEditorAmbiance;

        private float _timeCustomizationEditorAmbianceStartTime, _timeCustomizationEditorAmbianceStopTime;

        private void Start()
        {
            _timeCustomizationEditorAmbianceStartTime = -1f;
            _timeCustomizationEditorAmbianceStopTime = -1f;

            GameObject oneShotAudioSourceObject = new GameObject("OneShot Global Audio Source");
            oneShotAudioSourceObject.transform.SetParent(base.transform, false);
            AudioSource oneShotAudioSource = oneShotAudioSourceObject.AddComponent<AudioSource>();
            oneShotAudioSource.spatialBlend = 0f;
            _oneShotGlobalAudioSource = oneShotAudioSource;

            _loadingSoundSourcePrefab = ModResources.Prefab(ModAssetBundles.SFX, "LoadingSoundSource");
            _customizationEditorAmbianceSourcePrefab = ModResources.Prefab(ModAssetBundles.SFX, "CustEditorAmbianceSource");
            _focused = !MuteSoundWhenUnfocused || Application.isFocused;
            _volumeMultiplier = _focused ? 1f : 0f;
            RefreshVolume();
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.ExitingToMainMenu, StopCustomizationEditorAmbiance);
        }

        private void Update()
        {
            if (ModBuild.IsDebugBuild)
            {
                UIDeveloperMenu.SetKeyValue("Master volume", (_masterVolume * (MuteMasterVolumeWhenUnfocused ? _volumeMultiplier : 1f)).ToString());
                UIDeveloperMenu.SetKeyValue("Music volume", (_musicVolume * (MuteMusicWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? _volumeMultiplier : 1f)).ToString());
                UIDeveloperMenu.SetKeyValue("Commentators volume", (_commentatorsVolume * (MuteCommentatorsWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? _volumeMultiplier : 1f)).ToString());
                UIDeveloperMenu.SetKeyValue("Volume multiplier", _volumeMultiplier.ToString());
                UIDeveloperMenu.SetKeyValue("Mute in progress?", (Time.unscaledTime < _updateVolumeUntilTime).ToString());
            }

            if (_customizationEditorAmbiance)
            {
                if (_timeCustomizationEditorAmbianceStopTime != -1f)
                {
                    _customizationEditorAmbiance.volume = 1f - Mathf.Clamp01(Time.unscaledTime - _timeCustomizationEditorAmbianceStopTime);
                    if (_customizationEditorAmbiance.volume <= 0f)
                    {
                        Destroy(_customizationEditorAmbiance.gameObject);
                        _customizationEditorAmbiance = null;
                    }
                }
                else
                {
                    _customizationEditorAmbiance.volume = Mathf.Clamp01((Time.unscaledTime - _timeCustomizationEditorAmbianceStartTime) * 0.2f);
                }
            }

            if (Time.unscaledTime > _updateVolumeUntilTime)
                return;

            _volumeMultiplier = MuteSoundInstantlyWhenUnfocused ? (_focused ? 1f : 0f) : Mathf.Clamp01(_volumeMultiplier + (Time.unscaledDeltaTime * (5f * MuteSpeedMultiplier) * (_focused ? 1f : -1f)));

            _prevVolumeMultiplier = _volumeMultiplier;
            RefreshVolume();
        }

        private void OnApplicationFocus(bool focused)
        {
            refreshVolumeSettings();
            _focused = !MuteSoundWhenUnfocused || focused;
            _updateVolumeUntilTime = Time.unscaledTime + 2f;
        }

        public void PlayOrStopCustomizationEditorAmbiance()
        {
            if (!PersonalizationEditorManager.IsInEditorMode())
                return;

            if (PersonalizationEditorManager.PlayAmbiance)
            {
                _timeCustomizationEditorAmbianceStopTime = -1f;
                _timeCustomizationEditorAmbianceStartTime = Time.unscaledTime;
                PlayCustomizationEditorAmbiance();
            }
            else
            {
                _timeCustomizationEditorAmbianceStartTime = -1f;
                _timeCustomizationEditorAmbianceStopTime = Time.unscaledTime;
                StopCustomizationEditorAmbiance();
            }
        }

        public void PlayCustomizationEditorAmbiance()
        {
            if (_customizationEditorAmbiance)
                return;

            _timeCustomizationEditorAmbianceStopTime = -1f;
            _timeCustomizationEditorAmbianceStartTime = Time.unscaledTime;
            _customizationEditorAmbiance = Instantiate(_customizationEditorAmbianceSourcePrefab).GetComponent<AudioSource>();
        }

        public void StopCustomizationEditorAmbiance()
        {
            if (!_customizationEditorAmbiance)
                return;

            _timeCustomizationEditorAmbianceStartTime = -1f;
            _timeCustomizationEditorAmbianceStopTime = Time.unscaledTime;
        }

        public void PlayTransitionSound(float volumeOffset = 0f)
        {
            if (TransitionSoundBehaviour.Instance)
            {
                TransitionSoundBehaviour.Instance.PlayAgain(volumeOffset);
                return;
            }

            GameObject gameObject = Instantiate(_loadingSoundSourcePrefab);
            DontDestroyOnLoad(gameObject);
            TransitionSoundBehaviour transitionSoundBehaviour = gameObject.AddComponent<TransitionSoundBehaviour>();
            transitionSoundBehaviour.Initialize(volumeOffset);
        }

        public void StopTransitionSound()
        {
            if (!TransitionSoundBehaviour.Instance)
                return;

            TransitionSoundBehaviour.Instance.FadeOutSoundThenDestroySelf();
        }

        public void PlayOneShotGlobal(AudioClipDefinition audioClipDefinition)
        {
            if (!_oneShotGlobalAudioSource)
            {
                ModDebug.Warn("OneShot Global Audio Source has been destroyed!");
                return;
            }

            if (audioClipDefinition == null) return;

            _oneShotGlobalAudioSource.PlayOneShot(audioClipDefinition.Clip);
        }

        public void OnGameLoaded()
        {
            refreshAudioSources();
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ExitingToMainMenu, StopCustomizationEditorAmbiance);
        }

        public void RefreshVolume()
        {
            SetVolume(_masterVolume * (MuteMasterVolumeWhenUnfocused ? _volumeMultiplier : 1f), _musicVolume * (MuteMusicWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? _volumeMultiplier : 1f), _commentatorsVolume * (MuteCommentatorsWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? _volumeMultiplier : 1f));
        }

        public void StopChangingVolume(bool updateVolume = true)
        {
            _updateVolumeUntilTime = -1f;
            refreshVolumeSettings();

            if (updateVolume)
                SetVolume(_masterVolume, _musicVolume, _commentatorsVolume);
        }

        private void refreshAudioSources()
        {
            AudioManager audioManager = AudioManager.Instance;
            _musicAudioSource = audioManager.MusicAudioSource;
            _commentatorAudioSources = audioManager.CommentatorAudioSources;
        }

        private void refreshVolumeSettings()
        {
            SettingsManager settingsManager = SettingsManager.Instance;
            _masterVolume = settingsManager.GetSoundVolume();
            _musicVolume = settingsManager.GetMusicVolume();
            _commentatorsVolume = settingsManager.GetCommentatorVolume();
        }

        public void SetVolume(float master, float music, float commentators)
        {
            SetMasterVolume(master);
            SetMusicVolume(music);
            SetCommentatorsVolume(commentators);
        }

        public void SetMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        public void SetMusicVolume(float value)
        {
            AudioSource source = _musicAudioSource;
            if (source)
            {
                source.volume = value;
            }
        }

        public void SetCommentatorsVolume(float value)
        {
            AudioSource[] sources = _commentatorAudioSources;
            if (sources != null)
            {
                foreach (AudioSource source in sources)
                    if (source)
                        source.volume = value;
            }
        }
    }
}
