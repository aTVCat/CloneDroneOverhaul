using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul
{
    internal class OverhaulDiscordRPCController : OverhaulBehaviour
    {
        public static OverhaulDiscordRPCController Instance;

        private static Discord.Discord m_Client;
        private static Discord.Activity m_ClientActivity;
        private static Discord.ActivityManager.UpdateActivityHandler m_ActUpdHandler;

        private static bool m_HasInitialized;

        private string m_CurrentGamemode;
        public string CurrentGamemode
        {
            get
            {
                return m_CurrentGamemode;
            }
            set
            {
                m_CurrentGamemode = value;
            }
        }

        private string m_GamemodeDetails;
        public string CurrentGamemodeDetails
        {
            get
            {
                return m_GamemodeDetails;
            }
            set
            {
                m_GamemodeDetails = value;
            }
        }

        private float m_TimeLeftToRefresh;

        private void Start()
        {
            if (!m_HasInitialized)
            {
                ActivityAssets actAssets = new ActivityAssets();
                actAssets.LargeImage = "clonedroneoverhaulimage03placeholder";

                m_ClientActivity = new Activity();
                m_ClientActivity.Assets = actAssets;
                m_ClientActivity.ApplicationId = 1091373211163308073;
                m_ClientActivity.Name = "Overhaul mod";
                m_ClientActivity.Details = "v" + OverhaulVersion.ModVersion.ToString();
                m_ClientActivity.Type = ActivityType.Playing;

                m_ActUpdHandler = new ActivityManager.UpdateActivityHandler(handleActUpdate);

                m_Client = new Discord.Discord(1091373211163308073, (UInt64)global::Discord.CreateFlags.Default);
                m_HasInitialized = true;
            }
            Instance = this;
        }

        private void Update()
        {
            // This one is required to make it work
            if(m_Client != null)
            {
                m_Client.RunCallbacks();
            }

            m_TimeLeftToRefresh -= Time.deltaTime;
            if(m_TimeLeftToRefresh <= 0f && m_Client != null)
            {
                m_TimeLeftToRefresh = 5f;
                updateGamemodeString();
                updateDetailsString();
                UpdateRPC();
            }
        }

        private void OnApplicationQuit()
        {
            DestroyDiscord();
        }

        private void handleActUpdate(Result res)
        {
        }

        public void UpdateRPC()
        {
            if (!string.IsNullOrEmpty(CurrentGamemodeDetails))
            {
                m_ClientActivity.State = CurrentGamemode + " [" + CurrentGamemodeDetails + "]";
            }
            else
            {
                m_ClientActivity.State = CurrentGamemode;
            }
            m_Client.GetActivityManager().UpdateActivity(m_ClientActivity, m_ActUpdHandler);
        }

        private void updateGamemodeString()
        {
            GameMode gm = GameFlowManager.Instance.GetCurrentGameMode();
            string gamemodeString = "In Menu";
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
            }

            CurrentGamemode = gamemodeString;
        }

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

                    if(levelsBeatenSm == 0)
                    {
                        CurrentGamemodeDetails = "Chapter " + chapterNumber;
                    }
                    else
                    {
                        CurrentGamemodeDetails = "Chapter " + chapterNumber + ", Level " + levelsBeatenSm;
                    }
                    break;

                case GameMode.Endless:
                    int levelsBeaten = LevelManager.Instance.GetNumberOfLevelsWon() + 1;
                    string tier = EndlessModeManager.Instance.GetNextLevelDifficultyTier(levelsBeaten-1).ToString();
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
                    if(request != null)
                    {
                        BattleRoyaleManager brManager = BattleRoyaleManager.Instance;
                        if(brManager == null)
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

        public void DestroyDiscord()
        {
            m_HasInitialized = false;
            if (m_Client != null)
            {
                m_Client.Dispose();
            }
        }
    }
}
