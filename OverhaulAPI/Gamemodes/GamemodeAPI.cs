using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OverhaulAPI
{
    public static class GamemodeAPI
    {
        private static readonly List<IOverhaulGamemode> _gamemodes = new List<IOverhaulGamemode>();
        public static void Reset() => _gamemodes.Clear();
        public static bool HasAlreadyAddedSameGamemode(IOverhaulGamemode gamemode) { foreach (IOverhaulGamemode g in _gamemodes) { if (g.GetGameMode() == gamemode.GetGameMode()) return true; } return false; }

        /// <summary>
        /// Set gamemode
        /// </summary>
        /// <param name="gamemode"></param>
        public static void SetGamemode(GameMode gamemode)
        {
            GameFlowManager.Instance.SetPrivateField<GameMode>("_gameMode", gamemode);
        }

        /// <summary>
        /// Create an instance of gamemode controller and implement it into game
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateGamemodeController<T>() where T : IOverhaulGamemode
        {
            T result = Activator.CreateInstance<T>();
            GameModeCardData newCardData = null;

            FindGamemodeErrors(result);

            newCardData = MakeCardData(result);
            ImplementGamemode(result, newCardData);

            _gamemodes.Add(result);

            return result;
        }

        /// <summary>
        /// Check gamemode controller for errors
        /// </summary>
        /// <param name="gamemode"></param>
        public static void FindGamemodeErrors(IOverhaulGamemode gamemode)
        {
            if (gamemode.GetGameMode() < (GameMode)12)
            {
                API.ThrowException(gamemode.GetType() + " - " + "Cannot use vanilla game gamemode type. [" + gamemode.GetGameMode().ToString() + "]");
            }
            else if (string.IsNullOrWhiteSpace(gamemode.GetGamemodeName()))
            {
                API.ThrowException(gamemode.GetType() + " - " + "Unsupported form of gamemode naming. [" + gamemode.GetGamemodeName() + "]");
            }
            else if (string.IsNullOrWhiteSpace(gamemode.GetGamemodeDescription()))
            {
                API.ThrowException(gamemode.GetType() + " - " + "Unsupported form of gamemode description. [" + gamemode.GetGamemodeDescription() + "]");
            }
            else if (HasAlreadyAddedSameGamemode(gamemode))
            {
                API.ThrowException(gamemode.GetType() + " - " + "The gamemode controller with the same gamemode is already added. [" + gamemode.GetGameMode() + "]");
            }
        }

        /// <summary>
        /// Create gamemode description that game can understand
        /// </summary>
        /// <param name="gamemode"></param>
        /// <returns></returns>
        public static GameModeCardData MakeCardData(IOverhaulGamemode gamemode)
        {
            GameModeCardData data = new GameModeCardData
            {
                NameOfMode = gamemode.GetGamemodeName(),
                Description = gamemode.GetGamemodeDescription(),
                ClickedCallback = new UnityEngine.Events.UnityEvent()
            };
            data.ClickedCallback.AddListener(delegate { ActivateGamemodeController(gamemode); });
            return data;
        }

        /// <summary>
        /// Add card data to singleplayer gamemode selection screen
        /// </summary>
        /// <param name="cardData"></param>
        public static void AddCardData(GameModeCardData cardData)
        {
            List<GameModeCardData> data = GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen.GameModeData.ToList();
            data.Add(cardData);
            GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen.GameModeData = data.ToArray();
        }

        /// <summary>
        /// Fully implement a gamemode controller into game
        /// </summary>
        /// <param name="gamemode"></param>
        /// <param name="cardData"></param>
        public static void ImplementGamemode(IOverhaulGamemode gamemode, GameModeCardData cardData)
        {
            if (gamemode == null && cardData == null)
            {
                API.ThrowException(gamemode.GetType() + " - " + "Cannot implement gamemode. [ Gamemode & CardData are NULL ]");
            }
            else if (gamemode == null)
            {
                API.ThrowException(gamemode.GetType() + " - " + "Cannot implement gamemode. [ Gamemode is NULL ]");
            }
            else if (cardData == null)
            {
                API.ThrowException(gamemode.GetType() + " - " + "Cannot implement gamemode. [ CardData is NULL ]");
            }
            CheckVanillaHUD();

            AddCardData(cardData);
        }

        public static void ActivateGamemodeController(IOverhaulGamemode gamemode)
        {
            if (gamemode == null)
            {
                API.ThrowException(gamemode.GetType() + " - " + "Cannot activate gamemode. [ Gamemode is NULL ]");
            }
            CheckVanillaHUD();

            SetGamemode(gamemode.GetGameMode());
            GameDataManager.Instance.SaveHighScoreDataWithoutModifyingIt();
            GameFlowManager.Instance.HideTitleScreen(true);
            GameFlowManager.Instance.ResetGameDataToStart();
            GameFlowManager.Instance.ResetArenaGameStateBooleans();
            LevelManager.Instance.CleanUpLevelThisFrame();
            CacheManager.Instance.CreateOrClearInstance();
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                if (SceneTransitionManager.Instance.IsDisconnecting())
                {
                    return;
                }

                gamemode.StartGamemode();
                LevelManager.Instance.SetPrivateField<bool>("_currentLevelHidesTheArena", gamemode.HideArena());
                GlobalEventManager.Instance.Dispatch(GlobalEvents.HideArenaToggleChanged);
                GameFlowManager.Instance.CallPrivateMethod("createPlayerAndSetLift", null);
                gamemode.OnPlayerSpawned();
                GlobalEventManager.Instance.Dispatch("GameRestarted");
            });
        }

        public static void CheckVanillaHUD()
        {
            if (GameUIRoot.Instance == null)
            {
                API.ThrowException("GameUIRoot.Instance is NULL");
            }
            else if (GameUIRoot.Instance.TitleScreenUI == null)
            {
                API.ThrowException("GameUIRoot.Instance.TitleScreenUI is NULL");
            }
            else if (GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen == null)
            {
                API.ThrowException("GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen is NULL");
            }
            else if (GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen.GameModeData == null)
            {
                API.ThrowException("GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen.GameModeData is NULL");
            }
        }
    }
}
