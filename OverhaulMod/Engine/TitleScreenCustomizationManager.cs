using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class TitleScreenCustomizationManager : Singleton<TitleScreenCustomizationManager>, IGameLoadListener
    {
        public const string CUSTOMIZATION_INFO_FILE = "TitleScreenCustomizationInfo.json";

        [ModSetting(ModSettingConstants.TITLE_SCREEN_MUSIC_TRACK_INDEX, 0)]
        public static int MusicTrackIndex;

        private TitleScreenCustomizationInfo m_customizationInfo;

        public LevelDescription overrideLevelDescription
        {
            get;
            private set;
        }

        public bool disallowUserFromClickingLogo
        {
            get;
            set;
        }

        public override void Awake()
        {
            base.Awake();
            LoadCustomizationInfo();
        }

        public void OnGameLoaded()
        {
            RefreshMusicTrack();

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelEditorStarted, onLevelEditorStarted);
        }

        private void onLevelEditorStarted()
        {
            AudioManager.Instance.FadeOutMusic(1f);
            DelegateScheduler.Instance.Schedule(delegate
            {
                AudioManager.Instance.StopMusic();
            }, 1.25f);
        }

        public void LoadCustomizationInfo()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo;
            try
            {
                titleScreenCustomizationInfo = ModDataManager.Instance.DeserializeFromFile<TitleScreenCustomizationInfo>(CUSTOMIZATION_INFO_FILE, false);
                titleScreenCustomizationInfo.FixValues();
            }
            catch
            {
                titleScreenCustomizationInfo = new TitleScreenCustomizationInfo();
                titleScreenCustomizationInfo.FixValues();
            }
            m_customizationInfo = titleScreenCustomizationInfo;

            overrideLevelDescription = titleScreenCustomizationInfo.StaticBackgroundInfo.Level;
        }

        public void SaveCustomizationInfo()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo = m_customizationInfo;
            if (titleScreenCustomizationInfo == null)
                return;

            ModDataManager.Instance.SerializeToFile(CUSTOMIZATION_INFO_FILE, titleScreenCustomizationInfo, false);
        }

        public TitleScreenBackgroundInfo GetStaticBackgroundInfo()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo = m_customizationInfo;
            if (titleScreenCustomizationInfo == null)
                return null;

            TitleScreenBackgroundInfo bgInfo = titleScreenCustomizationInfo.StaticBackgroundInfo;
            if (bgInfo == null)
            {
                bgInfo = new TitleScreenBackgroundInfo();
                titleScreenCustomizationInfo.StaticBackgroundInfo = bgInfo;
            }
            return bgInfo;
        }

        public void SetStaticLevel(LevelDescription level, DifficultyTier difficulty, bool refresh = true)
        {
            TitleScreenBackgroundInfo bgInfo = GetStaticBackgroundInfo();
            if (bgInfo == null)
                return;

            bgInfo.Level = level;
            bgInfo.Tier = difficulty;

            if (refresh)
            {
                overrideLevelDescription = level;
                SpawnStaticBackground();
            }
        }

        public void SpawnStaticBackground()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo = m_customizationInfo;
            if (titleScreenCustomizationInfo == null)
                return;

            titleScreenCustomizationInfo.FixValues();
            GameFlowManager.Instance.SwapOutTitleScreenLevel();
            /*
            LevelManager levelManager = LevelManager.Instance;
            if (!levelManager || levelManager.IsSpawningCurrentLevel() || levelManager.IsCurrentlySwappingInLevel())
                return;

            levelManager.CleanUpLevelRootsWaitingForDestruction();
            levelManager.CleanUpLevelThisFrame();
            base.StartCoroutine(levelManager.SpawnCurrentLevel(true));
            ArenaCameraManager.Instance.WaitForPreviewCameraToSpawnThenPanToIt();*/
        }

        public List<Dropdown.OptionData> GetMusicTracks()
        {
            List<Dropdown.OptionData> list = DropdownStringOptionData.GetOptionsData(AudioLibrary.Instance.GetMusicClipNames());
            list[0].text = "None";
            return list;
        }

        public void RefreshMusicTrack()
        {
            if (!GameModeManager.IsOnTitleScreen())
                return;

            List<Dropdown.OptionData> list = GetMusicTracks();
            if (list.IsNullOrEmpty() || MusicTrackIndex < 0 || MusicTrackIndex >= list.Count)
                return;

            if (MusicTrackIndex == 0)
            {
                AudioManager.Instance.StopMusic();
                return;
            }

            AudioClipDefinition audioClip = AudioLibrary.Instance.GetAudioClip((list[MusicTrackIndex] as DropdownStringOptionData).StringValue);
            if (audioClip != null && audioClip.Clip)
            {
                AudioManager.Instance.PlayMusicClip(audioClip, true, true);
                AudioManager.Instance.FadeInMusic(2f);
            }
        }
    }
}
