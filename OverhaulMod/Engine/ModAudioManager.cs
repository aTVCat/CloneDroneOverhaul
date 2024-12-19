using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ModAudioManager : Singleton<ModAudioManager>, IGameLoadListener
    {
        [ModSetting(ModSettingsConstants.ENABLE_REVERB_FILTER, true)]
        public static bool EnableReverbFilter;

        [ModSetting(ModSettingsConstants.REVERB_FILTER_INTENSITY, 0.6f)]
        public static float ReverbIntensity;

        [ModSetting(ModSettingsConstants.MUTE_SOUND_WHEN_UNFOCUSED, false)]
        public static bool MuteSoundWhenUnfocused;

        [ModSetting(ModSettingsConstants.MUTE_MASTER_VOLUME_WHEN_UNFOCUSED, true)]
        public static bool MuteMasterVolumeWhenUnfocused;

        [ModSetting(ModSettingsConstants.MUTE_MUSIC_WHEN_UNFOCUSED, true)]
        public static bool MuteMusicWhenUnfocused;

        [ModSetting(ModSettingsConstants.MUTE_COMMENTATORS_WHEN_UNFOCUSED, true)]
        public static bool MuteCommentatorsWhenUnfocused;

        [ModSetting(ModSettingsConstants.MUTE_SOUND_INSTANTLY_WHEN_UNFOCUSED, false)]
        public static bool MuteSoundInstantlyWhenUnfocused;

        [ModSetting(ModSettingsConstants.MUTE_SPEED_MULTIPLIER, 0.6f)]
        public static float MuteSpeedMultiplier;

        private AudioSource m_musicAudioSource;

        private AudioSource[] m_commentatorAudioSources;

        private GameObject m_loadingSoundSourcePrefab;

        private AudioClipDefinition m_customizationManagerAmbience;

        private float m_updateVolumeUntilTime;

        private float m_volumeMultiplier, m_prevVolumeMultiplier;

        private float m_masterVolume, m_musicVolume, m_commentatorsVolume;

        private bool m_focused;

        private void Start()
        {
            m_loadingSoundSourcePrefab = ModResources.Prefab(AssetBundleConstants.SFX, "LoadingSoundSource");
            m_customizationManagerAmbience = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "SB_Ambiance_19") };
            m_focused = !MuteSoundWhenUnfocused || Application.isFocused;
            m_volumeMultiplier = m_focused ? 1f : 0f;
            RefreshVolume();
        }

        private void Update()
        {
            if (ModBuildInfo.debug)
            {
                UIDeveloperMenu.SetKeyValue("Master volume", (m_masterVolume * (MuteMasterVolumeWhenUnfocused ? m_volumeMultiplier : 1f)).ToString());
                UIDeveloperMenu.SetKeyValue("Music volume", (m_musicVolume * (MuteMusicWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? m_volumeMultiplier : 1f)).ToString());
                UIDeveloperMenu.SetKeyValue("Commentators volume", (m_commentatorsVolume * (MuteCommentatorsWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? m_volumeMultiplier : 1f)).ToString());
                UIDeveloperMenu.SetKeyValue("Volume multiplier", m_volumeMultiplier.ToString());
                UIDeveloperMenu.SetKeyValue("Mute in progress?", (Time.unscaledTime < m_updateVolumeUntilTime).ToString());
            }

            if (Time.unscaledTime > m_updateVolumeUntilTime)
                return;

            m_volumeMultiplier = MuteSoundInstantlyWhenUnfocused ? (m_focused ? 1f : 0f) : Mathf.Clamp01(m_volumeMultiplier + (Time.unscaledDeltaTime * (5f * MuteSpeedMultiplier) * (m_focused ? 1f : -1f)));

            m_prevVolumeMultiplier = m_volumeMultiplier;
            RefreshVolume();
        }

        private void OnApplicationFocus(bool focused)
        {
            refreshVolumeSettings();
            m_focused = !MuteSoundWhenUnfocused || focused;
            m_updateVolumeUntilTime = Time.unscaledTime + 2f;
        }

        public void PlayCustomizationEditorAmbience()
        {
            AudioManager.Instance.PlayClipGlobal(m_customizationManagerAmbience, 0f, true, 0.7f);
        }

        public void PlayTransitionSound(float volumeOffset = 0f)
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.TransitionUpdates) || TransitionSoundBehaviour.Instance)
                return;

            GameObject gameObject = Instantiate(m_loadingSoundSourcePrefab);
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

        public void OnGameLoaded()
        {
            refreshAudioSources();
        }

        public void RefreshVolume()
        {
            SetVolume(m_masterVolume * (MuteMasterVolumeWhenUnfocused ? m_volumeMultiplier : 1f), m_musicVolume * (MuteMusicWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? m_volumeMultiplier : 1f), m_commentatorsVolume * (MuteCommentatorsWhenUnfocused && !MuteMasterVolumeWhenUnfocused ? m_volumeMultiplier : 1f));
        }

        public void StopChangingVolume(bool updateVolume = true)
        {
            m_updateVolumeUntilTime = -1f;
            refreshVolumeSettings();

            if (updateVolume)
                SetVolume(m_masterVolume, m_musicVolume, m_commentatorsVolume);
        }

        private void refreshAudioSources()
        {
            AudioManager audioManager = AudioManager.Instance;
            m_musicAudioSource = audioManager.MusicAudioSource;
            m_commentatorAudioSources = audioManager.CommentatorAudioSources;
        }

        private void refreshVolumeSettings()
        {
            SettingsManager settingsManager = SettingsManager.Instance;
            m_masterVolume = settingsManager.GetSoundVolume();
            m_musicVolume = settingsManager.GetMusicVolume();
            m_commentatorsVolume = settingsManager.GetCommentatorVolume();
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
            AudioSource source = m_musicAudioSource;
            if (source)
            {
                source.volume = value;
            }
        }

        public void SetCommentatorsVolume(float value)
        {
            AudioSource[] sources = m_commentatorAudioSources;
            if (sources != null)
            {
                foreach (AudioSource source in sources)
                    if (source)
                        source.volume = value;
            }
        }
    }
}
