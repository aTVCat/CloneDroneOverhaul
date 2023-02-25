using ModLibrary;
using System;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class OverhaulGameplayCoreController : OverhaulController
    {
        #region Events
        public const string GamemodeChangedEventString = "GamemodeChanged";
        public const string MainCameraSwitchedEventString = "CameraMainSwitched";
        public const string CurrentCameraSwitchedEventString = "CameraCurrentSwitched";

        public const string FirstPersonMoverSpawnedEventString = "FPMSpawned";
        public const string FirstPersonMoverSpawned_DelayEventString = "FPMSpawned_Delay";

        public const string PlayerSetAsCharacter = "PlayerSet_Base";
        public const string PlayerSetAsFirstPersonMover = "PlayerSet_FirstPersonMover";
        #endregion

        /// <summary>
        /// The start index of mod gamemodes
        /// </summary>
        public const int GamemodeStartIndex = 2000;

        public static OverhaulGameplayCoreController Instance
        {
            get;
            private set;
        }

        public GameFlowManager GameFlowManager
        {
            get;
            private set;
        }

        public LevelController Levels
        {
            get;
            private set;
        }

        public PlayerOutfitController Outfit
        {
            get;
            private set;
        }

        public GamemodeSubstatesController GamemodeSubstates { get; private set; }

        private GameMode _gamemodeLastFrame;
        private Camera _cameraLastFrame;
        private Camera _cameraCurrentLastFrame;

        public override void Initialize()
        {
            Instance = this;

            GameFlowManager = GameFlowManager.Instance;
            GamemodeSubstates = OverhaulController.InitializeController<GamemodeSubstatesController>();
            Levels = OverhaulController.InitializeController<LevelController>();
            _ = OverhaulController.InitializeController<WeaponSkinsControllerV2>();
            Outfit = OverhaulController.InitializeController<PlayerOutfitController>();

            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);
        }

        /// <summary>
        /// Set gamemode, spawn level and player
        /// </summary>
        /// <param name="gamemode"></param>
        /// <param name="levelData"></param>
        /// <param name="spawnLevel"></param>
        /// <param name="spawnPlayer"></param>
        public void StartGamemode(GameMode gamemode, LevelEditorLevelData levelData, Action onPlayerSpawned = null, bool spawnLevel = true, bool spawnPlayer = true)
        {
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                SetGamemode(gamemode);

                GameUIRoot.Instance.TitleScreenUI.Hide();
                GameUIRoot.Instance.SetPlayerHUDVisible(spawnPlayer);

                if (spawnLevel)
                {
                    LevelManager.Instance.CleanUpLevelThisFrame();
                }

                if (spawnLevel)
                {
                    _ = Levels.SpawnLevel(levelData, "Level" + UnityEngine.Random.Range(0, 100).ToString(), true, delegate { if (spawnPlayer) { SpawnPlayer(); if (onPlayerSpawned != null) { onPlayerSpawned(); } } });
                }
            });
        }

        /// <summary>
        /// Set gamemode specified in <paramref name="mode"/>
        /// </summary>
        /// <param name="mode"></param>
        public void SetGamemode(in GameMode mode)
        {
            GameFlowManager.SetPrivateField<GameMode>("_gameMode", mode);
        }

        /// <summary>
        /// Spawn player if <see cref="AdventureCheckPoint"/> would be found
        /// </summary>
        public void SpawnPlayer()
        {
            AdventureCheckPoint point = FindObjectOfType<AdventureCheckPoint>();
            if (point != null)
            {
                _ = GameFlowManager.SpawnPlayer(point.transform, true, true, null);
            }
            //GameFlow.CallPrivateMethod("createPlayerAndSetLift");
        }

        /// <summary>
        /// Send <see cref="GamemodeChangedEventString"/> event
        /// </summary>
        private void sendGamemodeWasUpdateEvent()
        {
            OverhaulEventManager.DispatchEvent(GamemodeChangedEventString);
        }

        private void Update()
        {
            GameMode currentGamemode = GameFlowManager.GetCurrentGameMode();
            if (currentGamemode != _gamemodeLastFrame)
            {
                sendGamemodeWasUpdateEvent();
            }
            _gamemodeLastFrame = currentGamemode;

            Camera mainCamera = Camera.main;
            if (mainCamera != _cameraLastFrame)
            {
                OverhaulEventManager.DispatchEvent(MainCameraSwitchedEventString, mainCamera);
            }
            _cameraLastFrame = mainCamera;

            Camera currentCamera = Camera.current;
            if (currentCamera != _cameraCurrentLastFrame)
            {
                OverhaulEventManager.DispatchEvent(CurrentCameraSwitchedEventString, currentCamera);
            }
            _cameraCurrentLastFrame = currentCamera;
        }

        public override string[] Commands()
        {
            throw new NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}