using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class RichPresenceBase : OverhaulBehaviour
    {
        private float m_TimeToRefresh;

        /// <summary>
        /// The gamemode player is playing right now
        /// </summary>
        public string gameModeString
        {
            get;
            set;
        }

        /// <summary>
        /// The details of gamemode (Progress for example)
        /// </summary>
        public string gameModeDetailsString
        {
            get;
            set;
        }

        protected float GetRefreshRate() => 1f;

        public virtual void RefreshInformation()
        {
            gameModeString = GetGameModeString(GameFlowManager.Instance._gameMode);
            gameModeDetailsString = GetGameModeDetailsString();
        }

        public override void Update()
        {
            float time = Time.unscaledTime;
            if (time >= m_TimeToRefresh)
            {
                m_TimeToRefresh = time + GetRefreshRate();
                RefreshInformation();
            }
        }

        public static string GetGameModeString(GameMode gameMode)
        {
            string gameModeString = "SinglePlayer";
            switch (gameMode)
            {
                case GameMode.Adventure:
                    gameModeString = "Adventure";
                    break;
                case GameMode.BattleRoyale:
                    gameModeString = "LBS Match";
                    break;
                case GameMode.Challenge:
                    gameModeString = "Challenge";
                    break;
                case GameMode.CoopChallenge:
                    gameModeString = "Co-op challenge";
                    break;
                case GameMode.Endless:
                    gameModeString = "Endless mode";
                    break;
                case GameMode.EndlessCoop:
                    gameModeString = "Co-op endless mode";
                    break;
                case GameMode.LevelEditor:
                    gameModeString = "Level Editor";
                    break;
                case GameMode.MultiplayerDuel:
                    gameModeString = "Duel";
                    break;
                case GameMode.SingleLevelPlaytest:
                    gameModeString = "Level Editor";
                    break;
                case GameMode.Story:
                    gameModeString = "Story mode";
                    break;
                case GameMode.Twitch:
                    gameModeString = "Twitch mode";
                    break;
                case GameMode.None:
                    gameModeString = "Main menu";
                    break;
                case (GameMode)2500:
                    gameModeString = "Customization editor";
                    break;
            }
            return gameModeString;
        }
        /// <summary>
        /// Update <see cref="gameModeDetailsString"/> value
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

            switch (GameFlowManager.Instance._gameMode)
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

                case GameMode.EndlessCoop:
                case GameMode.Endless:
                    if (!levelManager || !endlessModeManager)
                        return result;

                    int levelsWon = levelManager.GetNumberOfLevelsWon() + 1;
                    DifficultyTier tier = endlessModeManager.GetNextLevelDifficultyTier(levelsWon - 1);

                    string tierString;
                    switch (tier)
                    {
                        case (DifficultyTier)9:
                            tierString = "Nightmarium";
                            break;
                        default:
                            tierString = tier.ToString();
                            break;
                    }

                    result = "Level " + levelsWon + " · " + tierString;
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

                        string regionString = null;
                        if (request.ForceRegion != null)
                            regionString = "Region: " + request.ForceRegion.Value.ToString();

                        string separator = string.IsNullOrEmpty(regionString) ? string.Empty : " · ";
                        string lobbyPrivacyString = brManager.IsPrivateMatch() ? "Private lobby" : "Public lobby";
                        result = lobbyPrivacyString + separator + regionString;
                    }
                    break;
                case GameMode.LevelEditor:
                    LevelEditorDataManager levelEditorDataManager = LevelEditorDataManager.Instance;
                    if (!levelEditorDataManager)
                        return result;

                    string path = levelEditorDataManager.GetCurrentlyEditedLevelPath();
                    if (path.IsNullOrEmpty())
                        return result;

                    result = path.Contains("/") ? "Editing " + path.Substring(path.LastIndexOf('/') + 1) : "Editing " + path;

                    break;
            }
            return result;
        }
    }
}
