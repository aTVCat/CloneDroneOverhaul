using OverhaulMod.Content.Personalization;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class TitleScreenCustomizationManager : Singleton<TitleScreenCustomizationManager>, IGameLoadListener
    {
        public const string CUSTOMIZATION_INFO_FILE = "TitleScreenCustomizationInfo.json";

        public const string CUSTO_LEVEL_ID = "customTitleScreenLevel";

        [ModSetting(ModSettingsConstants.TITLE_SCREEN_MUSIC_TRACK_INDEX, 0)]
        public static int MusicTrackIndex;

        [ModSetting(ModSettingsConstants.TITLE_SCREEN_PANEL_POSITION, TitleScreenPanelPosition.LeftSide)]
        public static TitleScreenPanelPosition PanelPosition;

        [ModSetting(ModSettingsConstants.TITLE_SCREEN_BACKGROUND_FADE_POWER, 1f)]
        public static float BackgroundFadePower;

        [ModSetting(ModSettingsConstants.CLONE_DRONE_LOGO_FIRE, false)]
        public static bool ShowLogoFireParticles;

        [ModSetting(ModSettingsConstants.TITLE_SCREEN_SHOW_MODBOT_ACCOUNT_INFO, true)]
        public static bool ShowModBotAccountInfo;

        [ModSetting(ModSettingsConstants.TITLE_SCREEN_SOCIAL_MEDIA_POPUPS, true)]
        public static bool ShowSocialMediaPopups;

        [ModSetting(ModSettingsConstants.TITLE_SCREEN_SOCIAL_MEDIA_BUTTONS, true)]
        public static bool ShowSocialMediaButtons;

        private TitleScreenCustomizationInfo _customizationInfo;

        private GameObject _levelIsLoadingBg;

        private float _timeToRefreshMusicTrack;

        private bool _isWaitingForSoundpackToLoad;

        public LevelDescription overrideLevelDescription
        {
            get;
            set;
        }

        public bool disallowClickingLogo
        {
            get;
            set;
        }

        public override void Awake()
        {
            base.Awake();

            _timeToRefreshMusicTrack = -1f;

            LoadCustomizationInfo();
        }

        private void Update()
        {
            if (_timeToRefreshMusicTrack != -1f && Time.unscaledTime >= _timeToRefreshMusicTrack)
            {
                _timeToRefreshMusicTrack = -1f;

                if (!_isWaitingForSoundpackToLoad)
                    RefreshMusicTrack();
            }
        }

        public void OnGameLoaded()
        {
            waitUntilSoundpackIsLoadedThenPlayMusic();

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelEditorStarted, StopTitleScreenMusic);
            GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.EDITOR_STARTED_EVENT, StopTitleScreenMusic);

            Transform transform = ArenaCameraManager.Instance?.ArenaCameraTransform?.parent;
            if (transform)
                transform.SetParent(WorldRoot.Instance?.transform);

            Camera camera = ArenaCameraManager.Instance?.TitleScreenLevelCamera;
            if (camera)
                camera.farClipPlane = 10000f;
        }

        public bool ShouldLockUserCustomization()
        {
            return ModBuild.ShouldShowHypocrisis3Special();
        }

        public void StopTitleScreenMusic()
        {
            ModGameUtils.FadeThenStopMusic(0.3f);
        }

        public void SetHypocrisisBackgroundLevel()
        {
            if (_customizationInfo.StaticBackgroundInfo.Level != null && _customizationInfo.StaticBackgroundInfo.Level.WorkshopItem != null && _customizationInfo.StaticBackgroundInfo.Level.WorkshopItem.WorkshopItemID == UITitleScreenHypocrisisSkin.MAIN_MENU_LEVEL_STEA_ID)
                return;

            if (ModSteamUGC.IsItemInstalled(UITitleScreenHypocrisisSkin.MAIN_MENU_LEVEL_STEA_ID) && SteamUGC.GetItemInstallInfo(UITitleScreenHypocrisisSkin.MAIN_MENU_LEVEL_STEA_ID, out _, out string folder, ModSteamUGC.cchFolderSize, out _))
            {
                SteamWorkshopItem workshopItem = null;

                List<LevelDescription> list = WorkshopLevelManager.Instance.GetAllWorkShopEndlessLevels();
                if (list != null && list.Count != 0)
                {
                    foreach (LevelDescription levelDescription in list)
                        if (levelDescription.WorkshopItem != null && levelDescription.WorkshopItem.WorkshopItemID == UITitleScreenHypocrisisSkin.MAIN_MENU_LEVEL_STEA_ID)
                        {
                            workshopItem = levelDescription.WorkshopItem;
                        }
                }

                if (workshopItem == null)
                {
                    workshopItem = new SteamWorkshopItem()
                    {
                        Title = "ARCHONETHER",
                        CreatorName = "Archaeologist",
                        WorkshopItemID = UITitleScreenHypocrisisSkin.MAIN_MENU_LEVEL_STEA_ID,
                        Folder = folder,
                    };
                }

                LevelDescription level = new LevelDescription()
                {
                    LevelID = CUSTO_LEVEL_ID,
                    LevelJSONPath = Path.Combine(folder, "LevelData.json"),
                    LevelTags = new List<LevelTags>(),
                    WorkshopItem = workshopItem
                };
                overrideLevelDescription = level;
                _customizationInfo.StaticBackgroundInfo.Level = level;
                SpawnStaticBackground();
                SaveCustomizationInfo();
            }
        }

        public void LoadCustomizationInfo()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo;
            try
            {
                titleScreenCustomizationInfo = ModDataManager.DeserializeFile<TitleScreenCustomizationInfo>(CUSTOMIZATION_INFO_FILE, false);
                titleScreenCustomizationInfo.FixValues();
            }
            catch (Exception)
            {
                titleScreenCustomizationInfo = new TitleScreenCustomizationInfo();
                titleScreenCustomizationInfo.FixValues();
            }
            _customizationInfo = titleScreenCustomizationInfo;
            overrideLevelDescription = titleScreenCustomizationInfo.StaticBackgroundInfo.Level;
        }

        public void SaveCustomizationInfo()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo = _customizationInfo;
            if (titleScreenCustomizationInfo == null)
                return;

            ModDataManager.SerializeToFile(CUSTOMIZATION_INFO_FILE, titleScreenCustomizationInfo, false);
        }

        public TitleScreenBackgroundInfo GetStaticBackgroundInfo()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo = _customizationInfo;
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

        public void SetStaticLevel(LevelDescription level, bool refresh = true)
        {
            TitleScreenBackgroundInfo bgInfo = GetStaticBackgroundInfo();
            if (bgInfo == null)
                return;

            bgInfo.Level = level;

            if (refresh)
            {
                overrideLevelDescription = level;
                SpawnStaticBackground();
            }
        }

        public void SpawnStaticBackground()
        {
            TitleScreenCustomizationInfo titleScreenCustomizationInfo = _customizationInfo;
            if (titleScreenCustomizationInfo == null)
                return;

            titleScreenCustomizationInfo.FixValues();

            _ = ModActionUtils.RunCoroutine(spawnStaticBackgroundCoroutine());
        }

        public void SetLevelIsLoadingBG(GameObject gameObject)
        {
            _levelIsLoadingBg = gameObject;
        }

        public List<Dropdown.OptionData> GetMusicTracks()
        {
            List<string> stringList = new List<string>(AudioLibrary.Instance.GetMusicClipNames());
            _ = stringList.Remove("Chapter4TravelInTubesMusic");

            List<Dropdown.OptionData> list = DropdownStringOptionData.GetOptionsData(stringList);
            list[0].text = "None";
            foreach (Dropdown.OptionData od in list)
            {
                od.text = StringUtils.AddSpacesToCamelCasedString(od.text);
                if (od.text.Contains("_"))
                    od.text = od.text.Replace("_", string.Empty);
                if (od.text.Contains("C5"))
                    od.text = od.text.Replace("C5", "Chapter 5");
                if (od.text.Contains("Chapter5"))
                    od.text = od.text.Replace("Chapter5", "Chapter 5");
                if (od.text.Contains("Chapter4"))
                    od.text = od.text.Replace("Chapter4", "Chapter 4");
                if (od.text.Contains("Chapter3"))
                    od.text = od.text.Replace("Chapter3", "Chapter 3");
                if (od.text.Contains("Chapter2"))
                    od.text = od.text.Replace("Chapter2", "Chapter 2");
                if (od.text.Contains("Ghost"))
                    od.text = od.text.Replace("Ghost", "Emilia");
                if (od.text.Contains("Spidertron7000"))
                    od.text = od.text.Replace("Spidertron7000", "Spidertron 7000");
                if (od.text.Contains(" Music"))
                    od.text = od.text.Replace(" Music", string.Empty);
                if (od.text.Contains("Photo Clip"))
                    od.text = od.text.Replace("Photo Clip", "Photo Mode");
            }
            return list;
        }

        private void waitUntilSoundpackIsLoadedThenPlayMusic()
        {
            if (_isWaitingForSoundpackToLoad)
                return;

            _isWaitingForSoundpackToLoad = true;
            waitUntilSoundpackIsLoadedThenPlayMusicCoroutine().Run();
        }

        private IEnumerator waitUntilSoundpackIsLoadedThenPlayMusicCoroutine()
        {
            while (!ModIntegrationUtils.SoundpackMod.HasLoadedSoundpack())
                yield return null;

            _isWaitingForSoundpackToLoad = false;
            RefreshMusicTrack();

            yield break;
        }

        public void RefreshMusicTrackDelayed(float time)
        {
            _timeToRefreshMusicTrack = Time.unscaledTime + time;
        }

        public void RefreshMusicTrack()
        {
            if (!GameModeManager.IsOnTitleScreen())
                return;

            CreditsCrawlAnimation creditsCrawlAnimation = ModCache.UIRoot.CreditsCrawl;
            if (creditsCrawlAnimation && creditsCrawlAnimation._isShowing)
                return;

            List<Dropdown.OptionData> list = GetMusicTracks();
            if (list.IsNullOrEmpty() || MusicTrackIndex < 0 || MusicTrackIndex >= list.Count)
                return;

            bool isHcModEnabled = ModBuild.ShouldShowHypocrisis3Special();
            if (!isHcModEnabled && MusicTrackIndex == 0)
            {
                AudioManager.Instance.StopMusic();
                return;
            }

            AudioClipDefinition audioClip = AudioLibrary.Instance.GetAudioClip(isHcModEnabled ? "Chapter4VictoryMusic" : ((list[MusicTrackIndex] as DropdownStringOptionData).StringValue));
            if (audioClip != null)
            {
                AudioManager.Instance.PlayMusicClip(audioClip, true, true);
                AudioManager.Instance.FadeInMusic(2f);
            }
        }

        public Transform LoadLevel(string levelJSONPath, LevelManager levelManager)
        {
            LevelEditorLevelData levelEditorLevelData;
            try
            {
                levelEditorLevelData = ModJsonUtils.DeserializeStream<LevelEditorLevelData>(levelJSONPath);
            }
            catch (Exception)
            {
                levelEditorLevelData = new LevelEditorLevelData()
                {
                    RootLevelObject = new LevelEditorLevelObject(),
                    DifficultyConfigurations = new List<LevelEditorDifficultyData>(),
                    ModdedMetadata = new Dictionary<string, string>()
                };
                levelEditorLevelData.AssignNewGeneratedUniqueID();
            }

            levelManager._currentWorkshopLevelDifficultyIndex = 0;
            levelManager._currentLevelHidesTheArena = levelEditorLevelData.ArenaIsHidden || levelEditorLevelData.LevelType == LevelType.Adventure;
            _ = levelManager.CreateLevelTransformFromLevelEditorData(levelEditorLevelData, false).MoveNext();
            return levelManager._currentLevelTransform;
        }

        private IEnumerator spawnStaticBackgroundCoroutine()
        {
            if (_levelIsLoadingBg)
                _levelIsLoadingBg.SetActive(true);

            yield return null;
            GameFlowManager.Instance.SwapOutTitleScreenLevel();
            yield return null;

            LevelManager levelManager = LevelManager.Instance;
            while (levelManager.IsCurrentlySwappingInLevel() || levelManager.IsSpawningCurrentLevel())
                yield return null;

            if (_levelIsLoadingBg)
                _levelIsLoadingBg.SetActive(false);

            yield break;
        }
    }
}
