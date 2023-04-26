using Discord;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul
{
    internal class OverhaulDiscordController : OverhaulBehaviour
    {
        [OverhaulSetting("Gameplay.Discord.Enable Overhaul Activity", true, false, "Restart the game if you've toggled this setting")]
        public static bool EnableDiscordActivity;

        public const long OverhaulClientID = 1091373211163308073;
        public const CreateFlags CreateFlag = CreateFlags.NoRequireDiscord;

        public static OverhaulDiscordController Instance;
        private static Discord.Discord m_Client;
        private static Discord.Activity m_ClientActivity;
        private static Discord.ActivityManager.UpdateActivityHandler m_ActUpdHandler;

        private static bool m_HasInitialized;
        private static bool m_HasError;

        public static bool SuccessfulInitialization => !m_HasError &&
            m_HasInitialized &&
            m_Client != null &&
            Instance != null;

        /// <summary>
        /// The gamemode player is playing right now
        /// </summary>
        public string CurrentGamemode { get; set; }

        /// <summary>
        /// The details of gamemode (Progress)
        /// </summary>
        public string CurrentGamemodeDetails { get; set; }

        public long UserID
        {
            get
            {
                if (!SuccessfulInitialization)
                {
                    return 0;
                }

                UserManager m = m_Client.GetUserManager();
                if(m == null)
                {
                    return 0;
                }

                User u = m.GetCurrentUser();
                return u.Id;
            }
        }

        private float m_TimeLeftToRefresh;

        private float m_UnscaledTimeToInitializeDiscord;

        private void Start()
        {
            Instance = this;
            m_UnscaledTimeToInitializeDiscord = EnableDiscordActivity ? Time.unscaledTime + 1.5f : -1f;
        }

        private void Update()
        {
            if (m_UnscaledTimeToInitializeDiscord != -1f && Time.unscaledTime >= m_UnscaledTimeToInitializeDiscord)
            {
                TryInitializeDiscord();
                m_UnscaledTimeToInitializeDiscord = -1f;
            }

            if (!SuccessfulInitialization)
            {
                return;
            }

            try
            {
                m_Client.RunCallbacks();
            }
            catch
            {
                InterruptDiscord();
                return;
            }

            m_TimeLeftToRefresh -= Time.deltaTime;
            if (m_TimeLeftToRefresh <= 0f && m_Client != null)
            {
                m_TimeLeftToRefresh = 5f;
                updateGamemodeString();
                updateDetailsString();
                UpdateActivity();
            }
        }

        private void OnApplicationQuit()
        {
            DestroyDiscord();
        }

        public void TryInitializeDiscord()
        {
            if (m_HasError || m_HasInitialized)
            {
                return;
            }

            Discord.Discord client = null;
            Activity clientActivity;
            ActivityAssets clientActivityAssets;

            try
            {
                client = new Discord.Discord(OverhaulClientID, (ulong)CreateFlag);
            }
            catch
            {
                InterruptDiscord();
                return;
            }

            m_Client = client;
            clientActivityAssets = new ActivityAssets
            {
                LargeImage = "defaultimage"
            };
            clientActivity = new Activity
            {
                Assets = clientActivityAssets,
                ApplicationId = 1091373211163308073,
                Name = "Overhaul mod",
                Details = "v" + OverhaulVersion.ModVersion.ToString(),
                Type = ActivityType.Playing
            };
            m_ClientActivity = clientActivity;
            m_ActUpdHandler = new ActivityManager.UpdateActivityHandler(handleActivityUpdate);

            m_HasInitialized = true;
        }

        public void InterruptDiscord(bool setHasError = true)
        {
            m_Client = null;
            m_HasInitialized = false;
            m_HasError = setHasError;
            base.enabled = false;
        }

        /// <summary>
        /// Destroying discord will result app quit
        /// </summary>
        public void DestroyDiscord()
        {
            if (m_Client == null)
            {
                return;
            }

            m_Client.Dispose();
        }

        public void UpdateActivity()
        {
            if (!SuccessfulInitialization)
            {
                return;
            }

            m_ClientActivity.State = !string.IsNullOrEmpty(CurrentGamemodeDetails) ? CurrentGamemode + " [" + CurrentGamemodeDetails + "]" : CurrentGamemode;

            ActivityManager manager = m_Client.GetActivityManager();
            if (manager == null)
            {
                InterruptDiscord();
                return;
            }

            manager.UpdateActivity(m_ClientActivity, m_ActUpdHandler);
        }

        private void handleActivityUpdate(Result res)
        {
            if (!SuccessfulInitialization)
            {
                return;
            }

            if (res != Result.Ok)
            {
                InterruptDiscord();
            }
        }

        /// <summary>
        /// Update <see cref="CurrentGamemode"/> value
        /// </summary>
        private void updateGamemodeString()
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
                    gamemodeString = "In main menu";
                    break;
            }

            CurrentGamemode = gamemodeString;
        }

        /// <summary>
        /// Update <see cref="CurrentGamemodeDetails"/> value
        /// </summary>
        private void updateDetailsString()
        {
            CurrentGamemodeDetails = string.Empty;
            GameMode gm = GameFlowManager.Instance.GetCurrentGameMode();
            switch (gm)
            {
                case GameMode.Story:
                    GameDataManager datam = GameDataManager.Instance;
                    MetagameProgressManager metaG = MetagameProgressManager.Instance;

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
                    int levelsBeaten = LevelManager.Instance.GetNumberOfLevelsWon() + 1;
                    string tier = EndlessModeManager.Instance.GetNextLevelDifficultyTier(levelsBeaten - 1).ToString();
                    CurrentGamemodeDetails = "Level " + levelsBeaten + ", " + tier;
                    break;

                case GameMode.EndlessCoop:
                    int levelsBeaten2 = LevelManager.Instance.GetNumberOfLevelsWon() + 1;
                    string tier2 = EndlessModeManager.Instance.GetNextLevelDifficultyTier(levelsBeaten2 - 1).ToString();
                    CurrentGamemodeDetails = "Level " + levelsBeaten2 + ", " + tier2;
                    break;

                case GameMode.BattleRoyale:
                    CurrentGamemodeDetails = "Connecting to lobby...";
                    GameRequest request = MultiplayerMatchmakingManager.Instance.GetPrivateField<GameRequest>("_currentGameRequest");
                    if (request != null)
                    {
                        BattleRoyaleManager brManager = BattleRoyaleManager.Instance;
                        if (brManager == null)
                        {
                            return;
                        }

                        string regionString = "Unknown region";
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
