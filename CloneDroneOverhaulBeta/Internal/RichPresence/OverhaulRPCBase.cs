using UnityEngine;

namespace CDOverhaul.RichPresence
{
    public class OverhaulRPCBase : OverhaulBehaviour
    {
        private float m_TimeToRefresh;

        public bool initialized
        {
            get;
            protected set;
        }

        /// <summary>
        /// The gamemode player is playing right now
        /// </summary>
        public string GameModeString
        {
            get;
            set;
        }

        /// <summary>
        /// The details of gamemode (Progress for example)
        /// </summary>
        public string GameModeDetailsString
        {
            get;
            set;
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        protected float GetRefreshRate() => 1f;

        public virtual void RefreshInformation()
        {
            GameModeString = GetGameModeString(GameFlowManager.Instance._gameMode);
            GameModeDetailsString = GetGameModeDetailsString();
        }

        protected virtual void Update()
        {
            if (!initialized)
                return;

            float time = Time.unscaledTime;
            if (time >= m_TimeToRefresh)
            {
                m_TimeToRefresh = time + GetRefreshRate();
                RefreshInformation();
            }
        }

        public static string GetGameModeString(GameMode gameMode)
        {
            string gamemodeString = "SinglePlayer";
            switch (gameMode)
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
                case (GameMode)20:
                    gamemodeString = "SandBox";
                    break;
            }
            return gamemodeString;
        }
        /// <summary>
        /// Update <see cref="GameModeDetailsString"/> value
        /// </summary>
        public static string GetGameModeDetailsString()
        {
            string result = string.Empty;
            if (!GameFlowManager.Instance)
                return result;

            LevelManager levelManager = LevelManager.Instance;
            EndlessModeManager endlessModeManager = EndlessModeManager.Instance;
            GameDataManager gameDataManager = GameDataManager.Instance;
            MetagameProgressManager metagameManager = MetagameProgressManager.Instance;

            GameMode gameMode = GameFlowManager.Instance._gameMode;
            if (gameMode == GameMode.EndlessCoop)
                gameMode = GameMode.Endless;

            switch (gameMode)
            {
                case GameMode.Story:
                    if (!gameDataManager || !metagameManager)
                        return result;

                    int levelsBeatenSm = 0;
                    int chapterNumber = 1;
                    if (metagameManager.CurrentProgressHasReached(MetagameProgress.P10_ConqueredBattlecruiser))
                        chapterNumber = 5;
                    else if (metagameManager.CurrentProgressHasReached(MetagameProgress.P7_CompletedTowerAssault))
                        chapterNumber = 4;
                    else if (metagameManager.CurrentProgressHasReached(MetagameProgress.P5_DestroyedAlphaCentauri))
                        chapterNumber = 3;
                    else if (metagameManager.CurrentProgressHasReached(MetagameProgress.P2_FirstHumanEscaped))
                    {
                        chapterNumber = 2;
                        levelsBeatenSm = gameDataManager.GetNumberOfStoryLevelsWon() + 1;
                    }
                    else if (metagameManager.CurrentProgressHasReached(MetagameProgress.P0_None))
                    {
                        chapterNumber = 1;
                        levelsBeatenSm = gameDataManager.GetNumberOfStoryLevelsWon() + 1;
                    }
                    result = levelsBeatenSm == 0 ? "Chapter " + chapterNumber : "Chapter " + chapterNumber + " ·  Level " + levelsBeatenSm;
                    break;

                case GameMode.Endless:
                    if (!levelManager || !endlessModeManager)
                        return result;

                    int levelsBeaten = levelManager.GetNumberOfLevelsWon() + 1;
                    string tier = endlessModeManager.GetNextLevelDifficultyTier(levelsBeaten - 1).ToString();
                    result = "Level " + levelsBeaten + " · " + tier;
                    break;

                case GameMode.BattleRoyale:
                    MultiplayerMatchmakingManager multiplayerMatchmakingManager = MultiplayerMatchmakingManager.Instance;
                    if (!multiplayerMatchmakingManager)
                        return result;

                    GameRequest request = multiplayerMatchmakingManager._currentGameRequest;
                    if (request != null)
                    {
                        BattleRoyaleManager brManager = BattleRoyaleManager.Instance;
                        if (!brManager)
                            return result;

                        string regionString = string.Empty;
                        if (request.ForceRegion != null)
                            regionString = "Region: " + request.ForceRegion.Value.ToString();

                        string separator = string.IsNullOrEmpty(regionString) ? string.Empty : " · ";
                        string privateString = brManager.IsPrivateMatch() ? "Private lobby" : "Public lobby";
                        result = privateString + separator + regionString;
                    }
                    break;
            }
            return result;
        }
    }
}
