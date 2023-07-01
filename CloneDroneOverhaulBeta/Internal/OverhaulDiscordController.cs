using Discord;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul
{
    internal class OverhaulDiscordController : OverhaulController
    {
        [OverhaulSetting("Gameplay.Discord.Enable Overhaul Activity", true, false, "Restart the game if you've toggled this setting")]
        public static bool IsEnabled;

        /// <summary>
        /// Overhaul mod Discord App ID
        /// </summary>
        public const long ApplicationID = 1091373211163308073;
        public const CreateFlags CreateFlag = CreateFlags.NoRequireDiscord;

        public const float ClientActivityRefreshRate = 1f;

        public static OverhaulDiscordController Instance;
        private static Discord.Discord s_DiscordClient;
        private static Activity s_DiscordClientActivity;
        private static ActivityManager.UpdateActivityHandler s_UpdateClientActivityHandler;

        private static bool s_Initialized;
        public static bool HasInitialized => s_Initialized && s_DiscordClient != null && Instance;

        private float m_UnscaledTimeToInitializeDiscord;
        private float m_TimeLeftToRefresh;

        /// <summary>
        /// The gamemode player is playing right now
        /// </summary>
        public string CurrentGamemode { get; set; }

        /// <summary>
        /// The details of gamemode (Progress for example)
        /// </summary>
        public string CurrentGamemodeDetails { get; set; }

        private static bool m_HasUser;
        private static User m_User;

        public long UserID
        {
            get
            {
                if (m_HasUser)
                    return m_User.Id;

                if (!HasInitialized)
                    return -1;

                UserManager m = s_DiscordClient.GetUserManager();
                if (m == null)
                    return -1;

                User u;
                try
                {
                    u = m.GetCurrentUser();
                    m_HasUser = true;
                    m_User = u;
                }
                catch
                {
                    return -1;
                }
                return u.Id;
            }
        }
        public string UserName
        {
            get
            {
                if (m_HasUser)
                    return m_User.Username;

                if (!HasInitialized)
                    return string.Empty;

                UserManager m = s_DiscordClient.GetUserManager();
                if (m == null)
                    return string.Empty;

                User u;
                try
                {
                    u = m.GetCurrentUser();
                    m_HasUser = true;
                    m_User = u;
                }
                catch
                {
                    return string.Empty;
                }
                return u.Username;
            }
        }
        public string UserDiscriminator
        {
            get
            {
                if (m_HasUser)
                    return m_User.Discriminator;

                if (!HasInitialized)
                    return string.Empty;

                UserManager m = s_DiscordClient.GetUserManager();
                if (m == null)
                    return string.Empty;

                User u;
                try
                {
                    u = m.GetCurrentUser();
                    m_HasUser = true;
                    m_User = u;
                }
                catch
                {
                    return string.Empty;
                }
                return u.Discriminator;
            }
        }

        public override void Initialize()
        {
            Instance = this;
            m_UnscaledTimeToInitializeDiscord = IsEnabled ? Time.unscaledTime + 1.5f : -1f;
        }

        protected override void OnDisposed()
        {
            Instance = null;
            base.OnDisposed();
        }

        private void Update()
        {
            if (m_UnscaledTimeToInitializeDiscord != -1f && Time.unscaledTime >= m_UnscaledTimeToInitializeDiscord)
            {
                TryInitializeDiscord();
                m_UnscaledTimeToInitializeDiscord = -1f;
            }

            if (!HasInitialized)
                return;

            try
            {
                s_DiscordClient.RunCallbacks();
            }
            catch
            {
                InterruptDiscord();
                return;
            }

            m_TimeLeftToRefresh -= Time.unscaledDeltaTime;
            if (m_TimeLeftToRefresh <= 0f && s_DiscordClient != null)
            {
                m_TimeLeftToRefresh = ClientActivityRefreshRate;
                UpdateActivity();
            }
        }

        private void OnApplicationQuit() => DestroyDiscord();

        public void TryInitializeDiscord()
        {
            if (!base.enabled || s_Initialized)
                return;

            Discord.Discord client = null;
            Activity clientActivity;
            ActivityAssets clientActivityAssets;

            try
            {
                client = new Discord.Discord(ApplicationID, (ulong)CreateFlag);
            }
            catch
            {
                InterruptDiscord();
                return;
            }

            s_DiscordClient = client;
            clientActivityAssets = new ActivityAssets
            {
                LargeImage = "defaultimage"
            };
            clientActivity = new Activity
            {
                Assets = clientActivityAssets,
                ApplicationId = 1091373211163308073,
                Name = "Overhaul Mod",
                Details = "V" + OverhaulVersion.ModVersion.ToString(),
                Type = ActivityType.Playing
            };
            s_DiscordClientActivity = clientActivity;
            s_UpdateClientActivityHandler = new ActivityManager.UpdateActivityHandler(handleActivityUpdate);

            s_Initialized = true;
        }

        public void InterruptDiscord(bool setHasError = true)
        {
            s_DiscordClient = null;
            s_Initialized = false;
            base.enabled = false;
        }

        /// <summary>
        /// Destroying discord will result app quit
        /// </summary>
        public void DestroyDiscord()
        {
            if (!s_Initialized)
                return;

            s_DiscordClient.Dispose();
        }

        public void UpdateActivity()
        {
            if (!HasInitialized)
                return;

            updateCurrentGamemodeInfo();
            updatedDetailsInfo();

            s_DiscordClientActivity.State = !string.IsNullOrEmpty(CurrentGamemodeDetails) ? CurrentGamemode + " [" + CurrentGamemodeDetails + "]" : CurrentGamemode;

            ActivityManager manager = s_DiscordClient.GetActivityManager();
            if (manager == null)
            {
                InterruptDiscord();
                return;
            }

            manager.UpdateActivity(s_DiscordClientActivity, s_UpdateClientActivityHandler);
        }

        private void handleActivityUpdate(Result res)
        {
            if (res != Result.Ok)
                InterruptDiscord();
        }

        /// <summary>
        /// Update <see cref="CurrentGamemode"/> value
        /// </summary>
        private void updateCurrentGamemodeInfo()
        {
            GameMode gm = GameFlowManager.Instance.GetCurrentGameMode();
            string gamemodeString = "Unknown game mode";
            switch (gm)
            {
                case GameMode.Adventure:
                    gamemodeString = "Adventure";
                    break;
                case GameMode.BattleRoyale:
                    gamemodeString = "LBS Match";
                    break;
                case GameMode.Challenge:
                    gamemodeString = "Challenge";
                    break;
                case GameMode.CoopChallenge:
                    gamemodeString = "Co-op challenge";
                    break;
                case GameMode.Endless:
                    gamemodeString = "Endless mode";
                    break;
                case GameMode.EndlessCoop:
                    gamemodeString = "Co-op endless mode";
                    break;
                case GameMode.LevelEditor:
                    gamemodeString = "Level Editor";
                    break;
                case GameMode.MultiplayerDuel:
                    gamemodeString = "Duel";
                    break;
                case GameMode.SingleLevelPlaytest:
                    gamemodeString = "Level Editor";
                    break;
                case GameMode.Story:
                    gamemodeString = "Story mode";
                    break;
                case GameMode.Twitch:
                    gamemodeString = "Twitch mode";
                    break;
                case GameMode.None:
                    gamemodeString = "Main menu";
                    break;
            }

            CurrentGamemode = gamemodeString;
        }

        /// <summary>
        /// Update <see cref="CurrentGamemodeDetails"/> value
        /// </summary>
        private void updatedDetailsInfo()
        {
            CurrentGamemodeDetails = string.Empty;
            if (!GameFlowManager.Instance)
                return;

            GameMode gm = GameFlowManager.Instance.GetCurrentGameMode();
            switch (gm)
            {
                case GameMode.Story:
                    GameDataManager datam = GameDataManager.Instance;
                    MetagameProgressManager metaG = MetagameProgressManager.Instance;
                    if(!datam || !metaG)
                        return;

                    int levelsBeatenSm = 0;
                    int chapterNumber = 1;
                    if (metaG.CurrentProgressHasReached(MetagameProgress.P10_ConqueredBattlecruiser))
                    {
                        chapterNumber = 5;
                    }
                    else if (metaG.CurrentProgressHasReached(MetagameProgress.P7_CompletedTowerAssault))
                    {
                        chapterNumber = 4;
                    }
                    else if (metaG.CurrentProgressHasReached(MetagameProgress.P5_DestroyedAlphaCentauri))
                    {
                        chapterNumber = 3;
                    }
                    else if (metaG.CurrentProgressHasReached(MetagameProgress.P2_FirstHumanEscaped))
                    {
                        chapterNumber = 2;
                        levelsBeatenSm = datam.GetNumberOfStoryLevelsWon() + 1;
                    }
                    else if (metaG.CurrentProgressHasReached(MetagameProgress.P0_None))
                    {
                        chapterNumber = 1;
                        levelsBeatenSm = datam.GetNumberOfStoryLevelsWon() + 1;
                    }

                    CurrentGamemodeDetails = levelsBeatenSm == 0 ? "Chapter " + chapterNumber : "Chapter " + chapterNumber + ", Level " + levelsBeatenSm;
                    break;

                case GameMode.Endless:
                    if (!LevelManager.Instance || !EndlessModeManager.Instance)
                        return;

                    int levelsBeaten = LevelManager.Instance.GetNumberOfLevelsWon() + 1;
                    string tier = EndlessModeManager.Instance.GetNextLevelDifficultyTier(levelsBeaten - 1).ToString();
                    CurrentGamemodeDetails = "Level " + levelsBeaten + ", " + tier;
                    break;

                case GameMode.EndlessCoop:
                    if (!LevelManager.Instance || !EndlessModeManager.Instance)
                        return;

                    int levelsBeaten2 = LevelManager.Instance.GetNumberOfLevelsWon() + 1;
                    string tier2 = EndlessModeManager.Instance.GetNextLevelDifficultyTier(levelsBeaten2 - 1).ToString();
                    CurrentGamemodeDetails = "Level " + levelsBeaten2 + ", " + tier2;
                    break;

                case GameMode.BattleRoyale:
                    if (!MultiplayerMatchmakingManager.Instance)
                        return;

                    CurrentGamemodeDetails = "Connecting to lobby...";
                    GameRequest request = MultiplayerMatchmakingManager.Instance.GetPrivateField<GameRequest>("_currentGameRequest");
                    if (request != null)
                    {
                        BattleRoyaleManager brManager = BattleRoyaleManager.Instance;
                        if (!brManager)
                            return;

                        string regionString = "N/A Region";
                        if (request.ForceRegion.HasValue)
                        {
                            CustomRegion reg = request.ForceRegion.Value;
                            regionString = reg.ToString();
                        }

                        string privateString = brManager.IsPrivateMatch() ? "Private lobby" : "Public lobby";
                        CurrentGamemodeDetails = privateString + ", " + regionString;
                    }
                    break;
            }
        }
    }
}
